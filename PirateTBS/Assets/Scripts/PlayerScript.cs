using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Network;

public class PlayerScript : NetworkedMonoBehavior
{
    public string Name;
    public Nation Nationality { get; private set; }
    public int TotalGold { get; private set; }
    public int TotalShips { get; private set; }
    public int TotalCrew { get; private set; }
    public List<FleetScript> Fleets { get; private set; }

    //english, spanish, dutch, french
    public int[] Reputation { get; private set; }

    public GameObject FleetPrefab;

    public PortScript SpawnPort;
    public FleetScript ActiveFleet;

    void Start()
    {
        TotalGold = 0;
        TotalShips = 0;
        TotalCrew = 0;
        Fleets = new List<FleetScript>();
        Reputation = new int[4]{ 50, 50, 50, 50};

        StartCoroutine("WaitForPortList");
	}

    public void Initialize()
    {
        Debug.Log(GameObject.Find("Grid"));
        PortScript[] ports = GameObject.Find("Grid").GetComponent<HexGrid>().ports.ToArray();
        Debug.Log(ports.Length);
        SpawnPort = ports[Random.Range(0, ports.Length - 1)];

        if (Networking.PrimarySocket.Connected)
            SpawnFleet();
        else
            Networking.PrimarySocket.connected += SpawnFleet;
    }

    void Update()
    {

	}

    void SpawnFleet()
    {
        Networking.PrimarySocket.connected -= SpawnFleet;
        Networking.Instantiate(FleetPrefab, NetworkReceivers.AllBuffered, callback: OnFleetSpawn);
    }

    void OnFleetSpawn(SimpleNetworkedMonoBehavior new_fleet)
    {
        new_fleet.GetComponent<FleetScript>().AddShip(new ShipScript());

        Fleets.Add(new_fleet.GetComponent<FleetScript>());
    }

    IEnumerator WaitForPortList()
    {
        while (GameObject.Find("Grid").GetComponent<HexGrid>().ports.Count == 0)
            yield return null;
        Initialize();
    }

    //new_nation should actually be set to a nation that already exists in game
    public void ChangeNationality(Nation new_nation)
    {
        //make sure you aren't trying to join your own nation and that you aren't trying to join a nation that your current nation is at war with
        //maybe you should be able to join nations at war, but for now, whatever
        if(new_nation != Nationality && !Nationality.Enemies.Contains(new_nation))
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
}