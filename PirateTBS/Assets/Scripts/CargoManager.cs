using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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
        resource_list.FindChild("Food\\Text").GetComponent<Text>().text = string.Format("Food\n{0}", ship.Cargo.Food);
        resource_list.FindChild("Food").GetComponentInChildren<NumericUpDown>().UpdateValue(0);
        resource_list.FindChild("Food").GetComponentInChildren<NumericUpDown>().SetMaxValue(ship.Cargo.Food);

        resource_list.FindChild("Goods\\Text").GetComponent<Text>().text = string.Format("Goods\n{0}", ship.Cargo.Goods);
        resource_list.FindChild("Goods").GetComponentInChildren<NumericUpDown>().UpdateValue(0);
        resource_list.FindChild("Goods").GetComponentInChildren<NumericUpDown>().SetMaxValue(ship.Cargo.Goods);

        resource_list.FindChild("Sugar\\Text").GetComponent<Text>().text = string.Format("Sugar\n{0}", ship.Cargo.Sugar);
        resource_list.FindChild("Sugar").GetComponentInChildren<NumericUpDown>().UpdateValue(0);
        resource_list.FindChild("Sugar").GetComponentInChildren<NumericUpDown>().SetMaxValue(ship.Cargo.Sugar);

        resource_list.FindChild("Spice\\Text").GetComponent<Text>().text = string.Format("Spice\n{0}", ship.Cargo.Spice);
        resource_list.FindChild("Spice").GetComponentInChildren<NumericUpDown>().UpdateValue(0);
        resource_list.FindChild("Spice").GetComponentInChildren<NumericUpDown>().SetMaxValue(ship.Cargo.Spice);

        resource_list.FindChild("Luxuries\\Text").GetComponent<Text>().text = string.Format("Luxuries\n{0}", ship.Cargo.Luxuries);
        resource_list.FindChild("Luxuries").GetComponentInChildren<NumericUpDown>().UpdateValue(0);
        resource_list.FindChild("Luxuries").GetComponentInChildren<NumericUpDown>().SetMaxValue(ship.Cargo.Luxuries);
    }

    public void TransferLeftToRight(string cargo_type)
    {
        int transfer_amount = LefthandCargo.FindChild(cargo_type).GetComponentInChildren<NumericUpDown>().Value;

        switch(cargo_type)
        {
            case "Food":
                ShipA.Cargo.TransferCargo(ShipB.Cargo, transfer_amount);
                break;
            case "Goods":
                ShipA.Cargo.TransferCargo(ShipB.Cargo, 0, 0, transfer_amount);
                break;
            case "Sugar":
                ShipA.Cargo.TransferCargo(ShipB.Cargo, 0, 0, 0, transfer_amount);
                break;
            case "Spice":
                ShipA.Cargo.TransferCargo(ShipB.Cargo, 0, 0, 0, 0, transfer_amount);
                break;
            case "Luxuries":
                ShipA.Cargo.TransferCargo(ShipB.Cargo, 0, 0, 0, 0, 0, transfer_amount);
                break;
            default:
                break;
        }
    }

    public void TransferRightToLeft(string cargo_type)
    {
        int transfer_amount = RighthandCargo.FindChild(cargo_type).GetComponentInChildren<NumericUpDown>().Value;

        switch (cargo_type)
        {
            case "Food":
                ShipB.Cargo.TransferCargo(ShipA.Cargo, transfer_amount);
                break;
            case "Goods":
                ShipB.Cargo.TransferCargo(ShipA.Cargo, 0, 0, transfer_amount);
                break;
            case "Sugar":
                ShipB.Cargo.TransferCargo(ShipA.Cargo, 0, 0, 0, transfer_amount);
                break;
            case "Spice":
                ShipB.Cargo.TransferCargo(ShipA.Cargo, 0, 0, 0, 0, transfer_amount);
                break;
            case "Luxuries":
                ShipB.Cargo.TransferCargo(ShipA.Cargo, 0, 0, 0, 0, 0, transfer_amount);
                break;
            default:
                break;
        }
    }
}
