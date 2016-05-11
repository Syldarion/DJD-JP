using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Port : NetworkBehaviour
{
    [SyncVar]
    public string PortName;                     //Name of port
    public Nationality PortNationality;         //Associated nation of port
    public HexTile SpawnTile;                   //Tile fleets from port are spawned on
    [SyncVar] public int PortGold;

    [SyncVar]
    public Cargo Market;                        //Contents of port's marketplace
    public Fleet Shipyard;                      //Contents of port's shipyard
    public List<ResourceGenerator> Resources;   //List of resource generators belonging to the port

    public GameObject FleetPrefab;              //Fleet prefab for spawning new fleets
    public GameObject ResourceGeneratorPrefab;  //Generator prefab for spawning new resource generators

    public static int[] DefaultPortPrices = {10, 20, 30, 40, 80};
    public static int[] PortPriceMods = {2, 4, 8, 10, 20};

    public SyncListInt PortPrices = new SyncListInt();

    void Start()
    {
        GenerateResources();
    }

    void Update()
    {

    }

    /// <summary>
    /// Finds nearby water tile to set as spawn tile and initializes port market
    /// </summary>
    public void InitializePort()
    {
        foreach (HexCoordinate hc in GetComponentInParent<HexTile>().Directions)
        {
            HexTile neighbor = GetComponentInParent<HexTile>().GetNeighbor(hc);

            if (neighbor && neighbor.IsWater)
            {
                SpawnTile = GetComponentInParent<HexTile>().GetNeighbor(hc);
                break;
            }
        }

        Market = new Cargo(Random.Range(0, 500), Random.Range(0, 500), Random.Range(0, 500), Random.Range(0, 500), Random.Range(0, 500));

        PortGold = Random.Range(5000, 20000);

        PortPrices.Clear();

        for (var i = 0; i < 5; i++)
            PortPrices.Add(DefaultPortPrices[i] + Random.Range(-PortPriceMods[i], PortPriceMods[i]));
    }

    void GenerateResources()
    {
        Resources = new List<ResourceGenerator>();
        LandHex parent_hex = transform.parent.GetComponent<LandHex>();
        if (!parent_hex)
            return;

        foreach(HexCoordinate hc in parent_hex.Directions)
        {
            HexTile neighbor = parent_hex.GetNeighbor(hc);
            if (neighbor && !neighbor.IsWater)
            {
                if (neighbor.transform.childCount < 1)
                {
                    ResourceGenerator new_resource = Instantiate(ResourceGeneratorPrefab).GetComponent<ResourceGenerator>();
                    new_resource.transform.SetParent(neighbor.transform);
                    new_resource.transform.position = neighbor.transform.position + new Vector3(0.0f, 2.0f);
                    new_resource.transform.localScale = new Vector3(0.01f, 0.1f, 0.01f);
                    Resources.Add(new_resource);
                }
            }
        }
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        PortNationality = (Nationality)Random.Range(0, 5);

        int port_index = Random.Range(0, 5);
        PortNationality = (Nationality)port_index;
        if (port_index == 5)
            port_index = Random.Range(0, 4);

        PortName = name = NameGenerator.Instance.GetPortName(port_index);

        Shipyard = Instantiate(FleetPrefab).GetComponent<Fleet>();
        Shipyard.Name = string.Format("{0}Shipyard", PortName);
        Shipyard.transform.position = new Vector3(0.0f, 0.0f, 10000.0f);

        NetworkServer.Spawn(Shipyard.gameObject);

        RpcSetShipyard(Shipyard.Name);
    }

    /// <summary>
    /// Client-side call to set shipyard
    /// </summary>
    /// <param name="name">Name of shipyard to attach to port</param>
    [ClientRpc]
    void RpcSetShipyard(string name)
    {
        StartCoroutine(WaitForShipyard(name));
    }

    /// <summary>
    /// Waits for shipyard to be spawned
    /// </summary>
    /// <param name="name">Name of shipyard to wait for</param>
    /// <returns></returns>
    IEnumerator WaitForShipyard(string name)
    {
        while (!GameObject.Find(name))
            yield return null;
        Shipyard = GameObject.Find(name).GetComponent<Fleet>();
    }
    
    /// <summary>
    /// Client-side function to update information that can't be sent over network
    /// </summary>
    /// <param name="parent_tile">Name of tile port is to be on</param>
    [ClientRpc]
    public void RpcSpawnPortOthers(string parent_tile)
    {
        transform.SetParent(GameObject.Find(string.Format("Grid/{0}", parent_tile)).transform, false);
        transform.localPosition = new Vector3(0.0f, 0.25f, 0.0f);
        HexGrid.Ports.Add(this);

        InitializePort();
    }

    void OnMouseDown()
    {
        Fleet current_fleet = PlayerScript.MyPlayer.ActiveFleet;
        PortShopManager.Instance.CurrentPort = this;
        PortShopManager.Instance.OpenShop(current_fleet);
    }

    void OnMouseEnter()
    {
        Tooltip.Instance.EnableTooltip(true);
        Tooltip.Instance.UpdateTooltip(name);

        foreach(ResourceGenerator generator in Resources)
        {
            generator.GetComponentInChildren<CanvasGroup>().alpha = 1;
        }
    }

    void OnMouseExit()
    {
        Tooltip.Instance.EnableTooltip(false);

        foreach (ResourceGenerator generator in Resources)
        {
            generator.GetComponentInChildren<CanvasGroup>().alpha = 0;
        }
    }
}
