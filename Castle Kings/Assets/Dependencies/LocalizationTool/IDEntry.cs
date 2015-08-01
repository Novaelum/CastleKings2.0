using System;

//Our IDEntry class holds a translation, it does not know the ID index
//Country handles it
[Serializable]
public class IDEntry
{
	public string mTranslation;
	public bool mEnabled;
	
	//Constructor, entries are disabled by default
	public IDEntry(string translation)
	{
		mTranslation = translation;
		mEnabled = false;
	}
}