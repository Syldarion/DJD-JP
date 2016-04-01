using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

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

        PortName = string.Format("Port{0}", ++port_id);

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
        GameObject.Find("PortShopBasePanel").GetComponent<PortShopManager>().CurrentPort = this;
        GameObject.Find("PortShopBasePanel").GetComponent<PortShopManager>().OpenShop(current_fleet);
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
