using System;
using System.Collections;
using System.Collections.Generic;

//Our language class contains a list of all its available countries
//It stores its name within a string
[Serializable]
public class Language
{
	public string mName;
	public List<Country> countries;
	
	//Constructor
	public Language(string name)
	{
		mName = name;
		countries = new List<Country>();
	}
	
	//Add a new country
	public void addCountry(string name)
	{
		Country co = new Country(name);
		countries.Add(co);
	}
	
	//Remove a country from countries
	public void removeCountry(int index)
	{
		countries.RemoveAt(index);
	}
}
