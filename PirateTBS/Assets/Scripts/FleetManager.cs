using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FleetManager : MonoBehaviour
{
    [HideInInspector]
    public static FleetManager Instance;

    public GameObject ShipStatBlockPrefab;
    public RectTransform FleetAList;
    public RectTransform FleetBList;

    Fleet FleetA, FleetB;

	void Start()
	{
        Instance = this;
	}

	void Update()
	{
		
	}

    /// <summary>
    /// Opens the fleet management panel, and populates it with the fleet information
    /// </summary>
    /// <param name="fleet_a">The fleet initializing the manager</param>
    /// <param name="fleet_b">The fleet being moved onto</param>
    public void PopulateFleetManager(Fleet fleet_a, Fleet fleet_b)
    {
        if (fleet_a.Ships.Count < 1 || fleet_b.Ships.Count < 1 || fleet_a == fleet_b)
            return;

        PlayerScript.MyPlayer.UIOpen = true;

        PanelUtilities.ActivatePanel(GetComponent<CanvasGroup>());

        GameObject.Find("FleetAName").GetComponentInChildren<Text>().text = fleet_a.name;
        GameObject.Find("FleetBName").GetComponentInChildren<Text>().text = fleet_b.name;

        foreach(Ship s in fleet_a.Ships)
        {
            GameObject new_stat_block = Instantiate(ShipStatBlockPrefab);
            new_stat_block.transform.SetParent(FleetAList, false);
            new_stat_block.GetComponent<ShipStatBlock>().PopulateStatBlock(s);
        }
        foreach(Ship s in fleet_b.Ships)
        {
            GameObject new_stat_block = Instantiate(ShipStatBlockPrefab);
            new_stat_block.transform.SetParent(FleetBList, false);
            new_stat_block.GetComponent<ShipStatBlock>().PopulateStatBlock(s);
        }
    }

    /// <summary>
    /// Closes the fleet manager, wiping the list
    /// </summary>
    public void CloseFleetManager()
    {
        PlayerScript.MyPlayer.UIOpen = false;

        Transform FleetAContent = GameObject.Find("FleetAShipsContent").transform;
        Transform FleetBContent = GameObject.Find("FleetBShipsContent").transform;

        for (int i = 0; i < FleetAContent.childCount; i++)
            Destroy(FleetAContent.GetChild(i).gameObject);
        for (int i = 0; i < FleetBContent.childCount; i++)
            Destroy(FleetBContent.GetChild(i).gameObject);

        PanelUtilities.DeactivatePanel(GetComponent<CanvasGroup>());
    }

    /// <summary>
    /// Transfers the ship to the given fleet, if the other fleet has it
    /// </summary>
    /// <param name="fleet_from">Fleet to move the ship from</param>
    /// <param name="fleet_to">Fleet to move the ship to</param>
    /// <param name="ship">Ship to transfer</param>
    public void TransferShip(Fleet fleet_from, Fleet fleet_to, Ship ship)
    {
        if(fleet_from.Ships.Contains(ship))
        {
            fleet_to.AddShip(ship);
            fleet_from.CmdRemoveShip(ship.name);
        }
    }
}