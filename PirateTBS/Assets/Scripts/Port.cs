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
    public HexTile SpawnTile;                   //Tile ships from port are spawned on
    [SyncVar] public int PortGold;

    [SyncVar]
    public Cargo Market;                        //Contents of port's marketplace
    public List<Ship> Shipyard;                      //Contents of port's shipyard
    public List<ResourceGenerator> Resources;   //List of resource generators belonging to the port

    public GameObject ShipPrefab;              //Ship prefab for spawning new ships
    public GameObject ResourceGeneratorPrefab;  //Generator prefab for spawning new resource generators

    public static int[] DefaultPortPrices = {10, 20, 30, 40, 80};
    public static int[] PortPriceMods = {2, 4, 8, 10, 20};

    public SyncListInt PortPrices = new SyncListInt();

    public int GoldForPlayer;

    void Start()
    {
        
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        StartCoroutine(WaitForParentTile());
    }

    IEnumerator WaitForParentTile()
    {
        while (transform.parent == null || transform.parent.GetComponent<LandHex>() == null)
            yield return null;

        GenerateResources();

        GoldForPlayer = 0;

        Shipyard = new List<Ship>();
        for (int i = 0; i < 4; i++)
        {
            GameObject new_obj = new GameObject();
            new_obj.AddComponent(typeof(Ship));
            Ship new_ship = new_obj.GetComponent<Ship>();
            new_ship.SetClass((ShipClass)Random.Range(0, 8));
            new_obj.name = new_ship.Name = NameGenerator.Instance.GetShipName();

            Shipyard.Add(new_ship);
        }
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
        PortShopManager.Instance.CurrentPort = this;
        PortShopManager.Instance.OpenShop(PlayerScript.MyPlayer.ActiveShip);
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
