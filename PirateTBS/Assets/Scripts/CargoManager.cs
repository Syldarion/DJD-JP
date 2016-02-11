using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Reflection;

public class CargoManager : MonoBehaviour
{
    public Ship ShipA;
    public Ship ShipB;

    public Dropdown LefthandShips;
    public Dropdown RighthandShips;

    public RectTransform LefthandCargo;
    public RectTransform RighthandCargo;

	void Start()
	{
		
	}

	void Update()
	{
		
	}

    public void OpenCargoManager()
    {
        GetComponent<CanvasGroup>().alpha = 1;
        GetComponent<CanvasGroup>().interactable = true;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void CloseCargoManager()
    {
        GetComponent<CanvasGroup>().alpha = 0;
        GetComponent<CanvasGroup>().interactable = false;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
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
        string[] resource_types = { "Food", "Goods", "Sugar", "Spice", "Luxuries" };

        foreach(string s in resource_types)
        {
            resource_list.FindChild(string.Format("{0}\\Text", s)).GetComponent<Text>().text = string.Format("{0}\n{1}", s, typeof(Cargo).GetType().GetField(s).GetValue(ship.Cargo));
            resource_list.FindChild(string.Format("{0}\\Text", s)).GetComponentInChildren<NumericUpDown>().UpdateValue(0);
            resource_list.FindChild(string.Format("{0}\\Text", s)).GetComponentInChildren<NumericUpDown>().SetMaxValue((int)typeof(Cargo).GetType().GetField(s).GetValue(ship.Cargo));
        }
    }

    public void TransferLeftToRight(string cargo_type)
    {
        int transfer_amount = LefthandCargo.FindChild(cargo_type).GetComponentInChildren<NumericUpDown>().Value;

        ShipA.Cargo.TransferTo(ShipB.Cargo, cargo_type, transfer_amount);
    }

    public void TransferRightToLeft(string cargo_type)
    {
        int transfer_amount = RighthandCargo.FindChild(cargo_type).GetComponentInChildren<NumericUpDown>().Value;

        ShipB.Cargo.TransferTo(ShipA.Cargo, cargo_type, transfer_amount);
    }
}
