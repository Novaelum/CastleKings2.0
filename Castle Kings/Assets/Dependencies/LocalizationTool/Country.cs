using System;
using System.Collections;
using System.Collections.Generic;

//Our Country class contains a list of all its available entries
//It stores its name within a string
[Serializable]
public class Country
{
	public string mName;
	public List<IDEntry> entries;
	
	//Constructor, call new entry for every entries available
	public Country(string name)
	{
		mName = name;
		entries = new List<IDEntry> ();
		foreach (string id in LocalizationWindow.ids) 
		{
			newEntry(id);
		}
	}
	
	//Add a new entry within entries a default value of none
	public void newEntry(string id)
	{
		entries.Add(new IDEntry("None"));
	}
	
	//Remove an entry from entries
	public void removeEntry(int index)
	{
		entries.RemoveAt (index);
	}
}

