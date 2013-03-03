using UnityEngine;
using System.IO;

public static class Parser
{
	public static byte[,,] ParseFromString(string str)
	{
		RemoveWhiteSpace(ref str);
		RemoveComasAtTheEnd(ref str);
		var length = ReadArraySize(str);
		var x = (int)length.x;
		var y = (int)length.y;
		var z = (int)length.z;
		byte[,,] array = new byte[x,y,z];
		FillArray(array, str);
		return array;
    }
	
	public static byte[,,] ParseFromFile(string path)
	{
		string str;
		using(TextReader reader = new StreamReader(path))
		{
			str = reader.ReadToEnd();
		}
		return ParseFromString(str);
    }
	
	private static void RemoveWhiteSpace(ref string str)
	{
		foreach(char c in str.ToString())
		{
			if(char.IsWhiteSpace(c))
			{
				str = str.Replace(c.ToString(), "");
			}
		}
	}
	
	private static void RemoveComasAtTheEnd(ref string str)
	{
		var s = str.ToString();
		
		for(int i = str.Length - 1; i >= 0; i--)
		{
			if(s[i] == '}' && s[i - 1] == ',')
			{
				str = str.Remove(i - 1, 1);
			}
		}
	}
	
	private static Vector3 ReadArraySize(string str)
	{
		int coordinate = -1;
		Vector3 vect = Vector3.zero;
		Vector3 length = Vector3.zero;
		
		foreach(char token in str)
		{
			switch(token)
			{
				case '{':
					coordinate++;
					break;
				case '}':
					length[coordinate] = vect[coordinate];
					vect[coordinate] = 0;
					coordinate--;
					break;
				case ',':
					vect[coordinate]++;
					break;
			}
		}
		return length + Vector3.one;
	}
	
	private static void FillArray(byte[,,] array, string str)
	{
		int coordinate = -1;
		Vector3 vect = Vector3.zero;
		                          
		foreach(char token in str)
		{
			switch(token)
			{
				case '{':
					coordinate++;
					break;
				case '}':
					vect[coordinate] = 0;
					coordinate--;
					break;
				case ',':
					vect[coordinate]++;
					break;
				default:
					byte number = 0;
					if(char.IsNumber(token))
					{
						number = byte.Parse(token.ToString());
					}
					else if(token == '_')
					{
						number = byte.MaxValue;
					}
					else
						break;
					
					var x = (int)vect.x;
					var y = (int)vect.y;
					var z = (int)vect.z;
					array[x,y,z] = number;
					break;
			}
		}
	}
}
