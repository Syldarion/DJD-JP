using UnityEngine;
using UnityEngine.UI;
using System;
using System.Reflection;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class PortShopManager : MonoBehaviour
{
    [HideInInspector]
    public static PortShopManager Instance;

    public Port CurrentPort;
    public Fleet DockedFleet;
    public Ship SelectedShip;
    public Dropdown ShipSelection;
    public ShipStatBlock StatBlockPrefab;

    public RectTransform ActivePanel;
    public RectTransform FleetResourceList;
    public RectTransform PortResourceList;
    public RectTransform FleetShipyardList;
    public RectTransform PortShipyardList;

    public void Start()
    {
        Instance = this;
    }

    void Update()
    {

    }

    public void OpenShop(Fleet fleet_to_dock)
    {
        if (fleet_to_dock.Ships.Count < 1)
            return;

        PlayerScript.MyPlayer.OpenUI = GetComponent<CanvasGroup>();

        DockedFleet = fleet_to_dock;
        SelectedShip = DockedFleet.Ships[0];
        PopulatePlayerShipyard(fleet_to_dock);

        PanelUtilities.ActivatePanel(GetComponent<CanvasGroup>());

        PopulateShipDropdown();
        PopulatePortMarket();
        PopulateShipResources();
        PopulatePortShipyard();
    }

    public void CloseShop()
    {
        PlayerScript.MyPlayer.OpenUI = null;

        PanelUtilities.DeactivatePanel(GetComponent<CanvasGroup>());
    }

    public void SwitchTo(RectTransform new_panel)
    {
        if (!ActivePanel || ActivePanel == new_panel || !new_panel.GetComponent<CanvasGroup>())
            return;

        PanelUtilities.DeactivatePanel(ActivePanel.GetComponent<CanvasGroup>());

        ActivePanel = new_panel;

        PanelUtilities.ActivatePanel(ActivePanel.GetComponent<CanvasGroup>());
    }

    public void OnShipDropdownChange(int new_value)
    {
        foreach (Ship PlayerFleetShip in DockedFleet.Ships)
            if (PlayerFleetShip.Name == ShipSelection.options[new_value].text)
                SelectedShip = PlayerFleetShip.GetComponent<Ship>();

        PopulateShipResources();
    }

    /// <summary>
    /// Populates the ship list
    /// </summary>
    public void PopulateShipDropdown()
    {
        ShipSelection.ClearOptions();

        List<Dropdown.OptionData> dropdown_options = new List<Dropdown.OptionData>();

        foreach (Ship PlayerFleetShips in DockedFleet.Ships)
        {
            dropdown_options.Add(new Dropdown.OptionData(PlayerFleetShips.name));
        }

        ShipSelection.AddOptions(dropdown_options);
    }

    /// <summary>
    /// Populates ship side of market
    /// </summary>
    /// <param name="ship_name">Ship to sell resources from</param>
    public void PopulateShipResources()
    {
        string[] resource_types = { "Food", "Goods", "Sugar", "Spice", "Luxuries" };
        foreach(string s in resource_types)
        {
            Type cargo_type = typeof(Cargo);
            FieldInfo field = cargo_type.GetField(s);
            int value = (int)field.GetValue(SelectedShip.Cargo);

            FleetResourceList.FindChild(string.Format("{0}/Text", s)).GetComponent<Text>().text = string.Format("{0}\n{1}", s, value);
            FleetResourceList.FindChild(string.Format("{0}/Quantity", s)).GetComponent<NumericUpDown>().UpdateValue(0);
            FleetResourceList.FindChild(string.Format("{0}/Quantity", s)).GetComponent<NumericUpDown>().SetMaxValue(value);
        }
    }

    /// <summary>
    /// Populates the market with the inventory of CurrentPort
    /// </summary>
    public void PopulatePortMarket()
    {
        string[] resource_types = { "Food", "Goods", "Sugar", "Spice", "Luxuries" };
        foreach (string s in resource_types)
        {
            Type cargo_type = typeof(Cargo);
            FieldInfo field = cargo_type.GetField(s);
            int value = (int)field.GetValue(CurrentPort.Market);

            PortResourceList.FindChild(string.Format("{0}/Text", s)).GetComponent<Text>().text = string.Format("{0}\n{1}", s, value);
            PortResourceList.FindChild(string.Format("{0}/Quantity", s)).GetComponent<NumericUpDown>().UpdateValue(0);
            PortResourceList.FindChild(string.Format("{0}/Quantity", s)).GetComponent<NumericUpDown>().SetMaxValue(value);
        }
    }

    /// <summary>
    /// Populates the ship list the player has available to sell
    /// </summary>
    /// <param name="fleet">Fleet to populate the list with</param>
    public void PopulatePlayerShipyard(Fleet fleet)
    {
        foreach (Ship playerShip in fleet.Ships)
        {
            ShipStatBlock playerShipStatBlock = Instantiate(StatBlockPrefab).GetComponent<ShipStatBlock>();

            playerShipStatBlock.PopulateStatBlock(playerShip);
            playerShipStatBlock.transform.SetParent(FleetShipyardList.transform, false);
        }
    }
    public void PopulatePortShipyard()
    {
        foreach (Ship forsale in CurrentPort.Shipyard)
        {
            ShipStatBlock portShipStatBlock = Instantiate(StatBlockPrefab).GetComponent<ShipStatBlock>();

            portShipStatBlock.PopulateStatBlock(forsale);
            portShipStatBlock.transform.SetParent(FleetShipyardList.transform, false);
        }
    }

    public void SellResources(string resource)
    {
        int sell_amount = FleetResourceList.FindChild(string.Format("{0}/Quantity", resource)).GetComponent<NumericUpDown>().Value;
        SelectedShip.Cargo.TransferTo(CurrentPort.Market, resource, sell_amount);

        PopulateShipResources();
        PopulatePortMarket();
    }

    public void BuyResources(string resource)
    {
        int buy_amount = PortResourceList.FindChild(string.Format("{0}/Quantity", resource)).GetComponent<NumericUpDown>().Value;
        CurrentPort.Market.TransferTo(SelectedShip.Cargo, resource, buy_amount);

        PopulateShipResources();
        PopulatePortMarket();
    }
}