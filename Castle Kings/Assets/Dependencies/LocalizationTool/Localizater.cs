using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

public class Localizater : MonoBehaviour {

    public enum SupportedCountry
    {
        Canada,
        France,
        UnitedKingdom,
        UnitedStates,
        Azeroth,
        Orchindoom
    }

    public enum SupportedLanguage
    {
        English,
        French,
        Orcish
    }

    public SupportedCountry m_country;
    public SupportedLanguage m_language;

    private static Localizater g_instance;
#if UNITY_EDITOR
	private const string SAVE_PATH = "Assets/Data/data.asset";
	private static SavedData savedData;
#endif

    //Will print an examepl of use of the localization system.
	void Awake()
	{
#if UNITY_EDITOR
		savedData = AssetDatabase.LoadAssetAtPath<SavedData>(SAVE_PATH);
		
		if (savedData == null) {
			savedData = ScriptableObject.CreateInstance<SavedData> ();
			AssetDatabase.CreateAsset (savedData, SAVE_PATH);
		}
#endif

        if (g_instance == null)
        {
            g_instance = this;
        }
        else
        {
            Debug.Log("Localizater already initialized... Destroying duplicate.");
        }

        DontDestroyOnLoad(this.gameObject);

#if UNITY_EDITOR
        Debug.Log("Attempt at translating unrecognized word: " + IDToWord("StartMessage"));
#endif
    }

    public static string GetTranslationFor(string p_toTranslate) {
        return g_instance.IDToWord(p_toTranslate);
    }
	
	//Fetch translation based on Language and Country for the specific ID
	string IDToWord(string ID)
	{
		//If the word wasn't found, some people might
		int theIndex = -1;
		string theTranslation = "UNHANDLED TRANSLATION";

		//If the ID was Legit, ignoring language/country restrictions
		theIndex = savedData.savedIDs.FindIndex(x => x == ID);
		if (theIndex != -1) {
			//If the language was valid
			Language zeLang = savedData.savedLanguages.Find (x => x.mName == m_language.ToString());
			if (zeLang != null) {
				//If the Country for that language was invalid or if the country for that language was valid
				// but the translation wasn't enabled, fallback to default country.
				Country zeCountry = zeLang.countries.Find (x => x.mName == m_country.ToString());
				if (zeCountry == null || !zeCountry.entries[theIndex].mEnabled)
				{
					zeCountry = zeLang.countries.Find (x => x.mName == "DEFAULT");
				}
				//If the final found ID is enabled, return the translation
				if (zeCountry.entries[theIndex].mEnabled)
				{
					theTranslation = zeCountry.entries[theIndex].mTranslation;
				}
			}
		}
		return theTranslation;
	}
}
