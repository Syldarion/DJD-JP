using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class PlayerScript : NetworkBehaviour
{
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

    void Start()
    {
        TotalGold = 0;
        TotalShips = 0;
        TotalCrew = 0;
        Fleets = new List<Fleet>();
        Reputation = new int[4] { 50, 50, 50, 50 };

        NewFleetID = 0;
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
            CmdSpawnFleet(string.Empty);
        if (Input.GetKeyDown(KeyCode.O))
            ActiveFleet.CmdSpawnShip(string.Empty);
        if (Input.GetKeyDown(KeyCode.C) && ActiveFleet)
        {
            GameObject.Find("CargoManagementPanel").GetComponent<CargoManager>().PopulateShipList(ActiveFleet);
            GameObject.Find("CargoManagementPanel").GetComponent<CargoManager>().OpenCargoManager();
        }
    }

    [Command]
    public void CmdSpawnFleet(string fleet_name)
    {
        Fleet new_fleet = Instantiate(FleetPrefab).GetComponent<Fleet>();
        new_fleet.Name = fleet_name;

        NetworkServer.SpawnWithClientAuthority(new_fleet.gameObject, connectionToClient);


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
}