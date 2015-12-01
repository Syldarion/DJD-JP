using UnityEngine;
using System.Text.RegularExpressions;

public class ChatFilter : MonoBehaviour
{
    public TextAsset PirateFilterList;

    public string PirateFilter(string s)
    {
        foreach (string word in PirateFilterList.text.Split(','))
            s = Regex.Replace(s, word.Split(':')[0], word.Split(':')[1], RegexOptions.IgnoreCase);

        return s;
    }
}
