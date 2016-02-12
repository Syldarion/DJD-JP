using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Network;

public class PlayerScript : NetworkedMonoBehavior
{
    [NetSync("OnNameChanged", NetworkCallers.Everyone)]
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

    void Start()
    {
        TotalGold = 0;
        TotalShips = 0;
        TotalCrew = 0;
        Fleets = new List<Fleet>();
        Reputation = new int[4] { 50, 50, 50, 50 };

        NewFleetID = 0;

        StartCoroutine("WaitForPortList");
    }

    protected override void NetworkStart()
    {
        base.NetworkStart();

        if (IsOwner)
            FindObjectOfType<PlayerInfoManager>().SetOwningPlayer(this);
    }

    public void Initialize()
    {
        Port[] ports = HexGrid.ports.ToArray();
        SpawnPort = ports[Random.Range(0, ports.Length - 1)];

        if (Networking.PrimarySocket.Connected)
            SpawnFleet();
        else
            Networking.PrimarySocket.connected += SpawnFleet;
    }

    void Update()
    {
        if (GameConsole.console_open)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (ActiveFleet != null)
                ActiveFleet = null;
            else
            {
                //Open menu
            }
        }
        if (Input.GetKeyDown(KeyCode.I))
            Initialize();
        if(Input.GetKeyDown(KeyCode.O))
            Networking.Instantiate(ActiveFleet.ShipPrefab, NetworkReceivers.All, callback: ActiveFleet.OnShipCreated);
        if (Input.GetKeyDown(KeyCode.C) && ActiveFleet)
        {
            GameObject.Find("CargoManagementPanel").GetComponent<CargoManager>().PopulateShipList(ActiveFleet);
            GameObject.Find("CargoManagementPanel").GetComponent<CargoManager>().OpenCargoManager();
        }
    }

    void SpawnFleet()
    {
        Networking.PrimarySocket.connected -= SpawnFleet;
        if (IsOwner)
            Networking.Instantiate(FleetPrefab, NetworkReceivers.All, callback: OnFleetSpawn);
    }

    void OnFleetSpawn(SimpleNetworkedMonoBehavior new_fleet)
    {
        Fleets.Add(new_fleet.GetComponent<Fleet>());
        string player_name = Networking.PrimarySocket.Me.Name;
        new_fleet.GetComponent<Fleet>().RPC("SpawnFleet", string.Format("{0}Fleet{1}", player_name, (++NewFleetID).ToString()), SpawnPort.SpawnTile.name);
    }

    IEnumerator WaitForPortList()
    {
        while (HexGrid.ports.Count == 0)
            yield return null;
        Initialize();
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

    void OnNameChanged()
    {
        GameObject.Find("ConsolePanel").GetComponent<GameConsole>().GenericLog(Name);
        name = Name;

        if (IsOwner)
            FindObjectOfType<PlayerInfoManager>().UpdateCaptainName(Name);
    }
}