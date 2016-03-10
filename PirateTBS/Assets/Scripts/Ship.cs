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

    public void TransferCargo(Cargo cargo, int food = 0, int gold = 0, int goods = 0, int sugar = 0, int spice = 0, int luxuries = 0)
    {
        cargo.Food += Mathf.Clamp(food, 0, Food);
        Food = Mathf.Clamp(Food, 0, Food - food);

        cargo.Gold += Mathf.Clamp(gold, 0, Gold);
        Gold = Mathf.Clamp(Gold, 0, Gold - gold);

        cargo.Goods += Mathf.Clamp(goods, 0, Goods);
        Goods = Mathf.Clamp(Goods, 0, Goods - goods);

        cargo.Sugar += Mathf.Clamp(sugar, 0, Sugar);
        Sugar = Mathf.Clamp(Sugar, 0, Sugar - sugar);

        cargo.Spice += Mathf.Clamp(spice, 0, Spice);
        Spice = Mathf.Clamp(Spice, 0, Spice - spice);

        cargo.Luxuries += Mathf.Clamp(luxuries, 0, Luxuries);
        Luxuries = Mathf.Clamp(Luxuries, 0, Luxuries - luxuries);
    }

    public void TransferTo(Cargo cargo, string cargo_type, int quantity)
    {
        int cargo_amount = (int)GetType().GetField(cargo_type).GetValue(this);
        int other_cargo_amount = (int)GetType().GetField(cargo_type).GetValue(cargo);

        int transfer_amount = Mathf.Clamp(quantity, 0, cargo_amount);

        other_cargo_amount += transfer_amount;
        cargo_amount -= transfer_amount;

        GetType().GetField(cargo_type).SetValue(this, cargo_amount);
        GetType().GetField(cargo_type).SetValue(cargo, other_cargo_amount);
    }

    /// <summary>
    /// The amount of cargo space taken by the cargo
    /// </summary>
    /// <returns></returns>
    public double Size()
    {
        return (Food * 0.1) + (Goods + 0.1) + Luxuries + (Spice + 0.1) + (Sugar * 0.1) + (Gold * 0.01);
    }

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
    public string Name;
    [SyncVar]
    public int HullHealth;
    [SyncVar]
    public int SailHealth;
    [SyncVar]
    public int CargoSpace;
    [SyncVar]
    public int Cannons;
    [SyncVar]
    public int FullSpeed;
    [SyncVar]
    public int CrewNeeded;
    [SyncVar]
    public double DodgeChance;
    [SyncVar]
    public Cargo Cargo;

    public string ShipType;
    public int MaxCannons;
    public int Speed;
    public int CrewMorale;
    public ShipClass Class;
    public int Price;
    public int CurrentCrew;

	void Start()
    {
        
	}
	
	void Update()
    {

	}
    
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
    }

    [Command]
    public void CmdApplyModifiers()
    {
        //apply modifiers from tech tree
    }
    
    [Command]
    public void CmdClearModifiers()
    {
        SetClass(Class);
    }
    
    public void DamageShip(int hdam, int sdam)
    {
        HullHealth -= hdam;
        SailHealth -= sdam;

        if (HullHealth <= 0) CmdSinkShip();
        Speed = (int)(FullSpeed * (SailHealth / 100.0f));
    }
    
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
    
    public void AddCargo(Cargo new_cargo)
    {
        if (Cargo.Size() + new_cargo.Size() <= CargoSpace)
            Cargo.MergeCargo(new_cargo);
    }
    
    [Command]
    public void CmdSinkShip()
    {
        Network.Destroy(this.gameObject);
    }
    
    public void ModifyMorale(int modifier)
    {
        CrewMorale = Mathf.Clamp(CrewMorale + modifier, 0, 100);
        if (CrewMorale <= 30)
        {
            GetComponentInParent<Fleet>().CmdRemoveShip(this.name);
        }
    }

    void OnNameChanged(string new_name)
    {
        GameObject.Find("ConsolePanel").GetComponent<GameConsole>().GenericLog(Name);
        name = new_name;
    }
}