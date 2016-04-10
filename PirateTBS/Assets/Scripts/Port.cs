using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Port : NetworkBehaviour
{
    [SyncVar]
    public string PortName;                 //Name of port
    public Nationality PortNationality;     //Associated nation of port
    public HexTile SpawnTile;               //Tile fleets from port are spawned on

    [SyncVar]
    public Cargo Market;                    //Contents of port's marketplace
    public Fleet Shipyard;                  //Contents of port's shipyard

    public GameObject FleetPrefab;          //Fleet prefab for spawning new fleets

    static int port_id = 0;                 //Dev variable for making sure all ports have unique names

    void Start()
    {

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

        Market = new Cargo(Random.Range(0, 500), Random.Range(0, 500), Random.Range(0, 500), Random.Range(0, 500), Random.Range(0, 500), Random.Range(0, 500));
    }

    void Update()
    {

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
        HexGrid.ports.Add(this);

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
        Tooltip.EnableTooltip(true);
        Tooltip.UpdateTooltip(name);
    }

    void OnMouseExit()
    {
        Tooltip.EnableTooltip(false);
    }
}
