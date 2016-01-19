using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Network;

public class Cargo
{
    public int Food { get; private set; }
    public int Gold { get; private set; }
    public double Size { get { return Food / 100.0 + Gold / 1000.0; } private set { } }

    public Cargo(int food, int gold)
    {
        Food = food;
        Gold = gold;
    }

    public void TakeCargo(Cargo cargo, int food, int gold)
    {
        food = Mathf.Clamp(food, 0, cargo.Food);
        gold = Mathf.Clamp(gold, 0, cargo.Gold);
        Food += food;
        cargo.Food -= food;
        Gold += gold;
        cargo.Gold -= gold;
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
    public int HullHealth { get; private set; }
    public int SailHealth { get; private set; }
    public int CargoSpace { get; private set; }
    public int Cannons { get; private set; }
    public int MaxCannons { get; private set; }
    public int Speed { get; private set; }
    public int FullSpeed { get; private set; }
    public int CrewNeeded { get; private set; }
    public double DodgeChance { get; private set; }
    public int CrewMorale { get; private set; }
    public string Name { get; private set; }

    public ShipClass Class { get { return Class; } set { SetClass(value); } }
    public Cargo Cargo { get; private set; }
    public PlayerScript Owner { get; private set; }

	void Start()
    {
        SetClass(ShipClass.Pinnace);
	}
	
	void Update()
    {

	}

    //This is only called once, when the object is first created
    //Wouldn't make much sense for a ship to suddenly become a different kind of ship
    public void SetClass(ShipClass new_class)
    {
        Class = new_class;

        switch(new_class)
        {
            case ShipClass.Pinnace:
                HullHealth = 50;
                SailHealth = 50;
                CargoSpace = 15;
                MaxCannons = 4;
                FullSpeed = 5;
                CrewNeeded = 6;
                DodgeChance = 0.4;
                break;
            case ShipClass.Sloop:
                HullHealth = 60;
                SailHealth = 70;
                CargoSpace = 35;
                MaxCannons = 6;
                FullSpeed = 5;
                CrewNeeded = 9;
                DodgeChance = 0.35;
                break;
            case ShipClass.Barque:
                HullHealth = 80;
                SailHealth = 80;
                CargoSpace = 45;
                MaxCannons = 8;
                FullSpeed = 4;
                CrewNeeded = 11;
                DodgeChance = 0.3;
                break;
            case ShipClass.Brig:
                HullHealth = 120;
                SailHealth = 100;
                CargoSpace = 100;
                MaxCannons = 10;
                FullSpeed = 3;
                CrewNeeded = 14;
                DodgeChance = 0.25;
                break;
            case ShipClass.Merchantman:
                HullHealth = 120;
                SailHealth = 100;
                CargoSpace = 150;
                MaxCannons = 8;
                FullSpeed = 3;
                CrewNeeded = 12;
                DodgeChance = 0.25;
                break;
            case ShipClass.MerchantGalleon:
                HullHealth = 200;
                SailHealth = 200;
                CargoSpace = 450;
                MaxCannons = 12;
                FullSpeed = 2;
                CrewNeeded = 19;
                DodgeChance = 0.15;
                break;
            case ShipClass.CombatGalleon:
                HullHealth = 250;
                SailHealth = 250;
                CargoSpace = 400;
                MaxCannons = 24;
                FullSpeed = 2;
                CrewNeeded = 31;
                DodgeChance = 0.15;
                break;
            case ShipClass.Frigate:
                HullHealth = 300;
                SailHealth = 300;
                CargoSpace = 600;
                MaxCannons = 32;
                FullSpeed = 1;
                CrewNeeded = 43;
                DodgeChance = 0.1;
                break;
        }
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
        Name = new_name;
    }

    public void AddCargo(Cargo new_cargo)
    {
        if (Cargo.Size + new_cargo.Size <= CargoSpace)
            Cargo.TakeCargo(new_cargo, new_cargo.Food, new_cargo.Gold);
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
            Owner = null;
        }
    }
}