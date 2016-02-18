using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class Fleet : NetworkBehaviour
{
    [SyncVar(hook = "OnNameChanged")]
    public string Name;
    public List<Ship> Ships;            //Ships in the fleet
    [SyncVar]
    public int FleetSpeed;              //Current speed of the fleet
    public HexTile CurrentPosition;     //HexTile that the fleet is a child of
    public GameObject ShipPrefab;
    public static int NewShipID = 0;

	void Start()
    {

	}

	void Update()
    {
	}

    public override void OnStartServer()
    {
        base.OnStartServer();

        FleetSpeed = 5;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        Ships = new List<Ship>();
    }

    [Command]
    public void CmdSpawnShip(string name)
    {
        Ship new_ship = Instantiate(ShipPrefab).GetComponent<Ship>();
        new_ship.Name = name;
        new_ship.SetClass((ShipClass)Random.Range(0, 8));
        new_ship.Cargo = new Cargo(50, 500);

        NetworkServer.SpawnWithClientAuthority(new_ship.gameObject, connectionToClient);

        AddShip(new_ship);
    }

    [Server]
    public void AddShip(Ship ship)
    {
        if (!Ships.Contains(ship))
            Ships.Add(ship);
        UpdateFleetSpeed();
        RpcAddShipOthers(ship.Name);   
    }
    
    [ClientRpc]
    void RpcAddShipOthers(string ship_name)
    {
        Ship ship = GameObject.Find(ship_name).GetComponent<Ship>();
        if (!Ships.Contains(ship))
            Ships.Add(ship);
    }
    
    [Command]
    public void CmdRemoveShip(string ship_name)
    {
        Ship ship = GameObject.Find(ship_name).GetComponent<Ship>();

        if (Ships.Contains(ship))
        {
            Ships.Remove(ship);
            Network.Destroy(ship.gameObject);
        }
        UpdateFleetSpeed();
        if (Ships.Count <= 0)
            Network.Destroy(this.gameObject);
    }

    [Server]
    public void UpdateFleetSpeed()
    {
        FleetSpeed = 5;                             //Fastest fleet speed possible
        foreach (Ship s in Ships)
        {
            if (s.Speed < FleetSpeed)
                FleetSpeed = s.Speed;
        }
    }
    
    [Command]
    public void CmdMoveFleet(int x, int y)
    {
        HexTile new_tile = GameObject.Find(string.Format("Grid/{0},{1}", x, y)).GetComponent<HexTile>();

        if (HexGrid.MovementHex(CurrentPosition, FleetSpeed).Contains(new_tile))
        {
            transform.SetParent(new_tile.transform, false);
            transform.localPosition = new Vector3(0.0f, 0.25f, 0.0f);
            CurrentPosition = new_tile; 
        }
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

    void OnNameChanged(string new_name)
    {
        name = new_name;
    }
}
