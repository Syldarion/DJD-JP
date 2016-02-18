using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System;

public delegate void OnClickDelegate();

public class ShipStatBlock : MonoBehaviour, IPointerClickHandler
{
    public OnClickDelegate StatBlockDelegate;
    public Ship ReferenceShip;

	void Start()
	{
        StatBlockDelegate = null;
	}
	
	void Update()
	{

	}

    /// <summary>
    /// Populates the stat block with the ship's information
    /// </summary>
    /// <param name="ship">Ship to make a stat block for</param>
    public void PopulateStatBlock(Ship ship)
    {
        ReferenceShip = ship;

        transform.FindChild("ShipNameText").GetComponent<Text>().text = ship.name;
        transform.FindChild("ShipTypeText").GetComponent<Text>().text = ship.ShipType;

        Transform ship_stats_transform = transform.FindChild("ShipStats");
        ship_stats_transform.FindChild("HealthText").GetComponent<Text>().text = string.Format("{0} | {1}", ship.HullHealth, ship.SailHealth);
        ship_stats_transform.FindChild("CargoText").GetComponent<Text>().text = string.Format("{0}/{1}", ship.Cargo.Size().ToString("F1"), ship.CargoSpace.ToString());
        ship_stats_transform.FindChild("SpeedText").GetComponent<Text>().text = ship.FullSpeed.ToString();
        ship_stats_transform.FindChild("CannonText").GetComponent<Text>().text = ship.Cannons.ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        AddToSelectionGroup();
        if (StatBlockDelegate != null)
            StatBlockDelegate.Invoke();
    }

    public void AddToSelectionGroup()
    {
        SelectionGroup parent_group = GetComponentInParent<SelectionGroup>();

        if (!parent_group)
            return;

        if (parent_group.SelectedObjects.Contains(gameObject))
            parent_group.RemoveSelection(gameObject);
        else
            parent_group.AddSelection(gameObject);
    }
}