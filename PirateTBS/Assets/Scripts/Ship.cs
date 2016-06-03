﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public struct Cargo
{
    public int Food;
    public int Goods;
    public int Luxuries;
    public int Spice;
    public int Sugar;

    /// <summary>
    /// Creates a new Cargo object
    /// </summary>
    /// <param name="food">1 food = 1/10 cargo</param>
    /// <param name="goods">1 goods = 1/10 cargo</param>
    /// <param name="sugar">1 sugar = 1/10 cargo</param>
    /// <param name="spice">1 spice = 1/10 cargo</param>
    /// <param name="luxuries">1 luxuries = 1 cargo</param>
    public Cargo(int food = 0, int goods = 0, int sugar = 0, int spice = 0, int luxuries = 0)
    {
        Food = food;
        Goods = goods;
        Luxuries = luxuries;
        Spice = spice;
        Sugar = sugar;
    }

    /// <summary>
    /// Merge another cargo into this cargo
    /// </summary>
    /// <param name="cargo">The cargo to merge</param>
    public void MergeCargo(Cargo cargo)
    {
        Food += cargo.Food;
        Goods += cargo.Goods;
        Luxuries += cargo.Luxuries;
        Spice += cargo.Spice;
        Sugar += cargo.Sugar;
    }

    public int GetCargoAmount(string cargo_type)
    {
        switch (cargo_type)
        {
            case "Food":
                return Food;
            case "Goods":
                return Goods;
            case "Sugar":
                return Sugar;
            case "Spice":
                return Spice;
            case "Luxuries":
                return Luxuries;
            default:
                return 0;
        }
    }

    public void TransferTo(ref Cargo cargo, string cargo_type, int quantity)
    {
        switch(cargo_type)
        {
            case "Food":
                cargo.Food += Mathf.Clamp(quantity, 0, Food);
                Food = Mathf.Clamp(Food, 0, Food - quantity);
                break;
            case "Goods":
                cargo.Goods += Mathf.Clamp(quantity, 0, Goods);
                Goods = Mathf.Clamp(Goods, 0, Goods - quantity);
                break;
            case "Sugar":
                cargo.Sugar += Mathf.Clamp(quantity, 0, Sugar);
                Sugar = Mathf.Clamp(Sugar, 0, Sugar - quantity);
                break;
            case "Spice":
                cargo.Spice += Mathf.Clamp(quantity, 0, Spice);
                Spice = Mathf.Clamp(Spice, 0, Spice - quantity);
                break;
            case "Luxuries":
                cargo.Luxuries += Mathf.Clamp(quantity, 0, Luxuries);
                Luxuries = Mathf.Clamp(Luxuries, 0, Luxuries - quantity);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// The amount of cargo space taken by the cargo
    /// </summary>
    /// <returns></returns>
    public double Size()
    {
        return (Food * 0.1) + (Goods + 0.1) + Luxuries + (Spice + 0.1) + (Sugar * 0.1);
    }

    /// <summary>
    /// Returns the amount of space the resource type takes
    /// </summary>
    /// <param name="resource_type">Type of resource</param>
    /// <returns>Space a single piece of that type takes</returns>
    public static double GetSizeReq(string resource_type)
    {
        switch(resource_type)
        {
            case "Gold":
                return 0.01;
            case "Food":
            case "Goods":
            case "Spice":
            case "Sugar":
                return 0.1;
            case "Luxuries":
            default:
                return 1.0;
        }
    }
}

public class CrewMember
{
    public string Name;
    public int Morale;

    public CrewMember(string name, int morale)
    {
        Name = name;
        Morale = morale;
    }
}

public enum ShipClass
{
    Pinnace,
    Sloop,
    Barque,
    Brig,
    Merchantman,
    MerchantGalleon,
    CombatGalleon,
    Frigate
}

public enum ShotType
{
    Normal,
    Cluster,
    Chain
}

public class Ship : NetworkBehaviour
{
    PlayerScript OwningPlayer;

    [SyncVar(hook = "OnNameChanged")]
    public string Name;                         //Name of the ship
    [SyncVar]
    public int HullHealth;                      //Current hull health
    [SyncVar]
    public int MaxHullHealth;
    [SyncVar]
    public int SailHealth;                      //Current sail health
    [SyncVar]
    public int MaxSailHealth;
    [SyncVar]
    public int CargoSpace;                      //Total cargo space
    [SyncVar]
    public int Cannons;                         //Current cannon count
    [SyncVar]
    public int FullSpeed;                       //Max speed of ship
    [SyncVar]
    public int CrewNeeded;                      //Crew members needed for ideal ship performance
    [SyncVar]
    public int DodgeChance;                     //Chance for ship to dodge projectiles
    [SyncVar]
    public Cargo Cargo;                         //Reference to this ship's cargo
    [SyncVar]
    public int Gold;
    [SyncVar]
    public int DamageMod;                       //Damage modifier
    [SyncVar]
    public int ReloadSpeed;                     //Time in seconds it takes to reload cannons
    [SyncVar]
    public bool MoveActionTaken;                //Tracks if this ship has moved this turn
    [SyncVar]
    public bool CombatActionTaken;              //Tracks if this ship has been in combat this turn
    [SyncVar]
    public string ShipType;                     //Text representation of ship type
    [SyncVar]
    public int MaxCannons;                      //Max cannon count
    [SyncVar]
    public int Speed;                           //Current ship speed
    [SyncVar]
    public int CrewMorale;                      //Current morale level of crew
    [SyncVar]
    public ShipClass Class;                     //Class of ship
    [SyncVar]
    public int Price;                           //Price of ship, used at port shipyards
    [SyncVar]
    public int CurrentCrew;                     //Current crew count

    public HexTile CurrentPosition;
    public List<WaterHex> MovementQueue;

    public void CopyShip(Ship other)
    {
        if (!other)
            return;

        CmdCopyShip(
            other.Name,
            other.HullHealth,
            other.MaxHullHealth,
            other.SailHealth,
            other.MaxSailHealth,
            other.CargoSpace,
            other.Cannons,
            other.FullSpeed,
            other.CrewNeeded,
            other.DodgeChance,
            other.Cargo,
            other.DamageMod,
            other.ReloadSpeed,
            other.ShipType,
            other.MaxCannons,
            other.Speed,
            other.CrewMorale,
            other.Class,
            other.Price,
            other.CurrentCrew);
    }

    [Command]
    public void CmdCopyShip(string name, int hh, int mhh, int sh, int msh, 
        int cs, int can, int fs, int cn, int dc, Cargo car, int dm, int rs,
        string st, int mc, int s, int cm, ShipClass c, int p, int cc)
    {
        Name = name;
        HullHealth = hh;
        MaxHullHealth = mhh;
        SailHealth = sh;
        MaxSailHealth = msh;
        CargoSpace = cs;
        Cannons = can;
        FullSpeed = fs;
        CrewNeeded = cn;
        DodgeChance = dc;
        Cargo = car;
        DamageMod = dm;
        ReloadSpeed = rs;
        ShipType = st;
        MaxCannons = mc;
        Speed = s;
        CrewMorale = cm;
        Class = c;
        Price = p;
        CurrentCrew = cc;
    }

	void Start()
    {
        MovementQueue = new List<WaterHex>();
	}

    void Update()
    {

	}

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();

        PlayerScript.MyPlayer.AddShip(this);

        Camera.main.GetComponent<PanCamera>().CenterOnTarget(this.transform);
        if (!PlayerScript.MyPlayer.Ships.Contains(this))
            PlayerScript.MyPlayer.Ships.Add(this);
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        MoveActionTaken = false;
        CombatActionTaken = false;

        Cargo = new Cargo(500, 50, 0, 0, 0);
    }

    /// <summary>
    /// Server-side function to set the ship class, which determines base stats
    /// </summary>
    /// <param name="new_class">Class to set ship class to</param>
    public void SetClass(ShipClass new_class)
    {
        Class = new_class;
        ShipType = new_class.ToString();

        switch(new_class)
        {
            case ShipClass.Pinnace:
                HullHealth = MaxHullHealth = 50;
                SailHealth = MaxSailHealth = 50;
                CargoSpace = 15;
                Cannons = MaxCannons = 4;
                Speed = FullSpeed = 5;
                CrewNeeded = 6;
                DodgeChance = 40;
                Price = 50;
                break;
            case ShipClass.Sloop:
                HullHealth = MaxHullHealth = 60;
                SailHealth = MaxSailHealth = 70;
                CargoSpace = 35;
                Cannons = MaxCannons = 6;
                Speed = FullSpeed = 5;
                CrewNeeded = 9;
                DodgeChance = 35;
                Price = 100;
                break;
            case ShipClass.Barque:
                HullHealth = MaxHullHealth = 80;
                SailHealth = MaxSailHealth = 80;
                CargoSpace = 45;
                Cannons = MaxCannons = 8;
                Speed = FullSpeed = 4;
                CrewNeeded = 11;
                DodgeChance = 30;
                Price = 150;
                break;
            case ShipClass.Brig:
                HullHealth = MaxHullHealth = 120;
                SailHealth = MaxSailHealth = 100;
                CargoSpace = 100;
                Cannons = MaxCannons = 10;
                Speed = FullSpeed = 3;
                CrewNeeded = 14;
                DodgeChance = 25;
                Price = 200;
                break;
            case ShipClass.Merchantman:
                HullHealth = MaxHullHealth = 120;
                SailHealth = MaxSailHealth = 100;
                CargoSpace = 150;
                Cannons = MaxCannons = 8;
                Speed = FullSpeed = 3;
                CrewNeeded = 12;
                DodgeChance = 25;
                Price = 200;
                break;
            case ShipClass.MerchantGalleon:
                HullHealth = MaxHullHealth = 200;
                SailHealth = MaxSailHealth = 200;
                CargoSpace = 450;
                Cannons = MaxCannons = 12;
                Speed = FullSpeed = 2;
                CrewNeeded = 19;
                DodgeChance = 15;
                ShipType = "Merchant Galleon";
                Price = 500;
                break;
            case ShipClass.CombatGalleon:
                HullHealth = MaxHullHealth = 250;
                SailHealth = MaxSailHealth = 250;
                CargoSpace = 400;
                Cannons = MaxCannons = 24;
                Speed = FullSpeed = 2;
                CrewNeeded = 31;
                DodgeChance = 15;
                ShipType = "Combat Galleon";
                Price = 800;
                break;
            case ShipClass.Frigate:
                HullHealth = MaxHullHealth = 300;
                SailHealth = MaxSailHealth = 300;
                CargoSpace = 600;
                Cannons = MaxCannons = 32;
                Speed = FullSpeed = 1;
                CrewNeeded = 43;
                DodgeChance = 10;
                Price = 1500;
                break;
        }
    }

    /// <summary>
    /// Command to modify ship stat on server
    /// </summary>
    /// <param name="modify_string">String representing the stat modification to execute</param>
    [Command]
    public void CmdUpdateStat(string modify_string)
    {
        if (modify_string == string.Empty)
            return;

        string[] split = modify_string.Split(' ');

        if (split.Length != 3)
            return;

        string var = split[0];
        string mod_oper = split[1];
        int val = int.Parse(split[2]);

        int current_var_val = (int)GetType().GetField(var).GetValue(this);

        switch(mod_oper)
        {
            case "+":
                GetType().GetField(var).SetValue(this, current_var_val + val);
                break;
            case "-":
                GetType().GetField(var).SetValue(this, current_var_val - val);
                break;
            case "*":
                GetType().GetField(var).SetValue(this, current_var_val * val);
                break;
            case "/":
                GetType().GetField(var).SetValue(this, current_var_val / val);
                break;
            default:
                return;
        }
    }
    
    /// <summary>
    /// Apply damage to ship
    /// </summary>
    /// <param name="hdam">Hull damage</param>
    /// <param name="sdam">Sail damage</param>
    [Command]
    public void CmdDamageShip(int hdam, int sdam)
    {
        HullHealth -= hdam;
        SailHealth -= sdam;

        if (HullHealth <= 0) CmdSinkShip();
        Speed = (int)(FullSpeed * (SailHealth / 100.0f));
    }
    
    /// <summary>
    /// Renames the ship
    /// </summary>
    /// <param name="new_name">New ship name</param>
    public void RenameShip(string new_name)
    {
        if (!GameObject.Find(new_name))
        {
            new_name = new_name.Trim();
            for (int i = 0; i < new_name.Length; i++)
                if (!char.IsLetter(new_name[i]) && new_name[i] != ' ')
                {
                    new_name = new_name.Remove(i, 1);
                    i--;
                }
            name = new_name;
        }
    }
    
    /// <summary>
    /// Adds cargo to this ship's cargo
    /// </summary>
    /// <param name="new_cargo">Cargo to mergo onto this ship</param>
    public void AddCargo(Cargo new_cargo)
    {
        if (Cargo.Size() + new_cargo.Size() <= CargoSpace)
            Cargo.MergeCargo(new_cargo);
    }
    
    /// <summary>
    /// Server-side command to destroy this ship
    /// </summary>
    [ClientRpc]
    public void RpcSinkShip()
    {
        //Network.Destroy(this.gameObject);

        if (PlayerScript.MyPlayer.Ships.Contains(this))
            PlayerScript.MyPlayer.RemoveShip(this);

        Destroy(gameObject);   
    }
    
    /// <summary>
    /// Modify the morale of this ship
    /// </summary>
    /// <param name="modifier">Modification to ship morale</param>
    public void ModifyMorale(int modifier)
    {
        CrewMorale = Mathf.Clamp(CrewMorale + modifier, 0, 100);
        if (CrewMorale <= 30)
        {
            //mutiny
        }
    }

    /// <summary>
    /// Callback when ship name is changed
    /// </summary>
    /// <param name="new_name">New ship name</param>
    void OnNameChanged(string new_name)
    {
        GameConsole.Instance.GenericLog(Name);
        name = new_name;
    }

    /// <summary>
    /// Server-side command to place newly spawned ship
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
    /// Client-side command to place newly spawned ship
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
    //[Command]
    public void CmdQueueMove(int x, int y)
    {
        WaterHex new_tile = GameObject.Find(string.Format("Grid/{0},{1}", x, y)).GetComponent<WaterHex>();
        if (new_tile)
            MovementQueue.Add(new_tile);
    }

    /// <summary>
    /// Server-side command to move ship along movement queue
    /// </summary>
    //[Command]
    public void CmdMoveShip()
    {
        Debug.Log(string.Format("{0} tiles in movement queue for {1}.", MovementQueue.Count, Name));

        if (MoveActionTaken || MovementQueue.Count <= 0)
            return;

        if (!HexGrid.MovementHex(CurrentPosition, 1).Contains(MovementQueue[0]))
        {
            ClearMovementQueue();
            return;
        }

        //transform.SetParent(MovementQueue[MovementQueue.Count - 1].transform, true);
        //CurrentPosition = MovementQueue[MovementQueue.Count - 1];

        WaterHex final = MovementQueue[MovementQueue.Count - 1];
        
        CmdUpdatePosition(final.HexCoord.Q, final.HexCoord.R);

        //RpcMoveShip(MovementQueue[MovementQueue.Count - 1].HexCoord.Q, MovementQueue[MovementQueue.Count - 1].HexCoord.R);

        //StopAllCoroutines();
        StartCoroutine(SmoothMove());

        MoveActionTaken = true;
    }

    [Command]
    public void CmdUpdatePosition(int x, int y)
    {
        WaterHex new_tile = GameObject.Find(string.Format("Grid/{0},{1}", x, y)).GetComponent<WaterHex>();
        transform.SetParent(new_tile.transform, true);
        CurrentPosition = new_tile;

        RpcUpdatePosition(x, y);
    }

    [ClientRpc]
    public void RpcUpdatePosition(int x, int y)
    {
        WaterHex new_tile = GameObject.Find(string.Format("Grid/{0},{1}", x, y)).GetComponent<WaterHex>();
        transform.SetParent(new_tile.transform, true);
        CurrentPosition = new_tile;
    }

    public void ClearMovementQueue()
    {
        foreach (WaterHex hex in MovementQueue)
        {
            if (hex.Discovered)
                hex.GetComponent<MeshRenderer>().sharedMaterial = hex.CloudMaterial;
            else if (hex.Fog)
                hex.GetComponent<MeshRenderer>().sharedMaterial = hex.FogMaterial;
            else
                hex.GetComponent<MeshRenderer>().sharedMaterial = hex.DefaultMaterial;
        }
        MovementQueue.Clear();
    }

    /// <summary>
    /// Smoothly moves ship along tiles in movement queue
    /// </summary>
    /// <returns></returns>
    public IEnumerator SmoothMove()
    {
        foreach (HexTile dest_tile in MovementQueue)
        {
            Ship tile_ship = dest_tile.GetComponentInChildren<Ship>();

            if (tile_ship && tile_ship != this)
            {
                if (PlayerScript.MyPlayer.Ships.Contains(tile_ship))
                {
                    //open cargo manager
                }
                else
                {
                    CombatManager.Instance.OpenCombatPanel();
                    CombatManager.Instance.StartCombat(this, tile_ship);
                }
            }
            else
            {
                Vector3 destination = dest_tile.transform.position + new Vector3(0.0f, 0.25f, 0.0f);
                transform.LookAt(destination);

                Vector3 direction = (destination - transform.position) / 20.0f;
                float step = direction.magnitude;

                for (int i = 0; i < 20; i++)
                {
                    transform.Translate(Vector3.forward * step);
                    yield return new WaitForSeconds(0.01f);
                }

                transform.position = destination;
            }

            yield return null;
        }

        ClearMovementQueue();

        transform.localPosition = new Vector3(0.0f, 0.25f, 0.0f);
    }

    /// <summary>
    /// Client-side command to move ship
    /// </summary>
    /// <param name="x">Q coordinate of tile</param>
    /// <param name="y">R coordinate of tile</param>
    [ClientRpc]
    public void RpcMoveShip(int x, int y)
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

        if (land_hex && !land_hex.Discovered)
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
        if (PlayerScript.MyPlayer.Ships.Contains(this))
            PlayerScript.MyPlayer.ActiveShip = this;
    }

    void OnMouseEnter()
    {
        Tooltip.Instance.EnableTooltip(true);
        Tooltip.Instance.UpdateTooltip(name);
    }

    void OnMouseExit()
    {
        Tooltip.Instance.EnableTooltip(false);
    }
}