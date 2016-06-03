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
    public int TotalGold;                           //Total gold across all ships
    public int TotalShips;                          //Total ships across all ships
    public int TotalCrew;                           //Total crew across all ships
    public List<Ship> Ships;

    //english, spanish, dutch, french
    public int[] Reputation;                        //Array of reputation values with each nation

    public GameObject ShipPrefab;                  //Prefab for instantiating ships

    public Port SpawnPort;                          //Reference to port to new ship at
    public Ship ActiveShip;                       //Player's currently active ship

    public int NewShipID;                          //Dev variable for creating unique names for new ships

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

        StartCoroutine(WaitForGrid());
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        this.name = Name;

        TotalGold = 0;
        TotalShips = 0;
        TotalCrew = 0;
        Ships = new List<Ship>();
        Reputation = new int[4] { 50, 50, 50, 50 };

        NewShipID = 0;
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (ActiveShip != null)
                ActiveShip = null;
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
            CmdSpawnShip(string.Format("{0}Ship{1}", Name, ++NewShipID), SpawnPort.SpawnTile.HexCoord.Q, SpawnPort.SpawnTile.HexCoord.R, 1000);
        }

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.F))
            FeedbackManager.Instance.OpenFeedback();

        if(Input.GetKeyDown(KeyCode.T))
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
    /// Server-side command to spawn new ship
    /// </summary>
    /// <param name="ship_name">Name of new ship</param>
    /// <param name="x">Q coordinate of tile to spawn ship on</param>
    /// <param name="y">R coordinate of tile to spawn ship on</param>
    [Command]
    public void CmdSpawnShip(string ship_name, int x, int y, int gold)
    {
        Ship new_ship = Instantiate(ShipPrefab).GetComponent<Ship>();
        new_ship.Name = ship_name;
        new_ship.SetClass((ShipClass)Random.Range(0, 8));

        new_ship.Gold = gold;

        HexTile new_tile = GameObject.Find(string.Format("Grid/{0},{1}", x, y)).GetComponent<HexTile>();
        
        NetworkServer.SpawnWithClientAuthority(new_ship.gameObject, gameObject);
        
        new_ship.CmdSpawnOnTile(new_tile.HexCoord.Q, new_tile.HexCoord.R);
    }

    public void AddShip(Ship new_ship)
    {
        Ships.Add(new_ship);
        PlayerInfoManager.Instance.AddShipToList(new_ship);
        PlayerInfoManager.Instance.UpdateAllStats();
    }

    public void RemoveShip(Ship ship_to_remove)
    {
        Ships.Remove(ship_to_remove);
        PlayerInfoManager.Instance.RemoveShipFromList(ship_to_remove);
        PlayerInfoManager.Instance.UpdateAllStats();
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

    IEnumerator WaitForGrid()
    {
        while (!HexGrid.Instance)
            yield return null;
        while (!HexGrid.Instance.DoneGenerating)
            yield return null;

        SpawnInitialShip();
    }

    void SpawnInitialShip()
    {
        Port[] AllPorts = GameObject.FindObjectsOfType<Port>();
        SpawnPort = AllPorts[Random.Range(0, AllPorts.Length)];
        CmdSpawnShip(NameGenerator.Instance.GetShipName(),
            SpawnPort.SpawnTile.HexCoord.Q,
            SpawnPort.SpawnTile.HexCoord.R,
            1000);
    }
}