using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System;

public delegate void OnClickDelegate();

public class ShipStatBlock : MonoBehaviour, IPointerClickHandler
{
    public OnClickDelegate StatBlockDelegate;           //Functions to call when the stat block is clicked
    public Ship ReferenceShip;                          //Ship this stat block is referring to

    public Text ShipNameText;                           //Reference to text showing name of ship
    public Text ShipTypeText;                           //Reference to text showing type of ship

    public Text HealthText;                             //Reference to text showing current health of ship
    public Text SpeedText;                              //Reference to text showing max speed of ship
    public Text CargoText;                              //Reference to text showing cargo space of ship
    public Text CannonText;                             //Reference to text showing cannon count of ship

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

        ShipNameText.text = ship.name;
        ShipTypeText.text = ship.ShipType;
        
        HealthText.text = string.Format("{0} | {1}", ship.HullHealth, ship.SailHealth);
        SpeedText.text = ship.FullSpeed.ToString();
        CargoText.text = string.Format("{0}/{1}", ship.Cargo.Size().ToString("F1"), ship.CargoSpace.ToString());
        CannonText.text = ship.Cannons.ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        AddToSelectionGroup();
        if (StatBlockDelegate != null)
            StatBlockDelegate.Invoke();
    }

    /// <summary>
    /// Adds this stat block to the nearest selection group
    /// </summary>
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

    /// <summary>
    /// Activates the tooltip
    /// </summary>
    /// <param name="text">Text to show on tooltip</param>
    public void ActivateTooltip(string text)
    {
        Tooltip.EnableTooltip(true);
        Tooltip.UpdateTooltip(text);
    }

    /// <summary>
    /// Deactivates the tooltip
    /// </summary>
    public void DeactivateTooltip()
    {
        Tooltip.EnableTooltip(false);
    }
}