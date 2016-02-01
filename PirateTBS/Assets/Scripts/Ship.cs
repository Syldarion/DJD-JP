using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Network;

public class Cargo
{
    public int Food;
    public int Goods;
    public int Luxuries;
    public int Spice;
    public int Sugar;
    public int Gold;

    /// <summary>
    /// Creates a new Cargo object
    /// </summary>
    /// <param name="food">1 food = 1/10 cargo</param>
    /// <param name="gold">1 gold = 1/100 cargo</param>
    /// <param name="goods">1 goods = 1/10 cargo</param>
    /// <param name="sugar">1 sugar = 1/10 cargo</param>
    /// <param name="spice">1 spice = 1/10 cargo</param>
    /// <param name="luxuries">1 luxuries = 1 cargo</param>
    public Cargo(int food = 0, int gold = 0, int goods = 0, int sugar = 0, int spice = 0, int luxuries = 0)
    {
        Food = food;
        Goods = goods;
        Luxuries = luxuries;
        Spice = spice;
        Sugar = sugar;
        Gold = gold;
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
        Gold += cargo.Gold;
    }

    /// <summary>
    /// The amount of cargo space taken by the cargo
    /// </summary>
    /// <returns></returns>
    public double Size()
    {
        return (Food * 0.1) + (Goods + 0.1) + Luxuries + (Spice + 0.1) + (Sugar * 0.1) + (Gold * 0.01);
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

public class Ship : NetworkedMonoBehavior
{
    [NetSync("OnNameChanged", NetworkCallers.Everyone)]
    public string Name;
    [NetSync] public int HullHealth;
    [NetSync] public int SailHealth;
    [NetSync] public int CargoSpace;
    [NetSync] public int Cannons;
    [NetSync] public int FullSpeed;
    [NetSync] public int CrewNeeded;
    [NetSync] public double DodgeChance;

    public string ShipType;
    public int MaxCannons;
    public int Speed;
    public int CrewMorale;
    public ShipClass Class;
    public Cargo Cargo;
    public int Price;

	void Start()
    {
        
	}
	
	void Update()
    {

	}
    
    void SetClass(ShipClass new_class)
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
                DodgeChance = 0.4;
                break;
            case ShipClass.Sloop:
                HullHealth = 60;
                SailHealth = 70;
                CargoSpace = 35;
                Cannons = MaxCannons = 6;
                Speed = FullSpeed = 5;
                CrewNeeded = 9;
                DodgeChance = 0.35;
                break;
            case ShipClass.Barque:
                HullHealth = 80;
                SailHealth = 80;
                CargoSpace = 45;
                Cannons = MaxCannons = 8;
                Speed = FullSpeed = 4;
                CrewNeeded = 11;
                DodgeChance = 0.3;
                break;
            case ShipClass.Brig:
                HullHealth = 120;
                SailHealth = 100;
                CargoSpace = 100;
                Cannons = MaxCannons = 10;
                Speed = FullSpeed = 3;
                CrewNeeded = 14;
                DodgeChance = 0.25;
                break;
            case ShipClass.Merchantman:
                HullHealth = 120;
                SailHealth = 100;
                CargoSpace = 150;
                Cannons = MaxCannons = 8;
                Speed = FullSpeed = 3;
                CrewNeeded = 12;
                DodgeChance = 0.25;
                break;
            case ShipClass.MerchantGalleon:
                HullHealth = 200;
                SailHealth = 200;
                CargoSpace = 450;
                Cannons = MaxCannons = 12;
                Speed = FullSpeed = 2;
                CrewNeeded = 19;
                DodgeChance = 0.15;
                ShipType = "Merchant Galleon";
                break;
            case ShipClass.CombatGalleon:
                HullHealth = 250;
                SailHealth = 250;
                CargoSpace = 400;
                Cannons = MaxCannons = 24;
                Speed = FullSpeed = 2;
                CrewNeeded = 31;
                DodgeChance = 0.15;
                ShipType = "Combat Galleon";
                break;
            case ShipClass.Frigate:
                HullHealth = 300;
                SailHealth = 300;
                CargoSpace = 600;
                Cannons = MaxCannons = 32;
                Speed = FullSpeed = 1;
                CrewNeeded = 43;
                DodgeChance = 0.1;
                break;
        }

        GetComponentInParent<Fleet>().UpdateFleetSpeed();
    }

    public void ApplyModifiers()
    {
        //apply modifiers from tech tree
    }

    public void ClearModifiers()
    {
        //this is so bad but it works
        SetClass(Class);
    }

    /// <summary>
    /// Apply damage to the ship
    /// </summary>
    /// <param name="hdam">Hull damage</param>
    /// <param name="sdam">Sail damage</param>
    public void DamageShip(int hdam, int sdam)
    {
        HullHealth -= hdam;
        SailHealth -= sdam;

        if (HullHealth <= 0) SinkShip();
        Speed = (int)(FullSpeed * (SailHealth / 100.0f));
    }

    /// <summary>
    /// Rename the ship, as long as a ship with that name doesn't exist
    /// </summary>
    /// <param name="new_name">The new name of the ship</param>
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
    /// Add cargo to the ship, as long as the ship has room for it
    /// </summary>
    /// <param name="new_cargo">The cargo to add to the ship</param>
    public void AddCargo(Cargo new_cargo)
    {
        if (Cargo.Size() + new_cargo.Size() <= CargoSpace)
            Cargo.MergeCargo(new_cargo);
    }

    /// <summary>
    /// Sinks the ship
    /// </summary>
    public void SinkShip()
    {
        NetworkDestroy(this.NetworkedId);
    }

    /// <summary>
    /// Modify the morale of the ship
    /// </summary>
    /// <param name="modifier">Modifier to apply to the ship</param>
    public void ModifyMorale(int modifier)
    {
        CrewMorale = Mathf.Clamp(CrewMorale + modifier, 0, 100);
        if (CrewMorale <= 30)
        {
            GetComponentInParent<Fleet>().RemoveShip(this);
        }
    }

    /// <summary>
    /// Callback to provide information for a newly spawned ship
    /// </summary>
    /// <param name="ship_name">Name of the new ship</param>
    /// <param name="fleet_parent_name">Name of the fleet the ship belongs to</param>
    [BRPC]
    public void SpawnShip(string ship_name, string fleet_parent_name)
    {
        Name = ship_name;

        Fleet parent_fleet = GameObject.Find(fleet_parent_name).GetComponent<Fleet>();
        transform.SetParent(parent_fleet.transform);
        transform.localPosition = Vector3.zero;

        SetClass((ShipClass)Random.Range(0, 7));
        Cargo = new Cargo(50, 500);
    }

    void OnNameChanged()
    {
        GameObject.Find("ConsolePanel").GetComponent<GameConsole>().GenericLog(Name);
        name = Name;
    }
}