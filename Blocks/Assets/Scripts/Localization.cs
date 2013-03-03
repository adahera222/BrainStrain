using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;

public static class Localization
{
	public static SystemLanguage Language
	{
		get
		{
			return _language;
		}
		set
		{
			_language = value;
			ChangeLanguage(_language);
		}
	}
	
	public static string Get(string key)
	{
		return _dictionary[key];	
	}
	
	private static SystemLanguage _language;
	private static Dictionary<string, string> _dictionary;
	
	static Localization()
	{
		var lang = Application.systemLanguage;
		if(lang == SystemLanguage.Bulgarian || lang == SystemLanguage.English)
			Language = lang;
		else
			Language = SystemLanguage.English;
	}
	
	private static void ChangeLanguage(SystemLanguage lang)
	{
		var jsonString = (Resources.Load("Localization/" + lang.ToString()) as TextAsset).text;
		var dict = Json.Deserialize(jsonString) as Dictionary<string,object>;
		_dictionary = new Dictionary<string, string>();
		foreach(var item in dict)
		{
			_dictionary.Add(item.Key, (string)item.Value);
		}
	}
}