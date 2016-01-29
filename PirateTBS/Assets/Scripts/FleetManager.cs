using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FleetManager : MonoBehaviour
{
    public GameObject ShipStatBlockPrefab;

    FleetScript FleetA, FleetB;

	void Start()
	{
		
	}

	void Update()
	{
		
	}

    public void PopulateFleetManager(FleetScript fleet_a, FleetScript fleet_b)
    {
        GameObject.Find("FleetAName").GetComponentInChildren<Text>().text = fleet_a.name;
        GameObject.Find("FleetBName").GetComponentInChildren<Text>().text = fleet_b.name;

        foreach(ShipScript s in fleet_a.Ships)
        {
            GameObject new_stat_block = Instantiate(ShipStatBlockPrefab);
            new_stat_block.transform.SetParent(GameObject.Find("FleetAShipsContent").transform, false);
            new_stat_block.GetComponent<ShipStatBlock>().PopulateStatBlock(s);
        }
        foreach(ShipScript s in fleet_b.Ships)
        {
            GameObject new_stat_block = Instantiate(ShipStatBlockPrefab);
            new_stat_block.transform.SetParent(GameObject.Find("FleetBShipsContent").transform, false);
            new_stat_block.GetComponent<ShipStatBlock>().PopulateStatBlock(s);
        }
    }

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

    public void TransferShip(FleetScript fleet_from, FleetScript fleet_to, ShipScript ship)
    {
        if(fleet_from.Ships.Contains(ship))
        {
            fleet_to.AddShip(ship);
            fleet_from.RemoveShip(ship);
        }
    }
}