﻿using UnityEngine;
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

    [SyncVar]
    public bool MoveActionTaken;
    [SyncVar]
    public bool CombatActionTaken;

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
        PlayerScript.MyPlayer.Fleets.Add(this);
    }

    [Command]
    public void CmdSpawnShip(string name)
    {
        Ship new_ship = Instantiate(ShipPrefab).GetComponent<Ship>();
        new_ship.Name = name;
        new_ship.SetClass((ShipClass)Random.Range(0, 8));
        new_ship.Cargo = new Cargo(50, 500);

        NetworkServer.SpawnWithClientAuthority(new_ship.gameObject, PlayerScript.MyPlayer.gameObject);

        AddShip(new_ship);
    }

    [Server]
    public void AddShip(Ship ship)
    {
        if (!Ships.Contains(ship))
            Ships.Add(ship);
        UpdateFleetSpeed();
        ship.transform.SetParent(this.transform, false);
        RpcAddShipOthers(ship.Name);   
    }
    
    [ClientRpc]
    void RpcAddShipOthers(string ship_name)
    {
        Ship ship = GameObject.Find(ship_name).GetComponent<Ship>();
        if (!Ships.Contains(ship))
        {
            Ships.Add(ship);
            ship.transform.SetParent(this.transform, false);
        }
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

        RpcUpdateFogRange();
    }

    [ClientRpc]
    void RpcUpdateFogRange()
    {
        GetComponent<SphereCollider>().radius = FleetSpeed * 2;
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
    public void CmdMoveFleet(int x, int y)
    {
        if (MoveActionTaken)
            return;

        HexTile new_tile = GameObject.Find(string.Format("Grid/{0},{1}", x, y)).GetComponent<HexTile>();

        if (HexGrid.MovementHex(CurrentPosition, FleetSpeed).Contains(new_tile))
        {
            transform.SetParent(new_tile.transform, false);
            transform.localPosition = new Vector3(0.0f, 0.25f, 0.0f);
            CurrentPosition = new_tile;

            RpcMoveFleet(x, y);

            MoveActionTaken = true;
        }
    }

    [ClientRpc]
    public void RpcMoveFleet(int x, int y)
    {
        HexTile new_tile = GameObject.Find(string.Format("Grid/{0},{1}", x, y)).GetComponent<HexTile>();

        if (HexGrid.MovementHex(CurrentPosition, FleetSpeed).Contains(new_tile))
        {
            transform.SetParent(new_tile.transform, false);
            transform.localPosition = new Vector3(0.0f, 0.25f, 0.0f);
            CurrentPosition = new_tile;
        }
    }

    void OnTriggerStay(Collider other)
    {
        WaterHex water_hex = other.GetComponent<WaterHex>();

        if (water_hex && water_hex.Fog)
        {
            water_hex.Fog = false;
            water_hex.GetComponent<MeshRenderer>().material = water_hex.DefaultMaterial;

            GameObject tile = GameObject.Find("MiniMap").transform.FindChild(other.name).gameObject;

            tile.GetComponent<MeshRenderer>().material = water_hex.DefaultMaterial;
        }
    }

    void OnTriggerExit(Collider other)
    {
        WaterHex water_hex = other.GetComponent<WaterHex>();

        if (water_hex && !water_hex.Fog)
        {
            water_hex.Fog = true;
            water_hex.GetComponent<MeshRenderer>().material = water_hex.FogMaterial;

            GameObject tile = GameObject.Find("MiniMap").transform.FindChild(other.name).gameObject;

            tile.GetComponent<MeshRenderer>().material = water_hex.FogMaterial;
        }
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
