using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Fleet : NetworkBehaviour
{
    PlayerScript OwningPlayer;

    [SyncVar(hook = "OnNameChanged")]
    public string Name;                     //Name of fleet
    public List<Ship> Ships;                //Ships in fleet
    [SyncVar]
    public int FleetSpeed;                  //Current fleet speed
    public HexTile CurrentPosition;         //HexTile fleet is on
    public GameObject ShipPrefab;           //Ship prefab for spawning new ship in fleet
    public int NewShipID = 0;               //Dev variable for making sure all new ships have a unique name

    [SyncVar]
    public bool MoveActionTaken;            //Tracks if this fleet has moved this turn
    [SyncVar]
    public bool CombatActionTaken;          //Tracks if this fleet has been in combat this turn

    public List<WaterHex> MovementQueue;    //List of tiles fleet needs to move to

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
    /// Debug server-side function to spawn a ship in fleet
    /// </summary>
    /// <param name="name">Name for the new ship</param>
    [Command]
    public void CmdSpawnShip()
    {
        string ship_name = NameGenerator.Instance.GetShipName();

        Ship new_ship = Instantiate(ShipPrefab).GetComponent<Ship>();
        new_ship.Name = ship_name;
        new_ship.SetClass((ShipClass)Random.Range(0, 8));
        new_ship.Cargo = new Cargo(50, 500);

        NetworkServer.SpawnWithClientAuthority(new_ship.gameObject, PlayerScript.MyPlayer.gameObject);

        CmdAddShip(new_ship.name);
    }

    /// <summary>
    /// Server-side command to add existing ship to fleet
    /// </summary>
    /// <param name="ship_name">Name of ship to find</param>
    [Command]
    public void CmdAddShip(string ship_name)
    {
        Ship ship = GameObject.Find(ship_name).GetComponent<Ship>();

        if (!ship || Ships.Count >= 8)
            return;

        if (!Ships.Contains(ship))
            Ships.Add(ship);
        else
            return;

        UpdateFleetSpeed();
        ship.transform.SetParent(this.transform, false);
        RpcAddShipOthers(ship.Name);   
    }
    
    /// <summary>
    /// Client-side command to add existing ship to fleet
    /// </summary>
    /// <param name="ship_name">Name of ship to find</param>
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
    
    /// <summary>
    /// Server-side command to remove existing ship from fleet
    /// </summary>
    /// <param name="ship_name">Name of ship to find</param>
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

    /// <summary>
    /// Server-side function to update fleet speed
    /// </summary>
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

    /// <summary>
    /// Client-side command to update fog range
    /// </summary>
    [ClientRpc]
    void RpcUpdateFogRange()
    {
        GetComponent<SphereCollider>().radius = FleetSpeed * 2.1f;
    }

    /// <summary>
    /// Server-side command to place newly spawned fleet
    /// </summary>
    /// <param name="x">Q-coordinate of tile to spawn on</param>
    /// <param name="y">R-coordinate of tile to spawn on</param>
    [Command]
    public void CmdSpawnOnTile(int x, int y)
    {
        HexTile new_tile = GameObject.Find(string.Format("Grid/{0},{1}", x, y)).GetComponent<HexTile>();

        transform.SetParent(new_tile.transform, false);
        transform.localPosition = new Vector3(0.0f, 0.25f, 0.0f);
        CurrentPosition = new_tile;

        RpcSpawnOnTile(x, y);
    }

    /// <summary>
    /// Client-side command to place newly spawned fleet
    /// </summary>
    /// <param name="x">Q-coordinate of tile to spawn on</param>
    /// <param name="y">R-coordinate of tile to spawn on</param>
    [ClientRpc]
    public void RpcSpawnOnTile(int x, int y)
    {
        HexTile new_tile = GameObject.Find(string.Format("Grid/{0},{1}", x, y)).GetComponent<HexTile>();

        transform.SetParent(new_tile.transform, false);
        transform.localPosition = new Vector3(0.0f, 0.25f, 0.0f);
        CurrentPosition = new_tile;
    }

    /// <summary>
    /// Server-side command to add tile to movement queue
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    [Command]
    public void CmdQueueMove(int x, int y)
    {
        WaterHex new_tile = GameObject.Find(string.Format("Grid/{0},{1}", x, y)).GetComponent<WaterHex>();
        if (new_tile)
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

        MovementQueue.Clear();
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
        MovementManager.Instance.SelectFleet(this);
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
