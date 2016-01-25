using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public struct Function
{
    public object Caller;
    public MethodInfo Method;

    public Function(object caller, MethodInfo method)
    {
        Caller = caller;
        Method = method;
    }
}

public class GameConsole : MonoBehaviour
{
    public GameObject LogItemPrefab;
    public GameObject ContentPanel;

    //no tuple, unity is on .net 3.5
    Dictionary<string, Function> ConsoleMethods;

    bool console_open;

	void Start()
	{
        ConsoleMethods = new Dictionary<string, Function>();
        console_open = false;
	}
	
	void Update()
	{
        if(Input.GetKeyDown(KeyCode.BackQuote))
        {
            if(console_open)
                
        }
	}

    public void ParseInput(string input)
    {
        string[] tokens = input.Split(' ', '\t');
        if (!ConsoleMethods.ContainsKey(tokens[0]))
        {
            AddToLog(string.Format("{0}\n\tUndefined Command", input));
            return;
        }

        int token_index = 1;

        List<object> args = new List<object>();

        foreach(ParameterInfo parameter in ConsoleMethods[tokens[0]].Method.GetParameters())
        {
            token_index++;

            if (parameter.ParameterType == Type.GetType("int"))
            {
                int temp = 0;
                if (int.TryParse(tokens[token_index], out temp))
                    args.Add(temp);
                else
                {
                    AddToLog(string.Format("{0}\n\tExcepted an integer for argument {1}", input, token_index.ToString()));
                    return;
                }
            }
            else if (parameter.ParameterType == Type.GetType("string"))
            {
                args.Add(tokens[token_index]);
            }
            else if (parameter.ParameterType == Type.GetType("bool"))
            {
                if (tokens[token_index] == "true" || tokens[token_index] == "1")
                    args.Add(true);
                else if (tokens[token_index] == "false" || tokens[token_index] == "0")
                    args.Add(false);
                else
                {
                    AddToLog(string.Format("{0}\n\tExcepted a boolean for argument {1}", input, token_index.ToString()));
                    return;
                }
            }
        }

        ConsoleMethods[tokens[0]].Method.Invoke(ConsoleMethods[tokens[0]].Caller, args.ToArray());
        AddToLog(string.Format("{0}\n\tSuccessfully Run", input));
    }

    public void RegisterFunction(string command, object caller, MethodInfo action)
    {
        foreach(ParameterInfo parameter in action.GetParameters())
            if (parameter.GetType() != Type.GetType("int") || parameter.GetType() != Type.GetType("string") || parameter.GetType() != Type.GetType("bool"))
                return;

        if (ConsoleMethods.ContainsKey(command))
            ConsoleMethods[command] = new Function(caller, action);
        else
            ConsoleMethods.Add(command, new Function(caller, action));
    }

    void AddToLog(string message)
    {
        GameObject new_log_item = Instantiate(LogItemPrefab);
        new_log_item.GetComponentInChildren<Text>().text = message;
        new_log_item.transform.SetParent(ContentPanel.transform, false);
    }
}
