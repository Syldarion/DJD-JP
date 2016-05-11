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
    public Button ResourceButtonPrefab;

    public RectTransform ActivePanel;
    public RectTransform FleetResourceList;
    public RectTransform PortResourceList;
    public RectTransform FleetShipyardList;
    public RectTransform PortShipyardList;
    public RectTransform ResourceList;

    public SelectionGroup ShipyardFleetSelection;
    public SelectionGroup ShipyardPortSelection;

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
        PopulateResourceList();
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
        foreach (Ship forsale in CurrentPort.Shipyard.Ships)
        {
            ShipStatBlock portShipStatBlock = Instantiate(StatBlockPrefab).GetComponent<ShipStatBlock>();

            portShipStatBlock.PopulateStatBlock(forsale);
            portShipStatBlock.transform.SetParent(FleetShipyardList.transform, false);
        }
    }

    public void PopulateResourceList()
    {
        foreach(ResourceGenerator generator in CurrentPort.Resources)
        {
            Button new_button = Instantiate(ResourceButtonPrefab);
            new_button.transform.SetParent(ResourceList, false);
            new_button.GetComponent<ResourceButton>().PopulateButton(generator);
            new_button.onClick.AddListener(generator.Purchase);
        }
    }

    public void ClearShipyard()
    {
        var children = new List<GameObject>();
        foreach (Transform child in FleetShipyardList.transform) children.Add(child.gameObject);
        foreach (Transform child in PortShipyardList.transform) children.Add(child.gameObject);
        foreach (Transform child in ResourceList.transform) children.Add(child.gameObject);
        foreach (GameObject go in children) Destroy(go);
    }

    public void SellResources()
    {
        string[] resource_types = { "Food", "Goods", "Sugar", "Spice", "Luxuries" };

        foreach (string resource in resource_types)
        {
            int sell_amount = FleetResourceList.FindChild(string.Format("{0}/Quantity", resource)).GetComponent<NumericUpDown>().Value;
            SelectedShip.Cargo.TransferTo(CurrentPort.Market, resource, sell_amount);

            Debug.Log(string.Format("{0} -> {1} -> {2}", SelectedShip.Cargo.Food, sell_amount, CurrentPort.Market.Food));
        }

        PopulateShipResources();
        PopulatePortMarket();
    }

    public void BuyResources()
    {
        string[] resource_types = { "Food", "Goods", "Sugar", "Spice", "Luxuries" };

        foreach (string resource in resource_types)
        {
            int buy_amount = PortResourceList.FindChild(string.Format("{0}/Quantity", resource)).GetComponent<NumericUpDown>().Value;
            CurrentPort.Market.TransferTo(SelectedShip.Cargo, resource, buy_amount);
        }

        PopulateShipResources();
        PopulatePortMarket();
    }

    public void BuyShip()
    {
        int transaction_total = 0;

        foreach (GameObject ship in ShipyardPortSelection.SelectedObjects)
        {
            transaction_total += ship.GetComponent<ShipStatBlock>().ReferenceShip.Price;
        }

        if (transaction_total < PlayerScript.MyPlayer.TotalGold)
        {
            foreach (GameObject ship in ShipyardPortSelection.SelectedObjects)
            {
                DockedFleet.CmdAddShip(ship.GetComponent<ShipStatBlock>().ReferenceShip.name);
                CurrentPort.Shipyard.CmdRemoveShip(ship.GetComponent<ShipStatBlock>().ReferenceShip.name);
            }
        }
    }


    public void SellShip()
    {
        int transaction_total = 0;

        foreach (GameObject ship in ShipyardFleetSelection.SelectedObjects)
        {
            transaction_total += ship.GetComponent<ShipStatBlock>().ReferenceShip.Price;
        }

        foreach (GameObject ship in ShipyardFleetSelection.SelectedObjects)
        {
            DockedFleet.CmdRemoveShip(ship.GetComponent<ShipStatBlock>().ReferenceShip.name);
            CurrentPort.Shipyard.CmdAddShip(ship.GetComponent<ShipStatBlock>().ReferenceShip.name);
        }

        if (transaction_total < CurrentPort.Market.Gold)
            PlayerScript.MyPlayer.TotalGold += transaction_total;
        else
            PlayerScript.MyPlayer.TotalGold += CurrentPort.Market.Gold;
    }

    void CompletePurchaseTransaction(int transAmount)
    {
        int remainingGold = PlayerScript.MyPlayer.TotalGold - transAmount;     // How much gold does the player have left

        int pership = remainingGold / DockedFleet.Ships.Count;      // How much should go in each ship
        int remainderpership = remainingGold % DockedFleet.Ships.Count;     // Is there any left over

        CurrentPort.Market.Gold += transAmount;      // Add the gold spent to the port

        foreach (Ship ship in DockedFleet.Ships)
        {
            ship.Cargo.Gold = pership;      // Give each ship their portion of the gold the player has left
        }
        if (remainderpership > 0)      // If there is a remainder...
        {
            for (int i = 0; i < DockedFleet.Ships.Count && remainderpership > 0; ++i, --remainderpership)   // ...start at the beginning of the 
            {                                                                                               // fleet and toss one coin on each 
                ++DockedFleet.Ships[i].Cargo.Gold;                                                          // ship until there isn't any left
            }
        }
    }

    public void HireCrew()
    {

    }

    public void Tavern(int goldToSpend)
    {
        int allCrew = 0;
        int barTab = 0;

        foreach (Ship ship in DockedFleet.Ships)
        {
            allCrew += ship.CurrentCrew;
        }

        barTab = allCrew * goldToSpend;

        if (barTab < PlayerScript.MyPlayer.TotalGold)
        {
            CompletePurchaseTransaction(barTab);

            foreach (Ship ship in DockedFleet.Ships)
            {
                ship.CrewMorale += ship.CurrentCrew * goldToSpend;
            }
        }

        PopulatePortMarket();
        PopulateShipResources();
    }
}