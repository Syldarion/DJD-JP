using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PortShopManager : MonoBehaviour
{
    [HideInInspector] public static PortShopManager Instance;

    public Port CurrentPort;
    public Ship DockedShip;

    public RectTransform ActivePanel;
    public RectTransform ShipResourceList;
    public RectTransform PortResourceList;
    public Button ShipyardBuyButtonPrefab;
    public Text DockedShipPriceText;
    public RectTransform PortShipyardList;
    public Button ResourceButtonPrefab;
    public RectTransform ResourceTileList;
    public Text[] ResourcePriceTexts;
    public Text[] ShipGoldTexts;
    public Text[] PortGoldTexts;
    public Button CollectGoldButton;

    void Start()
    {
        Instance = this;
    }

    void Update()
    {
    }

    public void OpenShop(Ship ship_to_dock)
    {
        if (ship_to_dock == null)
            return;

        PlayerScript.MyPlayer.OpenUI = GetComponent<CanvasGroup>();

        PanelUtilities.ActivatePanel(GetComponent<CanvasGroup>());

        DockedShip = ship_to_dock;

        if (CurrentPort.GoldForPlayer > 0)
        {
            CollectGoldButton.gameObject.SetActive(true);
            CollectGoldButton.GetComponentInChildren<Text>().text = string.Format("Collect {0} Gold", CurrentPort.GoldForPlayer);
        }

        PopulatePortMarket();
        PopulateShipResources();
        PopulateResourcePrices();
        PopulateShipyard();
        PopulateResourceList();
        UpdateGoldText();
    }

    public void CloseShop()
    {
        PlayerScript.MyPlayer.OpenUI = null;

        PanelUtilities.DeactivatePanel(GetComponent<CanvasGroup>());

        CollectGoldButton.gameObject.SetActive(false);

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

    /// <summary>
    ///     Populates ship side of market
    /// </summary>
    public void PopulateShipResources()
    {
        string[] resource_types = {"Food", "Goods", "Sugar", "Spice", "Luxuries"};
        foreach (var s in resource_types)
        {
            var value = DockedShip.Cargo.GetCargoAmount(s);

            ShipResourceList.FindChild(string.Format("{0}/Text", s)).GetComponent<Text>().text =
                string.Format("{0}\n{1}", s, value);
            ShipResourceList.FindChild(string.Format("{0}/Quantity", s)).GetComponent<NumericUpDown>().UpdateValue(0);
            ShipResourceList.FindChild(string.Format("{0}/Quantity", s))
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

    public void PopulateResourcePrices()
    {
        for (var i = 0; i < ResourcePriceTexts.Length; i++)
        {
            ResourcePriceTexts[i].text = CurrentPort.PortPrices[i].ToString();
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

    public void PopulateShipyard()
    {
        DockedShipPriceText.text = string.Format("Sell Docked Ship ({0})", DockedShip.Price);

        foreach(Ship s in CurrentPort.Shipyard)
        {
            Button new_button = Instantiate(ShipyardBuyButtonPrefab);
            new_button.transform.SetParent(PortShipyardList, false);

            new_button.onClick.AddListener(() => BuyShip(s));

            new_button.GetComponentInChildren<Text>().text =
                string.Format("Buy {0} ({1})\n{2}\n{3} Hull / {4} Sail / {5} Cannons",
                s.Name, s.Price, s.ShipType, s.HullHealth, s.SailHealth, s.Cannons);
        }
    }

    public void ClearPortManager()
    {
        var children = (from Transform child in PortShipyardList.transform select child.gameObject).ToList();
        children.AddRange(from Transform child in ResourceTileList.transform select child.gameObject);
        foreach (var go in children) Destroy(go);
    }

    public void SellResources()
    {
        string[] resource_types = {"Food", "Goods", "Sugar", "Spice", "Luxuries"};

        for(var i = 0; i < resource_types.Length; i++)
        {
            var sell_amount =
                ShipResourceList.FindChild(string.Format("{0}/Quantity", resource_types[i])).GetComponent<NumericUpDown>().Value;

            var transaction_price = sell_amount * CurrentPort.PortPrices[i];

            if (CurrentPort.PortGold < transaction_price) continue;

            DockedShip.Cargo.TransferTo(ref CurrentPort.Market, resource_types[i], sell_amount);
            DockedShip.Gold += transaction_price;
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

            if (DockedShip.Gold < transaction_price) continue;

            CurrentPort.Market.TransferTo(ref DockedShip.Cargo, resource_types[i], buy_amount);
            DockedShip.Gold -= transaction_price;
            CurrentPort.PortGold += transaction_price;
        }

        PopulateShipResources();
        PopulatePortMarket();
        UpdateGoldText();
    }
    
    public void BuyShip(Ship ship_to_sell)
    {
        if (DockedShip.Gold < ship_to_sell.Price) return;

        StartCoroutine(WaitForBoughtShip(PlayerScript.MyPlayer.Ships.Count, ship_to_sell));
        PlayerScript.MyPlayer.CmdSpawnShip(ship_to_sell.Name, CurrentPort.SpawnTile.HexCoord.Q, CurrentPort.SpawnTile.HexCoord.R, 0);

        DockedShip.Gold -= ship_to_sell.Price;

        UpdateGoldText();
    }

    IEnumerator WaitForBoughtShip(int old_count, Ship copy_ship)
    {
        while (PlayerScript.MyPlayer.Ships.Count <= old_count)
            yield return null;

        PlayerScript.MyPlayer.Ships[old_count].CopyShip(copy_ship);
    }

    public void ConfirmSellShip()
    {
        if (PlayerScript.MyPlayer.Ships.Count > 1)
        {
            DialogueBox.CurrentDialogue.NewDialogue("Are you sure you want to sell your docked ship?");
            DialogueBox.CurrentDialogue.AddOption("Yes", () => { SellShip(); DialogueBox.CurrentDialogue.CloseDialogue(); });
            DialogueBox.CurrentDialogue.AddOption("No", () => DialogueBox.CurrentDialogue.CloseDialogue());
        }
        else
            DialogueBox.CurrentDialogue.NewDialogue("You cannot sell your last ship.", 1.0f);
    }

    public void SellShip()
    {
        int resource_value = 0;
        resource_value += DockedShip.Cargo.Food * CurrentPort.PortPrices[0];
        resource_value += DockedShip.Cargo.Goods * CurrentPort.PortPrices[1];
        resource_value += DockedShip.Cargo.Sugar * CurrentPort.PortPrices[2];
        resource_value += DockedShip.Cargo.Spice * CurrentPort.PortPrices[3];
        resource_value += DockedShip.Cargo.Luxuries * CurrentPort.PortPrices[4];

        CurrentPort.GoldForPlayer += DockedShip.Price + DockedShip.Gold + resource_value;
        CloseShop();

        PlayerScript.MyPlayer.RemoveShip(DockedShip);

        NetworkServer.Destroy(DockedShip.gameObject);
    }

    public void CollectGold()
    {
        DockedShip.Gold += CurrentPort.GoldForPlayer;
        CurrentPort.GoldForPlayer = 0;
        CollectGoldButton.gameObject.SetActive(false);

        UpdateGoldText();
    }

    public void CollectResources()
    {
        foreach (ResourceGenerator generator in CurrentPort.Resources)
        {
            if (generator.Owner == PlayerScript.MyPlayer)
            {
                generator.GeneratedResources.TransferTo(ref DockedShip.Cargo, "Food", generator.GeneratedResources.Food);
                generator.GeneratedResources.TransferTo(ref DockedShip.Cargo, "Sugar", generator.GeneratedResources.Sugar);
                generator.GeneratedResources.TransferTo(ref DockedShip.Cargo, "Spice", generator.GeneratedResources.Spice);
            }
        }
    }

    public void HireCrew()
    {

    }

    public void Tavern(int gold_to_spend)
    {
        var bar_tab = 0;

        var all_crew = DockedShip.CurrentCrew;

        bar_tab = all_crew*gold_to_spend;

        if (bar_tab < PlayerScript.MyPlayer.TotalGold)
        {
            DockedShip.CrewMorale += DockedShip.CurrentCrew * gold_to_spend;
        }

        PopulatePortMarket();
        PopulateShipResources();
    }

    public void UpdateGoldText()
    {
        foreach (var text in ShipGoldTexts)
            text.text = DockedShip.Gold.ToString();
        foreach (var text in PortGoldTexts)
            text.text = CurrentPort.PortGold.ToString();
    }
}