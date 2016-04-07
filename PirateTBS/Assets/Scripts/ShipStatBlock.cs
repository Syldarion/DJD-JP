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

    public Text ShipNameText;
    public Text ShipTypeText;

    public Text HealthText;
    public Text SpeedText;
    public Text CargoText;
    public Text CannonText;

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

    public void ActivateTooltip(string text)
    {
        Tooltip.EnableTooltip(true);
        Tooltip.UpdateTooltip(text);
    }

    public void DeactivateTooltip()
    {
        Tooltip.EnableTooltip(false);
    }
}