using UnityEngine;
using System.Text.RegularExpressions;

public class ChatFilter : MonoBehaviour
{
    public TextAsset ProfFilterList;
    public TextAsset PirateFilterList;

	public string ProfanityFilter(string s)
    {
        foreach (string word in ProfFilterList.text.Split(','))
            s = Regex.Replace(s, word, "****", RegexOptions.IgnoreCase);

        return s;
    }

    public string PirateFilter(string s)
    {
        foreach (string word in PirateFilterList.text.Split(','))
            s = Regex.Replace(s, word.Split(':')[0], word.Split(':')[1], RegexOptions.IgnoreCase);

        return s;
    }
}
