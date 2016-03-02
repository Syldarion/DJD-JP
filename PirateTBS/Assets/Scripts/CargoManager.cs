using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Reflection;

public class CargoManager : MonoBehaviour
{
    [HideInInspector]
    public static CargoManager Instance;

    public Ship ShipA;
    public Ship ShipB;

    public Dropdown LefthandShips;
    public Dropdown RighthandShips;

    public RectTransform LefthandCargo;
    public RectTransform RighthandCargo;

    string[] AllResources = new string[] { "Food", "Goods", "Sugar", "Spice", "Luxuries" };

    void Start()
	{
        Instance = this;
	}

	void Update()
	{
		
	}

    public void OpenCargoManager()
    {
        PlayerScript.MyPlayer.UIOpen = true;

        PanelUtilities.ActivatePanel(GetComponent<CanvasGroup>());
    }

    public void CloseCargoManager()
    {
        PlayerScript.MyPlayer.UIOpen = false;

        PanelUtilities.DeactivatePanel(GetComponent<CanvasGroup>());
    }

    public void PopulateShipList(Fleet fleet)
    {
        if(fleet.Ships.Count < 2)
        {
            CloseCargoManager();
            return;
        }

        LefthandShips.options.Clear();
        RighthandShips.options.Clear();

        foreach(Ship s in fleet.Ships)
        {
            LefthandShips.options.Add(new Dropdown.OptionData(s.Name));
            RighthandShips.options.Add(new Dropdown.OptionData(s.name));
        }

        PopulateCargoList(fleet.Ships[0], fleet.Ships[1]);
    }

    public void PopulateCargoList(Ship ship_a, Ship ship_b)
    {
        if (ship_a == ship_b)
            return;

        ShipA = ship_a;
        ShipB = ship_b;

        UpdateResourceList(ShipA, LefthandCargo);
        UpdateResourceList(ShipB, RighthandCargo);
    }

    public void UpdateResourceList(Ship ship, RectTransform resource_list)
    {
        foreach(string s in AllResources)
        {
            Type cargo_type = typeof(Cargo);
            FieldInfo field = cargo_type.GetField(s);
            int value = (int)field.GetValue(ship.Cargo);
            
            resource_list.FindChild(string.Format("{0}/Text", s)).GetComponent<Text>().text = string.Format("{0}\n{1}", s, value);
            resource_list.FindChild(string.Format("{0}/Quantity", s)).GetComponent<NumericUpDown>().UpdateValue(0);
            resource_list.FindChild(string.Format("{0}/Quantity", s)).GetComponent<NumericUpDown>().SetMaxValue(value);
        }
    }

    public void TransferLeftToRight()
    {
        foreach (string s in AllResources)
        {
            int transfer_amount = LefthandCargo.FindChild(s).GetComponentInChildren<NumericUpDown>().Value;

            Cargo temp_cargo = new Cargo();
            temp_cargo.GetType().GetField(s).SetValue(temp_cargo, transfer_amount);

            if(temp_cargo.Size() + ShipB.Cargo.Size() > ShipB.CargoSpace)
            {
                double remaining_space = ShipB.CargoSpace - ShipB.Cargo.Size();
                transfer_amount = (int)(transfer_amount + (remaining_space - temp_cargo.Size()) / Cargo.GetSizeReq(s));
            }

            ShipA.Cargo.TransferTo(ShipB.Cargo, s, transfer_amount);
        }

        UpdateResourceList(ShipA, LefthandCargo);
    }

    public void TransferRightToLeft()
    {
        foreach (string s in AllResources)
        {
            int transfer_amount = RighthandCargo.FindChild(s).GetComponentInChildren<NumericUpDown>().Value;

            Cargo temp_cargo = new Cargo();
            temp_cargo.GetType().GetField(s).SetValue(temp_cargo, transfer_amount);

            if (temp_cargo.Size() + ShipB.Cargo.Size() > ShipB.CargoSpace)
            {
                double remaining_space = ShipA.CargoSpace - ShipA.Cargo.Size();
                transfer_amount = (int)(transfer_amount + (remaining_space - temp_cargo.Size()) / Cargo.GetSizeReq(s));
            }

            ShipB.Cargo.TransferTo(ShipA.Cargo, s, transfer_amount);
        }

        UpdateResourceList(ShipB, RighthandCargo);
    }
}
