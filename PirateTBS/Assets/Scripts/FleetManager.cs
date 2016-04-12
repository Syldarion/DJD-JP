using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class FleetManager : MonoBehaviour
{
    [HideInInspector]
    public static FleetManager Instance;

    public GameObject ShipStatBlockPrefab;          //Reference to prefab for instantiating ship stat blocks
    public RectTransform FleetAList;                //Reference to list of ships in left-hand fleet
    public RectTransform FleetBList;                //Reference to list of ships in right-hand fleet

    public Text FleetAName;                         //Reference to text showing left-hand fleet name
    public Text FleetBName;                         //Reference to text showing right-hand fleet name

    Fleet FleetA;                                   //Left-hand fleet
    Fleet FleetB;                                   //Right-hand fleet

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

        PlayerScript.MyPlayer.OpenUI = GetComponent<CanvasGroup>();

        PanelUtilities.ActivatePanel(GetComponent<CanvasGroup>());

        FleetA = fleet_a;
        FleetB = fleet_b;

        FleetAName.text = fleet_a.name;
        FleetBName.text = fleet_b.name;

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
        PlayerScript.MyPlayer.OpenUI = null;

        var children = new List<GameObject>();
        foreach (Transform child in FleetAList.transform) children.Add(child.gameObject);
        foreach (Transform child in FleetBList.transform) children.Add(child.gameObject);
        foreach (GameObject go in children) Destroy(go);

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
            fleet_to.CmdAddShip(ship.name);
            fleet_from.CmdRemoveShip(ship.name);
        }

        if (fleet_from.Ships.Count <= 0 || fleet_to.Ships.Count <= 0)
            CloseFleetManager();
    }

    /// <summary>
    /// Transfers selected ships from the left fleet to the right
    /// </summary>
    public void TransferLeftToRight()
    {
        foreach (GameObject ship in FleetAList.GetComponent<SelectionGroup>().SelectedObjects)
        {
            if (ship.GetComponent<ShipStatBlock>())
            {
                TransferShip(FleetA, FleetB, ship.GetComponent<ShipStatBlock>().ReferenceShip);
                ship.transform.SetParent(FleetBList, false);
            }
        }
    }

    /// <summary>
    /// Transfers selected ships from the right fleet to the left
    /// </summary>
    public void TransferRightToLeft()
    {
        foreach (GameObject ship in FleetBList.GetComponent<SelectionGroup>().SelectedObjects)
        {
            if (ship.GetComponent<ShipStatBlock>())
            {
                TransferShip(FleetB, FleetA, ship.GetComponent<ShipStatBlock>().ReferenceShip);
                ship.transform.SetParent(FleetAList, false);
            }
        }
    }
}