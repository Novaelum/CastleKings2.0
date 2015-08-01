#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
	
public class LocalizationWindow : EditorWindow {
	//Serialization related related
	private const string SAVE_PATH = "Assets/Data/data.asset";
	private SavedData savedData;

	//IDs are global, every localization uses said list
	public static List<string> ids = new List<string>();

	//Each languages has a list of localizations
	public static List<Language> languages = new List<Language>();

	public static bool hasSaved;

	//Used to display the list of languages and countries throughout the pop-up
	string[] languagesAsArray;
	string[] countriesAsArray;

	//Represent the currently selected language/country from the pop-up menus
	int currentLanguage;
	int currentCountry;

	//These strings are used for our multiple textfields (New language, Countries or ID)
	string newLanguageField;
	string newCountryField;
	string newIDField;


    [MenuItem ("Window/Localization Window")]

	public static void ShowWindow() 
	{
		EditorWindow.GetWindow(typeof(LocalizationWindow));
	}

	void OnGUI()
	{
		//Changes the distance between labels and fields
		EditorGUIUtility.labelWidth = 100f;

		//Draw everything
		LanguagesGUI();
		CountriesGUI();
		LineGUI();
		IDTranslationGUI ();
		NewIDGUI ();
	}

	//Add an ID to our list of ID, adding the necessary entries across all children of languages.
	void addID(string id)
	{
		if (id != null) {
			if (!ids.Exists (element => element == id)) {
				ids.Add (id);
				foreach (Language lang in languages) {
					foreach (Country co in lang.countries) {
						co.newEntry (id);
					}
				}
			}
		}
	}

	//Remove an ID to our list of IDs and from all the corresponding children of languages.
	void removeID(int index)
	{
		foreach (Language lang in languages)
		{
			foreach (Country co in lang.countries)
			{
				co.removeEntry(index);
			}	
		}
		ids.RemoveAt(index);
	}

	//Adds a language, instantly adding a default country
	void addLanguage(string name)
	{
		Language lang = new Language(name);
		languages.Add(lang);
		lang.addCountry ("DEFAULT");
	}

	//Removes a language, safely.
	void RemoveLanguage(int index)
	{
		for (int i = 0; i < languages[index].countries.Count; i++) 
		{
			languages[index].removeCountry(i);
		}
		languages.RemoveAt(index);
	}

	//Draws the languages part, you may only remove a language if there is one
	void LanguagesGUI()
	{
		GUILayout.BeginHorizontal();
		languagesAsArray = languages.Select(x => x.mName).ToArray();
		currentLanguage = EditorGUILayout.Popup ("Languages: ", currentLanguage, languagesAsArray);
		
		newLanguageField = EditorGUILayout.TextField ("Add Language: ", newLanguageField);
		if(GUILayout.Button ("+")) {
			if (newLanguageField != null)
			{
				addLanguage(newLanguageField);
				newLanguageField = null;
			}
		}
		if (languages.Count != 0) {
			if (GUILayout.Button ("-", GUILayout.Width (20f))) {
				RemoveLanguage (currentLanguage);
				if (currentLanguage != 0) {
					currentLanguage--;
				}
			}
		}
		GUILayout.EndHorizontal ();
	}

	//Draws the countries sections, only displayed when there is an available language.
	//You may not remove the default country from a language.
	void CountriesGUI()
	{
		GUILayout.BeginHorizontal ();
		if (languages.Count != 0) {
			if (languages [currentLanguage].countries.Count != 0) {
				if (languages[currentLanguage].countries.Count < currentCountry)
				{
					currentCountry = 0;
				}
				countriesAsArray = languages [currentLanguage].countries.Select (x => x.mName).ToArray ();
				currentCountry = EditorGUILayout.Popup ("Countries: ", currentCountry, countriesAsArray);
			}
			
			newCountryField = EditorGUILayout.TextField ("Add Country: ", newCountryField);
			if(GUILayout.Button ("+")) {
				if (newCountryField != null)
				{
					languages[currentLanguage].addCountry(newCountryField);
					newCountryField  = null;
				}
			}
			if (languages[currentLanguage].countries.Count != 1 && currentCountry != 0) {
				if(GUILayout.Button ("-", GUILayout.Width(20f))) {
					if (currentCountry != 0) 
					{
						languages[currentLanguage].removeCountry(currentCountry);
						currentCountry--;
					}
				}
			}
		}
		GUILayout.EndHorizontal ();
	}

	//Draws a nice pretty line
	void LineGUI()
	{
		GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
	}

	//Handles the drawing of every ID/Translation entries. The toggle will determine if the
	//Translations should not be considered. Only available when there's a language.
	void IDTranslationGUI()
	{
		if (languages.Count != 0) {
			for (int i =0; i < ids.Count; i++) {
				GUILayout.BeginHorizontal ();
				if (GUILayout.Button ("-", GUILayout.Width (20f))) {
					removeID (i);
					newIDField = null;
					break;
				}
				languages [currentLanguage].countries [currentCountry].entries [i].mEnabled = EditorGUILayout.Toggle (languages [currentLanguage].countries [currentCountry].entries [i].mEnabled, GUILayout.Width (20f));
				GUI.enabled = languages [currentLanguage].countries [currentCountry].entries [i].mEnabled;
				ids [i] = EditorGUILayout.TextField ("ID: ", ids [i], GUILayout.Width (200f));
				languages [currentLanguage].countries [currentCountry].entries [i].mTranslation = EditorGUILayout.TextField ("Translation: ", languages [currentLanguage].countries [currentCountry].entries [i].mTranslation);
				GUILayout.EndHorizontal ();
				GUI.enabled = true;
			}
		}
	}

	//Draws the "Add ID" Button at the end of our list.
	//Only available when there's a language.
	void NewIDGUI()
	{
		if (languages.Count != 0) {
			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("+", GUILayout.Width (20f))) {
				if (newIDField != null)
				{
					addID (newIDField);
					newIDField = null;
				}
			}
			newIDField = EditorGUILayout.TextField ("Add New ID: ", newIDField);
			GUILayout.EndHorizontal ();
		}
	}

	void OnEnable() {
		LoadSettings();
	}
	
	void OnDisable() {
		EditorUtility.SetDirty (savedData);
	}

	private void SaveSettings() 
	{
		savedData.savedIDs = ids;
		savedData.savedLanguages = languages;

	}


	private void LoadSettings() {
		savedData = AssetDatabase.LoadAssetAtPath<SavedData>(SAVE_PATH);
		
		if (savedData == null) {
			savedData = ScriptableObject.CreateInstance<SavedData>();
			AssetDatabase.CreateAsset(savedData, SAVE_PATH);
		}
		ids = savedData.savedIDs;
		languages = savedData.savedLanguages;
	}

}
#endif