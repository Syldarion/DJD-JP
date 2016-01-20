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

    public GameObject ShipPrefab;

    PortScript SpawnPort;

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
            SpawnShip();
        else
            Networking.PrimarySocket.connected += SpawnShip;
    }

    void Update()
    {

	}

    void SpawnShip()
    {
        Networking.PrimarySocket.connected -= SpawnShip;
        Networking.Instantiate(ShipPrefab, NetworkReceivers.AllBuffered, callback: OnShipSpawn);
    }

    void OnShipSpawn(SimpleNetworkedMonoBehavior new_ship)
    {
        MoveShip(new_ship.GetComponent<ShipScript>(), SpawnPort.SpawnTile);

        Fleets.Add(new FleetScript());
        Fleets[Fleets.Count - 1].Ships.Add(new_ship.GetComponent<ShipScript>());
    }

    void MoveShip(ShipScript ship, HexTile new_hex)
    {
        ship.transform.SetParent(new_hex.transform, false);
        ship.transform.localPosition = new Vector3(0.0f, 0.25f, 0.0f);
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