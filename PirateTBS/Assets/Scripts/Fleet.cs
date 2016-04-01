using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class Fleet : NetworkBehaviour
{
    PlayerScript OwningPlayer;

    [SyncVar(hook = "OnNameChanged")]
    public string Name;
    public List<Ship> Ships;            //Ships in the fleet
    [SyncVar]
    public int FleetSpeed;              //Current speed of the fleet
    public HexTile CurrentPosition;     //HexTile that the fleet is a child of
    public GameObject ShipPrefab;
    public int NewShipID = 0;

    [SyncVar]
    public bool MoveActionTaken;
    [SyncVar]
    public bool CombatActionTaken;

    public List<WaterHex> MovementQueue;

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
        MoveActionTaken = false;
        CombatActionTaken = false;

        MovementQueue = new List<WaterHex>();
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        Ships = new List<Ship>();
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();

        Camera.main.GetComponent<PanCamera>().CenterOnTarget(this.transform);
        if (!PlayerScript.MyPlayer.Fleets.Contains(this))
            PlayerScript.MyPlayer.Fleets.Add(this);

        OwningPlayer = PlayerScript.MyPlayer;
    }

    /// <summary>
    /// Debug function to spawn a ship in this fleet
    /// </summary>
    /// <param name="name">Name for the new ship</param>
    [Command]
    public void CmdSpawnShip(string name)
    {
        Ship new_ship = Instantiate(ShipPrefab).GetComponent<Ship>();
        new_ship.Name = name;
        new_ship.SetClass((ShipClass)Random.Range(0, 8));
        new_ship.Cargo = new Cargo(50, 500);

        NetworkServer.SpawnWithClientAuthority(new_ship.gameObject, PlayerScript.MyPlayer.gameObject);

        CmdAddShip(new_ship.name);
    }

    [Command]
    public void CmdAddShip(string ship_name)
    {
        Ship ship = GameObject.Find(ship_name).GetComponent<Ship>();

        if (!ship)
            return;

        if (!Ships.Contains(ship))
            Ships.Add(ship);
        else
            return;

        UpdateFleetSpeed();
        ship.transform.SetParent(this.transform, false);
        RpcAddShipOthers(ship.Name);   
    }
    
    [ClientRpc]
    void RpcAddShipOthers(string ship_name)
    {
        Ship ship = GameObject.Find(ship_name).GetComponent<Ship>();

        if (!ship)
            return;

        if (!Ships.Contains(ship))
            Ships.Add(ship);
        else
            return;

        ship.transform.SetParent(this.transform, false);
    }
    
    [Command]
    public void CmdRemoveShip(string ship_name)
    {
        Ship ship = GameObject.Find(ship_name).GetComponent<Ship>();

        if (Ships.Contains(ship))
        {
            Ships.Remove(ship);
            UpdateFleetSpeed();
            if (Ships.Count <= 0)
            {
                if (OwningPlayer)
                    OwningPlayer.Fleets.Remove(this);
                NetworkServer.Destroy(this.gameObject);
            }
        }
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

        RpcUpdateFogRange();
    }

    [ClientRpc]
    void RpcUpdateFogRange()
    {
        GetComponent<SphereCollider>().radius = FleetSpeed * 2.1f;
    }

    [Command]
    public void CmdSpawnOnTile(int x, int y)
    {
        HexTile new_tile = GameObject.Find(string.Format("Grid/{0},{1}", x, y)).GetComponent<HexTile>();

        transform.SetParent(new_tile.transform, false);
        transform.localPosition = new Vector3(0.0f, 0.25f, 0.0f);
        CurrentPosition = new_tile;

        RpcSpawnOnTile(x, y);
    }

    [ClientRpc]
    public void RpcSpawnOnTile(int x, int y)
    {
        HexTile new_tile = GameObject.Find(string.Format("Grid/{0},{1}", x, y)).GetComponent<HexTile>();

        transform.SetParent(new_tile.transform, false);
        transform.localPosition = new Vector3(0.0f, 0.25f, 0.0f);
        CurrentPosition = new_tile;
    }

    [Command]
    public void CmdQueueMove(int x, int y)
    {
        WaterHex new_tile = GameObject.Find(string.Format("Grid/{0},{1}", x, y)).GetComponent<WaterHex>();
        MovementQueue.Add(new_tile);
    }

    [Command]
    public void CmdMoveFleet()
    {
        if (MoveActionTaken)
            return;

        transform.SetParent(MovementQueue[MovementQueue.Count - 1].transform, true);
        CurrentPosition = MovementQueue[MovementQueue.Count - 1];

        RpcMoveFleet(MovementQueue[MovementQueue.Count - 1].HexCoord.Q, MovementQueue[MovementQueue.Count - 1].HexCoord.R);

        StopAllCoroutines();
        StartCoroutine(SmoothMove());

        MoveActionTaken = true;
    }

    public IEnumerator SmoothMove()
    {
        foreach (HexTile dest_tile in MovementQueue)
        {
            Vector3 destination = dest_tile.transform.position + new Vector3(0.0f, 0.25f, 0.0f);

            Vector3 direction = (destination - transform.position) / 20.0f;

            for(int i = 0; i < 20; i++)
            {
                transform.Translate(direction);
                yield return new WaitForSeconds(0.01f);
            }

            transform.position = destination;

            yield return null;
        }

        transform.localPosition = new Vector3(0.0f, 0.25f, 0.0f);
    }

    [ClientRpc]
    public void RpcMoveFleet(int x, int y)
    {
        HexTile new_tile = GameObject.Find(string.Format("Grid/{0},{1}", x, y)).GetComponent<HexTile>();

        transform.SetParent(new_tile.transform, true);
        CurrentPosition = new_tile;
    }

    void OnTriggerStay(Collider other)
    {
        WaterHex water_hex = other.GetComponent<WaterHex>();
        LandHex land_hex = other.GetComponent<LandHex>();

        if (water_hex && water_hex.Fog)
            water_hex.RevealTile();

        if(land_hex && !land_hex.Discovered)
            land_hex.DiscoverTile();
    }

    void OnTriggerExit(Collider other)
    {
        WaterHex water_hex = other.GetComponent<WaterHex>();

        if (water_hex && !water_hex.Fog)
            water_hex.HideTile();
    }

    void OnMouseDown()
    {
        GameObject.Find("MovementManager").GetComponent<MovementManager>().SelectFleet(this);
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
