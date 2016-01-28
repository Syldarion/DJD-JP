using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class ConsoleCommands<T>
{
    public Dictionary<string, T> Commands;

    public ConsoleCommands()
    {
        Commands = new Dictionary<string, T>();
    }

    public void RegisterCommand(string command, T action)
    {
        if (Commands.ContainsKey(command))
            Commands[command] = action;
        else
            Commands.Add(command, action);
    }
}

public class GameConsole : MonoBehaviour
{
    public GameObject LogItemPrefab;
    public GameObject ContentPanel;

    bool console_open;

	void Start()
	{
        console_open = false;
	}
	
	void Update()
	{
        if(Input.GetKeyDown(KeyCode.BackQuote))
        {
            StopAllCoroutines();

            if (console_open)
                StartCoroutine("CloseConsole");
            else
                StartCoroutine("OpenConsole");

            console_open = !console_open;
        }
	}

    IEnumerator OpenConsole()
    {
        RectTransform console_rect = GetComponent<RectTransform>();

        while (console_rect.sizeDelta.y < 600)
        {
            console_rect.sizeDelta = new Vector2(console_rect.sizeDelta.x, console_rect.sizeDelta.y + 20);
            yield return null;
        }
    }

    IEnumerator CloseConsole()
    {
        RectTransform console_rect = GetComponent<RectTransform>();

        while (console_rect.sizeDelta.y > 0)
        {
            console_rect.sizeDelta = new Vector2(console_rect.sizeDelta.x, console_rect.sizeDelta.y - 20);
            yield return null;
        }
    }

    public void ParseInput(string input)
    {
        string[] tokens = input.Split(' ', '\t');
    }

    void AddToLog(string input, string message)
    {
        GameObject new_log_item = Instantiate(LogItemPrefab);
        new_log_item.GetComponentInChildren<Text>().text = string.Format("{0}\n\t{1}", input, message);
        new_log_item.transform.SetParent(ContentPanel.transform, false);

        if (ContentPanel.transform.childCount > 20)
            Destroy(ContentPanel.transform.GetChild(0).gameObject);
    }
}