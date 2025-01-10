using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{
    private static System.Random random = new System.Random();

    public static void Shuffle<T>(T[] array)
    {
        for (int i = 0; i < array.Length - 1; ++i)
        {
            int r = random.Next(i, array.Length);
            (array[r], array[i]) = (array[i], array[r]);
        }
    }

	public static string GetDateAsString(DateTime d)
    {
		return d.ToString("MM/dd/yyyy");
    }

	public static string ConvertToJsonString(object data, bool addQuoteEscapes = false)
	{
		string jsonString = "";

		if (data is IDictionary)
		{
			string dictionaryItems = "";

			foreach (DictionaryEntry item in (data as IDictionary))
			{
				if (!string.IsNullOrEmpty(dictionaryItems))
				{
					dictionaryItems += ",";
				}

				if (addQuoteEscapes)
				{
					dictionaryItems += string.Format("\\\"{0}\\\":{1}", item.Key, ConvertToJsonString(item.Value, addQuoteEscapes));
				}
				else
				{
					dictionaryItems += string.Format("\"{0}\":{1}", item.Key, ConvertToJsonString(item.Value, addQuoteEscapes));
				}
			}

			jsonString += "{" + dictionaryItems + "}";
		}
		else if (data is IList)
		{
			IList list = data as IList;

			jsonString += "[";

			for (int i = 0; i < list.Count; i++)
			{
				if (i != 0)
				{
					jsonString += ",";
				}

				jsonString += ConvertToJsonString(list[i], addQuoteEscapes);
			}

			jsonString += "]";
		}
		else if (data is string || data is char)
		{
			// If the data is a string then we need to inclose it in quotation marks
			if (addQuoteEscapes)
			{
				jsonString += "\\\"" + data + "\\\"";
			}
			else
			{
				jsonString += "\"" + data + "\"";
			}
		}
		else if (data is bool)
		{
			jsonString += (bool)data ? "true" : "false";
		}
		else
		{
			// Else just return what ever data is as a string
			jsonString += data.ToString();
		}

		return jsonString;
	}
}
