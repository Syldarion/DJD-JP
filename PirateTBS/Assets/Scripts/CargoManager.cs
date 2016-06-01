using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Reflection;

public class CargoManager : MonoBehaviour
{
    [HideInInspector]
    public static CargoManager Instance;

    public Ship ShipA;                          //Reference to left-hand ship
    public Ship ShipB;                          //Reference to right-hand ship

    public Dropdown LefthandShips;              //Reference to dropdown for left-hand ships
    public Dropdown RighthandShips;             //Reference to dropdown for right-hand ships

    public RectTransform LefthandCargo;         //Reference to container for left-hand ship's cargo
    public RectTransform RighthandCargo;        //Reference to container for right-hand ship's cargo

    string[] AllResources = new string[] { "Food", "Goods", "Sugar", "Spice", "Luxuries" };

    void Start()
	{
        Instance = this;
	}

	void Update()
	{
		
	}

    /// <summary>
    /// Opens the cargo manager panel
    /// </summary>
    public void OpenCargoManager()
    {
        PlayerScript.MyPlayer.OpenUI = GetComponent<CanvasGroup>();

        PanelUtilities.ActivatePanel(GetComponent<CanvasGroup>());
    }

    /// <summary>
    /// Closes the cargo manager panel
    /// </summary>
    public void CloseCargoManager()
    {
        PlayerScript.MyPlayer.OpenUI = null;

        PanelUtilities.DeactivatePanel(GetComponent<CanvasGroup>());
    }

    /// <summary>
    /// Populates cargo lists
    /// </summary>
    /// <param name="ship_a">Left-hand ship</param>
    /// <param name="ship_b">Right-hand ship</param>
    public void PopulateCargoList(Ship ship_a, Ship ship_b)
    {
        if (ship_a == ship_b)
            return;

        ShipA = ship_a;
        ShipB = ship_b;

        UpdateResourceList(ShipA, LefthandCargo);
        UpdateResourceList(ShipB, RighthandCargo);
    }

    /// <summary>
    /// Updates a resource list
    /// </summary>
    /// <param name="ship">New ship to use</param>
    /// <param name="resource_list">Resource list to repopulate</param>
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

    /// <summary>
    /// Transfers cargo from left-hand ship to right-hand ship
    /// </summary>
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

            ShipA.Cargo.TransferTo(ref ShipB.Cargo, s, transfer_amount);
        }

        UpdateResourceList(ShipA, LefthandCargo);
    }

    /// <summary>
    /// Transfers cargo from right-hand ship to left-hand ship
    /// </summary>
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

            ShipB.Cargo.TransferTo(ref ShipA.Cargo, s, transfer_amount);
        }

        UpdateResourceList(ShipB, RighthandCargo);
    }
}
