using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Network;

public class Fleet : NetworkedMonoBehavior
{
    [NetSync("OnNameChanged", NetworkCallers.Everyone)]
    public string Name;
    public List<Ship> Ships;            //Ships in the fleet
    public int FleetSpeed;              //Current speed of the fleet
    public HexTile CurrentPosition;     //HexTile that the fleet is a child of
    public GameObject ShipPrefab;
    public static int NewShipID = 0;

	void Start()
    {
        Ships = new List<Ship>();
        FleetSpeed = 5;
	}

	void Update()
    {
	}

    /// <summary>
    /// Add a ship to the fleet
    /// </summary>
    /// <param name="ship">The ship to add to the fleet</param>
    public void AddShip(Ship ship)
    {
        if (!Ships.Contains(ship))
            Ships.Add(ship);
        UpdateFleetSpeed();

        RPC("AddShipOthers", NetworkReceivers.Others, ship.name);
    }
    
    [BRPC]
    void AddShipOthers(string ship_name)
    {
        Ship ship = GameObject.Find(ship_name).GetComponent<Ship>();
        if (!Ships.Contains(ship))
            Ships.Add(ship);
        UpdateFleetSpeed();
    }

    /// <summary>
    /// Remove a ship from the fleet, if it exists
    /// </summary>
    /// <param name="ship">The ship to remove from the fleet</param>
    public void RemoveShip(Ship ship)
    {
        if (Ships.Contains(ship))
            Ships.Remove(ship);
        UpdateFleetSpeed();
        if (Ships.Count <= 0)
            Destroy(this.gameObject);

        RPC("RemoveShipOthers", NetworkReceivers.Others, ship.name);
    }

    [BRPC]
    void RemoveShipOthers(string ship_name)
    {
        Ship ship = transform.FindChild(ship_name).GetComponent<Ship>();
        if (Ships.Contains(ship))
            Ships.Remove(ship);
        UpdateFleetSpeed();
        if (Ships.Count <= 0)
            Destroy(this.gameObject);
    }

    /// <summary>
    /// Update the speed of the fleet, which will be equal to the slowest ship in the fleet
    /// </summary>
    public void UpdateFleetSpeed()
    {
        FleetSpeed = 5;                             //Fastest fleet speed possible
        foreach (Ship s in Ships)
        {
            if (s.Speed < FleetSpeed)
                FleetSpeed = s.Speed;
        }
    }

    /// <summary>
    /// Spawn a new fleet across the network
    /// </summary>
    /// <param name="fleet_name">The name of the new fleet</param>
    /// <param name="initial_tile_name">The name of the initial position tile for the fleet</param>
    [BRPC]
    public void SpawnFleet(string fleet_name, string initial_tile_name)
    {
        Name = fleet_name;

        HexTile initial_tile = GameObject.Find(initial_tile_name).GetComponent<HexTile>();
        transform.SetParent(initial_tile.transform, false);
        transform.localPosition = new Vector3(0.0f, 0.25f, 0.0f);
        CurrentPosition = initial_tile;

        if (IsOwner)
            Networking.Instantiate(ShipPrefab, NetworkReceivers.All, callback: OnShipCreated);
    }

    public void OnShipCreated(SimpleNetworkedMonoBehavior new_ship)
    {
        new_ship.transform.SetParent(transform);
        new_ship.transform.localPosition = Vector3.zero;
        new_ship.RPC("SpawnShip", string.Format("{0}Ship{1}", Networking.PrimarySocket.Me.Name, (++NewShipID).ToString()), this.name);
        AddShip(new_ship.GetComponent<Ship>());
    }

    /// <summary>
    /// Move the fleet
    /// </summary>
    /// <param name="new_tile">The tile to move the fleet to</param>
    /// <returns>If the movement was successful</returns>
    public bool MoveFleet(HexTile new_tile)
    {
        if (HexGrid.MovementHex(CurrentPosition, FleetSpeed).Contains(new_tile))
        {
            transform.SetParent(new_tile.transform, false);
            transform.localPosition = new Vector3(0.0f, 0.25f, 0.0f);
            CurrentPosition = new_tile;

            return true;
        }
        return false;
    }

    void OnMouseEnter()
    {
        Tooltip.EnableTooltip(true);
        Tooltip.UpdateTooltip(name);
    }

    void OnMouseExit()
    {
        Tooltip.EnableTooltip(false);
    }

    void OnNameChanged()
    {
        GameObject.Find("ConsolePanel").GetComponent<GameConsole>().GenericLog(Name);
        name = Name;
    }
}
