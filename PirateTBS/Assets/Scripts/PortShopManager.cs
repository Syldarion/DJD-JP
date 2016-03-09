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

    public Text FleetGold;  // This won't work
    public Text PortGold;   // This won't work

    public RectTransform ActivePanel;
    public RectTransform FleetResourceList;
    public RectTransform PortResourceList;
    public RectTransform FleetShipyardList;
    public SelectionGroup FleetSelection;
    public RectTransform PortShipyardList;
    public SelectionGroup PortSelection;

    private int PlayersGold;

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

        PlayerScript.MyPlayer.UIOpen = true;

        DockedFleet = fleet_to_dock;
        SelectedShip = DockedFleet.Ships[0];
        PopulatePlayerShipyard(fleet_to_dock);

        PanelUtilities.ActivatePanel(GetComponent<CanvasGroup>());

        PopulateShipDropdown();
        PopulatePortMarket();
        PopulateShipResources();
        PopulatePortShipyard();
        PlayersGold = TotalPlayersGold();
    }

    public void CloseShop()
    {
        PlayerScript.MyPlayer.UIOpen = false;

        PanelUtilities.DeactivatePanel(GetComponent<CanvasGroup>());

        UnPopulateShipyard();
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
        foreach (string s in resource_types)
        {
            Type cargo_type = typeof(Cargo);
            FieldInfo field = cargo_type.GetField(s);
            int value = (int)field.GetValue(SelectedShip.Cargo);

            FleetResourceList.FindChild(string.Format("{0}/Text", s)).GetComponent<Text>().text = string.Format("{0}\n{1}", s, value);
            FleetResourceList.FindChild(string.Format("{0}/Quantity", s)).GetComponent<NumericUpDown>().UpdateValue(0);
            FleetResourceList.FindChild(string.Format("{0}/Quantity", s)).GetComponent<NumericUpDown>().SetMaxValue(value);
        }

        int total = 0;
        foreach (Ship ship in DockedFleet.Ships)
        {
            total += ship.Cargo.Gold;
        }

        FleetGold.text = string.Format("Gold: {0}", total);  // Can't use set references 'cause there are more than one panels with gold text
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

        PortGold.text = string.Format("Gold: {0}", CurrentPort.Market.Gold);  // Can't use set references 'cause there are more than one panels with gold text
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

    public void UnPopulateShipyard()
    {
        var children = new List<GameObject>();
        foreach (Transform child in FleetShipyardList.transform) children.Add(child.gameObject);
        foreach (Transform child in PortShipyardList.transform) children.Add(child.gameObject);
        foreach (GameObject go in children) Destroy(go);
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

    public void SellResources()
    {
        string[] resource_types = { "Food", "Goods", "Sugar", "Spice", "Luxuries" };

        foreach (string resource in resource_types)
        {
            int sell_amount = FleetResourceList.FindChild(string.Format("{0}/Quantity", resource)).GetComponent<NumericUpDown>().Value;
            SelectedShip.Cargo.TransferTo(CurrentPort.Market, resource, sell_amount);
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
        int tranTotal = 0;
                
        foreach (GameObject ship in PortSelection.SelectedObjects)
        {
            tranTotal += ship.GetComponent<ShipStatBlock>().ReferenceShip.Price;
        }

        if (tranTotal < PlayersGold)
        {
            foreach (GameObject ship in PortSelection.SelectedObjects)
            {
                DockedFleet.AddShip(ship.GetComponent<ShipStatBlock>().ReferenceShip);
                CurrentPort.Shipyard.Remove(ship.GetComponent<ShipStatBlock>().ReferenceShip);
            }

            CompletePurchaseTransaction(tranTotal);
        }
    }


    public void SellShip()
    {
        int tranTotal = 0;

        foreach (GameObject ship in FleetSelection.SelectedObjects)
        {
            tranTotal += ship.GetComponent<ShipStatBlock>().ReferenceShip.Price;
        }

        foreach (GameObject ship in FleetSelection.SelectedObjects)
        {
            DockedFleet.Ships.Remove(ship.GetComponent<ShipStatBlock>().ReferenceShip);
            CurrentPort.Shipyard.Add(ship.GetComponent<ShipStatBlock>().ReferenceShip);
        }

        if (tranTotal < CurrentPort.Market.Gold)
            PlayersGold += tranTotal;
        else
            PlayersGold += CurrentPort.Market.Gold;
    }

    public void HireCrew()
    {

    }

    public void Tavern( int goldToSpend )
    {
        int allCrew = 0;
        int barTab = 0;

        foreach (Ship ship in DockedFleet.Ships)
        {
            allCrew += ship.CurrentCrew;
        }

        barTab = allCrew * goldToSpend;

        if( barTab < PlayersGold )
        {
            CompletePurchaseTransaction(barTab);

            foreach (Ship ship in DockedFleet.Ships )
            {
                ship.CrewMorale += ship.CurrentCrew * goldToSpend;
            }
        }

        PopulatePortMarket();
        PopulateShipResources();
    }

    void CompletePurchaseTransaction( int transAmount )
    {
        int remainingGold = PlayersGold - transAmount;     // How much gold does the player have left

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

    int TotalPlayersGold(  )
    {
        int total = 0;

        foreach (Ship ship in DockedFleet.Ships)
        {
            total += ship.Cargo.Gold;
        }

        return total;
    }
}