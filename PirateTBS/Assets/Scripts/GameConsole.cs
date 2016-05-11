using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameConsole : MonoBehaviour
{
    [HideInInspector]
    public static GameConsole Instance;

    Dictionary<string, Action<string>> Commands;            //Dictionary of commands
    Dictionary<string, string> HelpText;                    //Dictionary of help text for commands

    public GameObject LogItemPrefab;                        //Reference to prefab for instantiating log items
    public GameObject ContentPanel;                         //Reference to panel container for log items

    string base_input;                                      //Base input from user

	void Start()
	{
        Instance = this;

        Commands = new Dictionary<string, Action<string>>();
        HelpText = new Dictionary<string, string>();

        RegisterCommand("Help", Help);
        RegisterCommand("ListFleets", ListFleets);
        RegisterCommand("ListShips", ListShips);
        RegisterCommand("ListPorts", ListPorts);
        RegisterCommand("ListInventory", ListInventory);
        RegisterCommand("ModifyStat", ModifyStat);

        HelpText.Add("Help", "Usage: Help <command>");
        HelpText.Add("ListFleets", "Usage: ListFleets");
        HelpText.Add("ListShips", "Usage: ListShips <fleet>");
        HelpText.Add("ListPorts", "Usage: ListPorts");
        HelpText.Add("ListInventory", "Usage: ListInventory <container>");
        HelpText.Add("ModifyStat", "Usage: ModifyStat <object_type>,<object_name>,<modification_string>");
	}
	
	void Update()
	{
        if(Input.GetKeyDown(KeyCode.BackQuote))
        {
            StopAllCoroutines();

            if (PlayerScript.MyPlayer.OpenUI)
                StartCoroutine(CloseConsole());
            else
                StartCoroutine(OpenConsole());
        }
	}

    /// <summary>
    /// Opens console from bottom of screen
    /// </summary>
    /// <returns></returns>
    IEnumerator OpenConsole()
    {
        PlayerScript.MyPlayer.OpenUI = GetComponent<CanvasGroup>();
        RectTransform console_rect = GetComponent<RectTransform>();

        PanelUtilities.ActivatePanel(GetComponent<CanvasGroup>());

        while (console_rect.sizeDelta.y < 600)
        {
            console_rect.sizeDelta = new Vector2(console_rect.sizeDelta.x, console_rect.sizeDelta.y + 20);
            yield return null;
        }
    }

    /// <summary>
    /// Retracts console into bottom of screen
    /// </summary>
    /// <returns></returns>
    IEnumerator CloseConsole()
    {
        PlayerScript.MyPlayer.OpenUI = GetComponent<CanvasGroup>();
        RectTransform console_rect = GetComponent<RectTransform>();

        while (console_rect.sizeDelta.y > 0)
        {
            console_rect.sizeDelta = new Vector2(console_rect.sizeDelta.x, console_rect.sizeDelta.y - 20);
            yield return null;
        }

        PanelUtilities.DeactivatePanel(GetComponent<CanvasGroup>());
    }

    /// <summary>
    /// Register a new command with the console
    /// </summary>
    /// <param name="command">String to activate command</param>
    /// <param name="action">Action, taking string param, to execute when command is entered</param>
    public void RegisterCommand(string command, Action<string> action)
    {
        if (Commands.ContainsKey(command))
            Commands[command] = action;
        else
            Commands.Add(command, action);
    }

    /// <summary>
    /// Parse user input to determine what to execute
    /// </summary>
    /// <param name="input">User input</param>
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

    /// <summary>
    /// Add generic log message to console
    /// </summary>
    /// <param name="message">Message to display</param>
    public void GenericLog(string message)
    {
        GameObject new_log_item = Instantiate(LogItemPrefab);
        new_log_item.GetComponentInChildren<Text>().text = message;
        new_log_item.transform.SetParent(ContentPanel.transform, false);

        if (ContentPanel.transform.childCount > 20)
            Destroy(ContentPanel.transform.GetChild(0).gameObject);
    }

    /// <summary>
    /// Add log message to console, prepended by the related command being executed
    /// </summary>
    /// <param name="message">Message to display</param>
    void AddToLog(string message)
    {
        GameObject new_log_item = Instantiate(LogItemPrefab);
        new_log_item.GetComponentInChildren<Text>().text = string.Format("{0}\n\t{1}", base_input, message);
        new_log_item.transform.SetParent(ContentPanel.transform, false);

        if (ContentPanel.transform.childCount > 20)
            Destroy(ContentPanel.transform.GetChild(0).gameObject);
    }

    /// <summary>
    /// Display help text for given command
    /// </summary>
    /// <param name="input">Command name</param>
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

    /// <summary>
    /// Command to list all fleets in-game
    /// </summary>
    /// <param name="input">User input, not used</param>
    void ListFleets(string input)
    {
        Fleet[] fleets = FindObjectsOfType<Fleet>();

        string fleet_str = string.Empty;
        foreach (Fleet f in fleets)
            fleet_str += string.Format("{0}\n\t", f.name);

        AddToLog(fleet_str);
    }

    /// <summary>
    /// List all ships in a fleet
    /// </summary>
    /// <param name="input">User input containing fleet name</param>
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

    /// <summary>
    /// List all ports in game
    /// </summary>
    /// <param name="input">User input, not used</param>
    void ListPorts(string input)
    {
        Port[] fleets = FindObjectsOfType<Port>();

        string port_str = string.Empty;
        foreach (Port p in fleets)
            port_str += string.Format("{0}\n\t", p.name);

        AddToLog(port_str);
    }

    /// <summary>
    /// List inventory of a given container
    /// </summary>
    /// <param name="input">User input, containing container type and container name</param>
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
            port_inventory_str += string.Format("Goods - {0}\n\t", port.Market.Goods);
            port_inventory_str += string.Format("Sugar - {0}\n\t", port.Market.Sugar);
            port_inventory_str += string.Format("Spice - {0}\n\t", port.Market.Spice);
            port_inventory_str += string.Format("Luxuries - {0}\n\t", port.Market.Luxuries);

            AddToLog(port_inventory_str);
        }
        else
            AddToLog(string.Format("{0} - Invalid cargo container (arg 1)", tokens[0].ToLower()));
    }

    /// <summary>
    /// Modifies the stats of a given object
    /// </summary>
    /// <param name="input">User input, containing object type [player, ship, fleet], object name, and modification string</param>
    void ModifyStat(string input)
    {
        string[] args = input.Split(',');

        switch(args[0])
        {
            case "Ship":
                Ship ship = GameObject.Find(args[1]).GetComponent<Ship>();
                if(!ship)
                {
                    AddToLog("Ship not found");
                    break;
                }
                ship.CmdUpdateStat(args[2]);
                break;
            case "Fleet":
                Fleet fleet = GameObject.Find(args[1]).GetComponent<Fleet>();
                if(!fleet)
                {
                    AddToLog("Fleet not found");
                    break;
                }
                fleet.CmdUpdateStat(args[2]);
                break;
            case "Player":
                PlayerScript player = GameObject.Find(args[1]).GetComponent<PlayerScript>();
                if(!player)
                {
                    AddToLog("Player not found");
                    break;
                }
                player.CmdUpdateStat(args[2]);
                break;
            default:
                AddToLog("Invalid object type");
                break;
        }
    }
}