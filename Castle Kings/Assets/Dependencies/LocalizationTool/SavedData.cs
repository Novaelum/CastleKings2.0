#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

[Serializable]
public class SavedData : ScriptableObject {
	public List<Language> savedLanguages;
	public List<String> savedIDs;
	
	public SavedData() {
		savedLanguages = new List<Language>();
		savedIDs = new List<String>();
	}
}
#endif
