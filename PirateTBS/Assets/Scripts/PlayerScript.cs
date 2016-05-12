using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class PlayerScript : NetworkBehaviour
{
    public static PlayerScript MyPlayer;            //Reference to local player
    public CanvasGroup OpenUI;                      //Reference to the currently open UI

    [SyncVar(hook = "OnNameChanged")]
    public string Name;                             //Player username

    public Nation Nationality;                      //Player nationality
    public int TotalGold;                           //Total gold across all fleets
    public int TotalShips;                          //Total ships across all fleets
    public int TotalCrew;                           //Total crew across all ships
    public List<Fleet> Fleets;                      //List of fleets controlled by player

    //english, spanish, dutch, french
    public int[] Reputation;                        //Array of reputation values with each nation

    public GameObject FleetPrefab;                  //Prefab for instantiating fleets

    public Port SpawnPort;                          //Reference to port to new fleet at
    public Fleet ActiveFleet;                       //Player's currently active fleet

    public int NewFleetID;                          //Dev variable for creating unique names for new fleets

    [SyncVar]
    public bool ReadyForNextTurn;                   //Flag to see if player is ready for next turn

    [SyncVar]
    public int ResourceCostMod;                     //Percent value for resource cost modifcations
    [SyncVar]
    public int ShipCostMod;                         //Percent value for ship cost modifications
    [SyncVar]
    public int ResourceGenMod;                      //Percent value for resource generation modifications
    [SyncVar]
    public int MoraleMod;                           //Percent value for morale modifications
    [SyncVar]
    public int ReputationMod;                       //Percent value for reputation modifications

    public override void OnStartServer()
    {
        base.OnStartServer();

        ReadyForNextTurn = false;
        ResourceCostMod = 100;
        ShipCostMod = 100;
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();

        MyPlayer = this;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        this.name = Name;

        TotalGold = 0;
        TotalShips = 0;
        TotalCrew = 0;
        Fleets = new List<Fleet>();
        Reputation = new int[4] { 50, 50, 50, 50 };

        NewFleetID = 0;
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (ActiveFleet != null)
                ActiveFleet = null;
            else if(OpenUI)
            {
                PanelUtilities.DeactivatePanel(OpenUI);
                OpenUI = null;
            }
            else
            {
                OpenUI = InGameMenuController.Instance.GetComponent<CanvasGroup>();
                PanelUtilities.ActivatePanel(OpenUI);
            }
        }

        if (OpenUI)
            return;

        if (Input.GetKeyDown(KeyCode.I))
        {
            Port[] AllPorts = GameObject.FindObjectsOfType<Port>();
            SpawnPort = AllPorts[Random.Range(0, AllPorts.Length)];
            CmdSpawnFleet(string.Format("{0}Fleet{1}", Name, ++NewFleetID), SpawnPort.SpawnTile.HexCoord.Q, SpawnPort.SpawnTile.HexCoord.R);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            ActiveFleet.CmdSpawnShip();
        }
        if (Input.GetKeyDown(KeyCode.P) && ActiveFleet)
        {
            CargoManager.Instance.PopulateShipList(ActiveFleet);
            CargoManager.Instance.OpenCargoManager();
        }

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.F))
            FeedbackManager.Instance.OpenFeedback();

        if(Input.GetKey(KeyCode.T))
        {
            OpenUI = TechTree.Instance.GetComponent<CanvasGroup>();
            PanelUtilities.ActivatePanel(OpenUI);
        }
    }

    /// <summary>
    /// Server-side command to update ship stat
    /// </summary>
    /// <param name="modify_string">String representation of modification command</param>
    [Command]
    public void CmdUpdateStat(string modify_string)
    {
        if (modify_string == string.Empty)
            return;

        string[] split = modify_string.Split(' ');

        if (split.Length != 3)
            return;

        string var = split[0];
        string mod_oper = split[1];
        int val = int.Parse(split[2]);

        int current_var_val = (int)GetType().GetField(var).GetValue(this);

        switch (mod_oper)
        {
            case "+":
                GetType().GetField(var).SetValue(this, current_var_val + val);
                break;
            case "-":
                GetType().GetField(var).SetValue(this, current_var_val - val);
                break;
            case "*":
                GetType().GetField(var).SetValue(this, current_var_val * val);
                break;
            case "/":
                GetType().GetField(var).SetValue(this, current_var_val / val);
                break;
            default:
                return;
        }
    }

    /// <summary>
    /// Server-side command to spawn new fleet
    /// </summary>
    /// <param name="fleet_name">Name of new fleet</param>
    /// <param name="x">Q coordinate of tile to spawn fleet on</param>
    /// <param name="y">R coordinate of tile to spawn fleet on</param>
    [Command]
    public void CmdSpawnFleet(string fleet_name, int x, int y)
    {
        Fleet new_fleet = Instantiate(FleetPrefab).GetComponent<Fleet>();
        new_fleet.Name = fleet_name;

        HexTile new_tile = GameObject.Find(string.Format("Grid/{0},{1}", x, y)).GetComponent<HexTile>();

        Fleets.Add(new_fleet);

        NetworkServer.SpawnWithClientAuthority(new_fleet.gameObject, gameObject);
        
        new_fleet.CmdSpawnOnTile(new_tile.HexCoord.Q, new_tile.HexCoord.R);
    }

    /// <summary>
    /// Change player's nationality
    /// </summary>
    /// <param name="new_nation">Nation to switch to</param>
    public void ChangeNationality(Nation new_nation)
    {
        //make sure you aren't trying to join your own nation and that you aren't trying to join a nation that your current nation is at war with
        //maybe you should be able to join nations at war, but for now, whatever
        if (new_nation != Nationality && !Nationality.Enemies.Contains(new_nation))
        {
            Nationality = new_nation;
            foreach (Nation n in new_nation.Allies)
                ModifyReputation(n.Name, 10);
            foreach (Nation n in new_nation.Enemies)
                ModifyReputation(n.Name, -10);
        }
    }

    /// <summary>
    /// Change reputation with a nation
    /// </summary>
    /// <param name="nation">Nation to modify</param>
    /// <param name="modifier">Amount to change reputation by</param>
    public void ModifyReputation(Nationality nation, int modifier)
    {
        Reputation[(int)nation] = Mathf.Clamp(Reputation[(int)nation] + modifier, 0, 100);
    }

    /// <summary>
    /// Callback when player name is changed
    /// </summary>
    /// <param name="new_name">New player name</param>
    void OnNameChanged(string new_name)
    {
        name = new_name;

        if (isLocalPlayer)
            FindObjectOfType<PlayerInfoManager>().UpdateCaptainName(new_name);
    }

    /// <summary>
    /// Server-side command to mark player ready for next turn
    /// </summary>
    [Command]
    public void CmdReadyForNextTurn()
    {
        ReadyForNextTurn = true;

        TurnManager.Instance.CmdCheckReadyForNextTurn();
    }

    /// <summary>
    /// Server-side command to mark player not ready for next turn
    /// </summary>
    [Command]
    public void CmdNotReadyForNextTurn()
    {
        ReadyForNextTurn = false;

        RpcSetEndTurnText("END TURN");
    }

    /// <summary>
    /// Client-side command to set text of end turn button
    /// </summary>
    /// <param name="text">Button text</param>
    [ClientRpc]
    public void RpcSetEndTurnText(string text)
    {
        if(this == MyPlayer)
            TurnManager.Instance.ActionButtonText.text = text;
    }
}