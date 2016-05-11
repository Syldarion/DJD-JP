using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PortShopManager : MonoBehaviour
{
    [HideInInspector] public static PortShopManager Instance;

    public Port CurrentPort;
    public Fleet DockedFleet;
    public Ship SelectedShip;

    public RectTransform ActivePanel;
    public RectTransform FleetResourceList;
    public RectTransform FleetShipyardList;
    public RectTransform PortResourceList;
    public RectTransform PortShipyardList;
    public Button ResourceButtonPrefab;
    public RectTransform ResourceTileList;
    public Dropdown ShipSelection;
    public Text[] ResourcePriceTexts;
    public Text[] FleetGoldTexts;
    public Text[] PortGoldTexts;
    public SelectionGroup ShipyardFleetSelection;
    public SelectionGroup ShipyardPortSelection;

    public ShipStatBlock StatBlockPrefab;

    void Start()
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

        PanelUtilities.ActivatePanel(GetComponent<CanvasGroup>());

        DockedFleet = fleet_to_dock;
        SelectedShip = DockedFleet.Ships[0];

        PopulateShipDropdown();
        PopulatePortMarket();
        PopulateShipResources();
        PopulateResourcePrices();
        PopulatePortShipyard();
        PopulatePlayerShipyard(fleet_to_dock);
        PopulateResourceList();
        UpdateGoldText();
    }

    public void CloseShop()
    {
        PlayerScript.MyPlayer.OpenUI = null;

        PanelUtilities.DeactivatePanel(GetComponent<CanvasGroup>());

        ClearPortManager();
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
        foreach (var player_fleet_ship in DockedFleet.Ships.Where(player_fleet_ship => player_fleet_ship.Name == ShipSelection.options[new_value].text))
            SelectedShip = player_fleet_ship.GetComponent<Ship>();

        PopulateShipResources();
    }

    /// <summary>
    ///     Populates the ship list
    /// </summary>
    public void PopulateShipDropdown()
    {
        ShipSelection.ClearOptions();

        var dropdown_options = DockedFleet.Ships.Select(player_fleet_ships => new Dropdown.OptionData(player_fleet_ships.name)).ToList();

        ShipSelection.AddOptions(dropdown_options);
    }

    /// <summary>
    ///     Populates ship side of market
    /// </summary>
    public void PopulateShipResources()
    {
        string[] resource_types = {"Food", "Goods", "Sugar", "Spice", "Luxuries"};
        foreach (var s in resource_types)
        {
            var value = SelectedShip.Cargo.GetCargoAmount(s);

            FleetResourceList.FindChild(string.Format("{0}/Text", s)).GetComponent<Text>().text =
                string.Format("{0}\n{1}", s, value);
            FleetResourceList.FindChild(string.Format("{0}/Quantity", s)).GetComponent<NumericUpDown>().UpdateValue(0);
            FleetResourceList.FindChild(string.Format("{0}/Quantity", s))
                .GetComponent<NumericUpDown>()
                .SetMaxValue(value);
        }
    }

    /// <summary>
    ///     Populates the market with the inventory of CurrentPort
    /// </summary>
    public void PopulatePortMarket()
    {
        string[] resource_types = {"Food", "Goods", "Sugar", "Spice", "Luxuries"};
        foreach (var s in resource_types)
        {
            var value = CurrentPort.Market.GetCargoAmount(s);

            PortResourceList.FindChild(string.Format("{0}/Text", s)).GetComponent<Text>().text =
                string.Format("{0}\n{1}", s, value);
            PortResourceList.FindChild(string.Format("{0}/Quantity", s)).GetComponent<NumericUpDown>().UpdateValue(0);
            PortResourceList.FindChild(string.Format("{0}/Quantity", s))
                .GetComponent<NumericUpDown>()
                .SetMaxValue(value);
        }
    }

    /// <summary>
    ///     Populates the ship list the player has available to sell
    /// </summary>
    /// <param name="fleet">Fleet to populate the list with</param>
    public void PopulatePlayerShipyard(Fleet fleet)
    {
        foreach (Transform child in FleetShipyardList.transform) Destroy(child.gameObject);

        foreach (var player_ship in fleet.Ships)
        {
            var player_ship_stat_block = Instantiate(StatBlockPrefab).GetComponent<ShipStatBlock>();

            player_ship_stat_block.PopulateStatBlock(player_ship);
            player_ship_stat_block.transform.SetParent(FleetShipyardList.transform, false);
        }
    }

    public void PopulatePortShipyard()
    {
        foreach (Transform child in PortShipyardList.transform) Destroy(child.gameObject);

        foreach (var forsale in CurrentPort.Shipyard.Ships)
        {
            var port_ship_stat_block = Instantiate(StatBlockPrefab).GetComponent<ShipStatBlock>();

            port_ship_stat_block.PopulateStatBlock(forsale);
            port_ship_stat_block.transform.SetParent(FleetShipyardList.transform, false);
        }
    }

    public void PopulateResourceList()
    {
        foreach (Transform child in ResourceTileList.transform) Destroy(child.gameObject);

        foreach (var generator in CurrentPort.Resources)
        {
            var new_button = Instantiate(ResourceButtonPrefab);
            new_button.transform.SetParent(ResourceTileList, false);
            new_button.GetComponent<ResourceButton>().PopulateButton(generator);
            new_button.onClick.AddListener(generator.Purchase);
        }
    }

    public void PopulateResourcePrices()
    {
        for(var i = 0; i < ResourcePriceTexts.Length; i++)
        {
            ResourcePriceTexts[i].text = CurrentPort.PortPrices[i].ToString();
        }
    }

    public void ClearPortManager()
    {
        var children = (from Transform child in FleetShipyardList.transform select child.gameObject).ToList();
        children.AddRange(from Transform child in PortShipyardList.transform select child.gameObject);
        children.AddRange(from Transform child in ResourceTileList.transform select child.gameObject);
        foreach (var go in children) Destroy(go);
    }

    public void SellResources()
    {
        string[] resource_types = {"Food", "Goods", "Sugar", "Spice", "Luxuries"};

        for(var i = 0; i < resource_types.Length; i++)
        {
            var sell_amount =
                FleetResourceList.FindChild(string.Format("{0}/Quantity", resource_types[i])).GetComponent<NumericUpDown>().Value;

            var transaction_price = sell_amount * CurrentPort.PortPrices[i];

            if (CurrentPort.PortGold < transaction_price) continue;

            SelectedShip.Cargo.TransferTo(ref CurrentPort.Market, resource_types[i], sell_amount);
            DockedFleet.FleetGold += transaction_price;
            CurrentPort.PortGold -= transaction_price;
        }

        PopulateShipResources();
        PopulatePortMarket();
        UpdateGoldText();
    }

    public void BuyResources()
    {
        string[] resource_types = {"Food", "Goods", "Sugar", "Spice", "Luxuries"};

        for(var i = 0; i < resource_types.Length; i++)
        {
            var buy_amount =
                PortResourceList.FindChild(string.Format("{0}/Quantity", resource_types[i])).GetComponent<NumericUpDown>().Value;

            var transaction_price = buy_amount * CurrentPort.PortPrices[i];

            if (DockedFleet.FleetGold < transaction_price) continue;

            CurrentPort.Market.TransferTo(ref SelectedShip.Cargo, resource_types[i], buy_amount);
            DockedFleet.FleetGold -= transaction_price;
            CurrentPort.PortGold += transaction_price;
        }

        PopulateShipResources();
        PopulatePortMarket();
        UpdateGoldText();
    }
    
    public void BuyShip()
    {
        foreach (var ship in ShipyardPortSelection.SelectedObjects)
        {
            var ship_price = ship.GetComponent<ShipStatBlock>().ReferenceShip.Price;

            if (DockedFleet.FleetGold < ship_price) continue;

            DockedFleet.CmdAddShip(ship.GetComponent<ShipStatBlock>().ReferenceShip.name);
            CurrentPort.Shipyard.CmdRemoveShip(ship.GetComponent<ShipStatBlock>().ReferenceShip.name);

            DockedFleet.FleetGold -= ship_price;
            CurrentPort.PortGold += ship_price;
        }

        PopulatePlayerShipyard(DockedFleet);
        PopulatePortShipyard();
        UpdateGoldText();
    }


    public void SellShip()
    {
        foreach (var ship in ShipyardFleetSelection.SelectedObjects)
        {
            var ship_price = ship.GetComponent<ShipStatBlock>().ReferenceShip.Price;

            if (CurrentPort.PortGold < ship_price) continue;

            DockedFleet.CmdRemoveShip(ship.GetComponent<ShipStatBlock>().ReferenceShip.name);
            CurrentPort.Shipyard.CmdAddShip(ship.GetComponent<ShipStatBlock>().ReferenceShip.name);

            DockedFleet.FleetGold += ship_price;
            CurrentPort.PortGold -= ship_price;
        }

        PopulatePlayerShipyard(DockedFleet);
        PopulatePortShipyard();
        UpdateGoldText();
    }

    public void HireCrew()
    {
    }

    public void Tavern(int gold_to_spend)
    {
        var bar_tab = 0;

        var all_crew = DockedFleet.Ships.Sum(ship => ship.CurrentCrew);

        bar_tab = all_crew*gold_to_spend;

        if (bar_tab < PlayerScript.MyPlayer.TotalGold)
        {
            foreach (var ship in DockedFleet.Ships)
            {
                ship.CrewMorale += ship.CurrentCrew*gold_to_spend;
            }
        }

        PopulatePortMarket();
        PopulateShipResources();
    }

    public void UpdateGoldText()
    {
        foreach (var text in FleetGoldTexts)
            text.text = DockedFleet.FleetGold.ToString();
        foreach (var text in PortGoldTexts)
            text.text = CurrentPort.PortGold.ToString();
    }
}