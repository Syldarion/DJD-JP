using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class PlayerScript : NetworkBehaviour
{
    public static PlayerScript MyPlayer;
    public CanvasGroup OpenUI;

    [SyncVar(hook = "OnNameChanged")]
    public string Name;

    public Nation Nationality;
    public int TotalGold;
    public int TotalShips;
    public int TotalCrew;
    public List<Fleet> Fleets;

    //english, spanish, dutch, french
    public int[] Reputation;

    public GameObject FleetPrefab;

    public Port SpawnPort;
    public Fleet ActiveFleet;

    public int NewFleetID;

    [SyncVar]
    public bool ReadyForNextTurn;

    public override void OnStartServer()
    {
        base.OnStartServer();

        ReadyForNextTurn = false;
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
            ActiveFleet.CmdSpawnShip(string.Format("{0}Ship{1}", ActiveFleet.Name, ++ActiveFleet.NewShipID));
        if (Input.GetKeyDown(KeyCode.C) && ActiveFleet)
        {
            CargoManager.Instance.PopulateShipList(ActiveFleet);
            CargoManager.Instance.OpenCargoManager();
        }

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.F))
            FeedbackManager.Instance.OpenFeedback();
    }

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

    [ClientRpc]
    void RpcUpdateFleet(GameObject fleet, GameObject parent, Vector3 local_pos)
    {
        fleet.transform.SetParent(parent.transform);
        fleet.transform.localPosition = local_pos;
    }

    //new_nation should actually be set to a nation that already exists in game
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

    public void ModifyReputation(Nationality nation, int modifier)
    {
        Reputation[(int)nation] = Mathf.Clamp(Reputation[(int)nation] + modifier, 0, 100);
    }

    void OnNameChanged(string new_name)
    {
        name = new_name;

        if (isLocalPlayer)
            FindObjectOfType<PlayerInfoManager>().UpdateCaptainName(new_name);
    }

    [Command]
    public void CmdReadyForNextTurn()
    {
        ReadyForNextTurn = true;

        TurnManager.Instance.CmdCheckReadyForNextTurn();
    }

    [Command]
    public void CmdNotReadyForNextTurn()
    {
        ReadyForNextTurn = false;

        RpcSetEndTurnText();
    }

    [ClientRpc]
    public void RpcSetEndTurnText()
    {
        if(this == MyPlayer)
            TurnManager.Instance.ActionButtonText.text = "END TURN";
    }
}