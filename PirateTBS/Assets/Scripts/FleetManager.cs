using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FleetManager : MonoBehaviour
{
    public GameObject ShipStatBlockPrefab;

    Fleet FleetA, FleetB;

	void Start()
	{
		
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
        GetComponent<CanvasGroup>().alpha = 1;
        GetComponent<CanvasGroup>().interactable = true;
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        GameObject.Find("FleetAName").GetComponentInChildren<Text>().text = fleet_a.name;
        GameObject.Find("FleetBName").GetComponentInChildren<Text>().text = fleet_b.name;

        foreach(Ship s in fleet_a.Ships)
        {
            GameObject new_stat_block = Instantiate(ShipStatBlockPrefab);
            new_stat_block.transform.SetParent(GameObject.Find("FleetAShipsContent").transform, false);
            new_stat_block.GetComponent<ShipStatBlock>().PopulateStatBlock(s);
        }
        foreach(Ship s in fleet_b.Ships)
        {
            GameObject new_stat_block = Instantiate(ShipStatBlockPrefab);
            new_stat_block.transform.SetParent(GameObject.Find("FleetBShipsContent").transform, false);
            new_stat_block.GetComponent<ShipStatBlock>().PopulateStatBlock(s);
        }
    }

    /// <summary>
    /// Closes the fleet manager, wiping the list
    /// </summary>
    public void CloseFleetManager()
    {
        Transform FleetAContent = GameObject.Find("FleetAShipsContent").transform;
        Transform FleetBContent = GameObject.Find("FleetBShipsContent").transform;

        for (int i = 0; i < FleetAContent.childCount; i++)
            Destroy(FleetAContent.GetChild(i).gameObject);
        for (int i = 0; i < FleetBContent.childCount; i++)
            Destroy(FleetBContent.GetChild(i).gameObject);

        GetComponent<CanvasGroup>().alpha = 0;
        GetComponent<CanvasGroup>().interactable = false;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
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