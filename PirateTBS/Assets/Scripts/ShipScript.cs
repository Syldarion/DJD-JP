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

    public Cargo()
    {
        Food = 0;
        Goods = 0;
        Luxuries = 0;
        Spice = 0;
        Sugar = 0;
        Gold = 0;
    }

    public void MergeCargo(Cargo cargo)
    {
        Food += cargo.Food;
        Goods += cargo.Goods;
        Luxuries += cargo.Luxuries;
        Spice += cargo.Spice;
        Sugar += cargo.Sugar;
        Gold += cargo.Gold;
    }

    public int Size()
    {
        return Food + Goods + Luxuries + Spice + Sugar + Gold;
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

public class ShipScript : NetworkedMonoBehavior
{
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

	void Start()
    {
        SetClass((ShipClass)Random.Range(0, 7));
	}
	
	void Update()
    {

	}

    //This is only called once, when the object is first created
    //Wouldn't make much sense for a ship to suddenly become a different kind of ship
    //Edit: Also called when clearing modifiers, because I am a terrible person
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

        GetComponentInParent<FleetScript>().UpdateFleetSpeed();
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

    public void DamageShip(int hdam, int sdam)
    {
        HullHealth -= hdam;
        SailHealth -= sdam;

        if (HullHealth <= 0) SinkShip();
        Speed = (int)(FullSpeed * (SailHealth / 100.0f));
    }

    public void RenameShip(string new_name)
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

    public void AddCargo(Cargo new_cargo)
    {
        if (Cargo.Size() + new_cargo.Size() <= CargoSpace)
            Cargo.MergeCargo(new_cargo);
    }

    public void SinkShip()
    {
        Destroy(gameObject);
    }

    public void ModifyMorale(int modifier)
    {
        CrewMorale = Mathf.Clamp(CrewMorale + modifier, 0, 100);
        if (CrewMorale <= 30)
        {
            GetComponentInParent<FleetScript>().RemoveShip(this);
        }
    }

    [BRPC]
    public void SpawnShip(string ship_name, string fleet_parent_name)
    {
        this.name = ship_name;

        FleetScript parent_fleet = GameObject.Find(fleet_parent_name).GetComponent<FleetScript>();
        transform.SetParent(parent_fleet.transform);
        transform.localPosition = Vector3.zero;
    }
}