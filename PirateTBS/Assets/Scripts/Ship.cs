using UnityEngine;
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
    [SyncVar(hook = "OnNameChanged")]
    public string Name;                         //Name of the ship
    [SyncVar]
    public int HullHealth;                      //Current hull health
    [SyncVar]
    public int SailHealth;                      //Current sail health
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
    public int DamageMod;                       //Damage modifier
    [SyncVar]
    public int ReloadSpeed;                     //Time in seconds it takes to reload cannons

    public string ShipType;                     //Text representation of ship type
    public int MaxCannons;                      //Max cannon count
    public int Speed;                           //Current ship speed
    public int CrewMorale;                      //Current morale level of crew
    public ShipClass Class;                     //Class of ship
    public int Price;                           //Price of ship, used at port shipyards
    public int CurrentCrew;                     //Current crew count

    public void CopyShip(Ship other)
    {
        if (!other)
            return;

        Name = other.Name;
        HullHealth = other.HullHealth;
        SailHealth = other.SailHealth;
        CargoSpace = other.CargoSpace;
        Cannons = other.Cannons;
        FullSpeed = other.FullSpeed;
        CrewNeeded = other.CrewNeeded;
        DodgeChance = other.DodgeChance;
        Cargo = other.Cargo;
        DamageMod = other.DamageMod;
        ReloadSpeed = other.ReloadSpeed;
        ShipType = other.ShipType;
        MaxCannons = other.MaxCannons;
        Speed = other.Speed;
        CrewMorale = other.CrewMorale;
        Class = other.Class;
        Price = other.Price;
        CurrentCrew = other.CurrentCrew;
    }

	void Start()
    {
        
	}

    void Update()
    {

	}

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();

        PlayerInfoManager.Instance.AddShipToList(this);
    }
    
    /// <summary>
    /// Server-side function to set the ship class, which determines base stats
    /// </summary>
    /// <param name="new_class">Class to set ship class to</param>
    [Server]
    public void SetClass(ShipClass new_class)
    {
        Class = new_class;
        ShipType = new_class.ToString();

        switch(new_class)
        {
            case ShipClass.Pinnace:
                HullHealth = 50;
                SailHealth = 50;
                CargoSpace = 15;
                Cannons = MaxCannons = 4;
                Speed = FullSpeed = 5;
                CrewNeeded = 6;
                DodgeChance = 40;
                break;
            case ShipClass.Sloop:
                HullHealth = 60;
                SailHealth = 70;
                CargoSpace = 35;
                Cannons = MaxCannons = 6;
                Speed = FullSpeed = 5;
                CrewNeeded = 9;
                DodgeChance = 35;
                break;
            case ShipClass.Barque:
                HullHealth = 80;
                SailHealth = 80;
                CargoSpace = 45;
                Cannons = MaxCannons = 8;
                Speed = FullSpeed = 4;
                CrewNeeded = 11;
                DodgeChance = 30;
                break;
            case ShipClass.Brig:
                HullHealth = 120;
                SailHealth = 100;
                CargoSpace = 100;
                Cannons = MaxCannons = 10;
                Speed = FullSpeed = 3;
                CrewNeeded = 14;
                DodgeChance = 25;
                break;
            case ShipClass.Merchantman:
                HullHealth = 120;
                SailHealth = 100;
                CargoSpace = 150;
                Cannons = MaxCannons = 8;
                Speed = FullSpeed = 3;
                CrewNeeded = 12;
                DodgeChance = 25;
                break;
            case ShipClass.MerchantGalleon:
                HullHealth = 200;
                SailHealth = 200;
                CargoSpace = 450;
                Cannons = MaxCannons = 12;
                Speed = FullSpeed = 2;
                CrewNeeded = 19;
                DodgeChance = 15;
                ShipType = "Merchant Galleon";
                break;
            case ShipClass.CombatGalleon:
                HullHealth = 250;
                SailHealth = 250;
                CargoSpace = 400;
                Cannons = MaxCannons = 24;
                Speed = FullSpeed = 2;
                CrewNeeded = 31;
                DodgeChance = 15;
                ShipType = "Combat Galleon";
                break;
            case ShipClass.Frigate:
                HullHealth = 300;
                SailHealth = 300;
                CargoSpace = 600;
                Cannons = MaxCannons = 32;
                Speed = FullSpeed = 1;
                CrewNeeded = 43;
                DodgeChance = 10;
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
    public void DamageShip(int hdam, int sdam)
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
    [Command]
    public void CmdSinkShip()
    {
        Network.Destroy(this.gameObject);
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
            GetComponentInParent<Fleet>().CmdRemoveShip(this.name);
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
}