using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameConsole : MonoBehaviour
{
    Dictionary<string, Action<string>> Commands;
    Dictionary<string, string> HelpText;

    public GameObject LogItemPrefab;
    public GameObject ContentPanel;

    string base_input;
    public static bool console_open;

	void Start()
	{
        Commands = new Dictionary<string, Action<string>>();
        HelpText = new Dictionary<string, string>();
        console_open = false;

        RegisterCommand("Help", Help);
        RegisterCommand("ListFleets", ListFleets);
        RegisterCommand("ListShips", ListShips);
        RegisterCommand("ListPorts", ListPorts);
        RegisterCommand("ListInventory", ListInventory);

        HelpText.Add("Help", "Usage: Help <command>");
        HelpText.Add("ListFleets", "Usage: ListFleets");
        HelpText.Add("ListShips", "Usage: ListShips <fleet>");
        HelpText.Add("ListPorts", "Usage: ListPorts");
        HelpText.Add("ListInventory", "Usage: ListInventory <container>");
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

    public void RegisterCommand(string command, Action<string> action)
    {
        if (Commands.ContainsKey(command))
            Commands[command] = action;
        else
            Commands.Add(command, action);
    }

    public void ParseInput(string input)
    {
        base_input = input;

        string command = base_input.Split(' ', '\t')[0];

        if(!Commands.ContainsKey(command))
        {
            AddToLog("Command Not Defined");
            return;
        }

        string args = base_input.Substring(base_input.IndexOfAny(new char[] { ' ', '\t' }) + 1);
        Debug.Log(args);

        Commands[command].Invoke(args);
    }

    public void GenericLog(string message)
    {
        GameObject new_log_item = Instantiate(LogItemPrefab);
        new_log_item.GetComponentInChildren<Text>().text = message;
        new_log_item.transform.SetParent(ContentPanel.transform, false);

        if (ContentPanel.transform.childCount > 20)
            Destroy(ContentPanel.transform.GetChild(0).gameObject);
    }

    void AddToLog(string message)
    {
        GameObject new_log_item = Instantiate(LogItemPrefab);
        new_log_item.GetComponentInChildren<Text>().text = string.Format("{0}\n\t{1}", base_input, message);
        new_log_item.transform.SetParent(ContentPanel.transform, false);

        if (ContentPanel.transform.childCount > 20)
            Destroy(ContentPanel.transform.GetChild(0).gameObject);
    }

    void Help(string input)
    {
        if (HelpText.ContainsKey(input))
            AddToLog(HelpText[input]);
        else
        {
            string all_commands = "Commands\n";
            foreach (string key in HelpText.Keys)
                all_commands += string.Format("{0}\n\t", key);
            AddToLog(all_commands);
        }
    }

    void ListFleets(string input)
    {
        Fleet[] fleets = FindObjectsOfType<Fleet>();

        string fleet_str = string.Empty;
        foreach (Fleet f in fleets)
            fleet_str += string.Format("{0}\n\t", f.name);

        AddToLog(fleet_str);
    }

    void ListShips(string input)
    {
        string[] tokens = input.Split(' ', '\t');
        if(tokens.Length < 1)
        {
            AddToLog("Missing fleet arg");
            return;
        }
        Fleet fleet = GameObject.Find(tokens[0]).GetComponent<Fleet>();
        if(!fleet)
        {
            AddToLog("Invalid fleet name (arg 1)");
            return;
        }

        List<Ship> ships = new List<Ship>();

        for (int i = 0; i < fleet.transform.childCount; i++)
            if (fleet.transform.GetChild(i).GetComponent<Ship>())
                ships.Add(fleet.transform.GetChild(i).GetComponent<Ship>());

        string ship_str = string.Empty;
        foreach (Ship s in ships)
            ship_str += string.Format("{0}\n\t", s.name);

        AddToLog(ship_str);
    }

    void ListPorts(string input)
    {
        Port[] fleets = FindObjectsOfType<Port>();

        string port_str = string.Empty;
        foreach (Port p in fleets)
            port_str += string.Format("{0}\n\t", p.name);

        AddToLog(port_str);
    }

    void ListInventory(string input)
    {
        string[] tokens = input.Split(' ', '\t');

        if(tokens[0].ToLower() == "ship")
        {
            Ship ship = GameObject.Find(tokens[1]).GetComponent<Ship>();
            if(!ship)
            {
                AddToLog("Ship not found");
                return;
            }

            string ship_cargo_str = string.Empty;
            ship_cargo_str += string.Format("Food - {0}\n\t", ship.Cargo.Food);
            ship_cargo_str += string.Format("Gold - {0}\n\t", ship.Cargo.Gold);
            ship_cargo_str += string.Format("Goods - {0}\n\t", ship.Cargo.Goods);
            ship_cargo_str += string.Format("Sugar - {0}\n\t", ship.Cargo.Sugar);
            ship_cargo_str += string.Format("Spice - {0}\n\t", ship.Cargo.Spice);
            ship_cargo_str += string.Format("Luxuries - {0}\n\t", ship.Cargo.Luxuries);

            AddToLog(ship_cargo_str);
        }
        else if(tokens[0].ToLower() == "port")
        {
            Port port = GameObject.Find(tokens[1]).GetComponent<Port>();
            if(!port)
            {
                AddToLog("Port not found");
                return;
            }

            string port_inventory_str = string.Empty;
            port_inventory_str += string.Format("Food - {0}\n\t", port.Market.Food);
            port_inventory_str += string.Format("Gold - {0}\n\t", port.Market.Gold);
            port_inventory_str += string.Format("Goods - {0}\n\t", port.Market.Goods);
            port_inventory_str += string.Format("Sugar - {0}\n\t", port.Market.Sugar);
            port_inventory_str += string.Format("Spice - {0}\n\t", port.Market.Spice);
            port_inventory_str += string.Format("Luxuries - {0}\n\t", port.Market.Luxuries);

            AddToLog(port_inventory_str);
        }
        else
            AddToLog(string.Format("{0} - Invalid cargo container (arg 1)", tokens[0].ToLower()));
    }
}