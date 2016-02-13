using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PortShopManager : MonoBehaviour
{
    public Port CurrentPort;
    public Fleet DockedFleet;

    public RectTransform ActivePanel;
    public RectTransform FleetResourceList;
    public RectTransform PortResourceList;
    public RectTransform FleetShipyardList;
    public RectTransform PortShipyardList;

    public Dropdown FleetShipDropdown;

	void Start()
	{

	}
	
	void Update()
	{

	}

    public void OpenShop(Fleet fleet_to_dock)
    {
        DockedFleet = fleet_to_dock;

        GetComponent<CanvasGroup>().alpha = 1;
        GetComponent<CanvasGroup>().interactable = true;
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        PopulateShipDropdown();
        PopulatePortMarket();
    }

    public void CloseShop()
    {
        GetComponent<CanvasGroup>().alpha = 0;
        GetComponent<CanvasGroup>().interactable = false;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void SwitchTo(RectTransform new_panel)
    {
        if (!ActivePanel || ActivePanel == new_panel || !new_panel.GetComponent<CanvasGroup>())
            return;

        ActivePanel.GetComponent<CanvasGroup>().alpha = 0;
        ActivePanel.GetComponent<CanvasGroup>().interactable = false;
        ActivePanel.GetComponent<CanvasGroup>().blocksRaycasts = false;

        ActivePanel = new_panel;

        ActivePanel.GetComponent<CanvasGroup>().alpha = 1;
        ActivePanel.GetComponent<CanvasGroup>().interactable = true;
        ActivePanel.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    /// <summary>
    /// Populates the ship list
    /// </summary>
    public void PopulateShipDropdown()
    {
        //Use the ship list on DockedFleet to populate the list of ships
        //Make sure the dropdown is filled with the names of the ships
        //This is because PopulateShipResources will use the name of the ship to find its info

        FleetShipDropdown.ClearOptions();

        List<Dropdown.OptionData> dropdown_options = new List<Dropdown.OptionData>();
        foreach (Ship s in DockedFleet.Ships)
            dropdown_options.Add(new Dropdown.OptionData(s.Name));
        FleetShipDropdown.AddOptions(dropdown_options);
    }

    /// <summary>
    /// Populates ship side of market
    /// </summary>
    /// <param name="ship_name">Ship to sell resources from</param>
    public void PopulateShipResources(string ship_name)
    {
        //This function will probably be called when the ship dropdown value changes
        //Find ship in DockedFleet ships that has the given name, get the Ship component, and use the cargo from it
        //For the text that says how much the port has, probably use something like this
        //string.Format("Food\n{0}", ship.Cargo.Food);
        //Call SetMaxValue(int value) on the associated NumericUpDown control, passing the amount that the ship has

        //If you want to access the text of the section to set it with the above string, all you have to do is this
        //FleetResourceList.transform.FindChild("Food").GetComponentInChildren<Text>().text
        //Just assign the string to that for each of the resources types
    }

    /// <summary>
    /// Populates the market with the inventory of CurrentPort
    /// </summary>
    public void PopulatePortMarket()
    {
        //Use CurrentPort
        //Just grab the values inside the port's inventory
        //Text values should be like this
        //string.Format("Food\n{0}", port.Market.Food);
        //Call SetMaxValue(int value) on the associated NumericUpDown control, passing the amount that the port has

        //If you want to access the text of the section to set it with the above string, all you have to do is this
        //PortResourceList.transform.FindChild("Food").GetComponentInChildren<Text>().text
        //Just assign the string to that for each of the resources types
    }

    /// <summary>
    /// Populates the ship list the player has available to sell
    /// </summary>
    /// <param name="fleet">Fleet to populate the list with</param>
    public void PopulatePlayerShipyard(Fleet fleet)
    {
        //Instantiate a new ShipStatBlock for each ship in the fleet
        //Set the block's parent to the player ship list content panel

        //The parent to set to is just
        //FleetShipyardList.transform
    }

    /// <summary>
    /// Populates the port ship list with the ships inside CurrentPort
    /// </summary>
    public void PopulatePortShipyard()
    {
        //Use CurrentPort
        //Instantiate a new ShipStatBlock for each ship the port has
        //Set the block's parent to the port ship list content panel

        //The parent to set to is just
        //PortShipyardList.transform
    }
}
