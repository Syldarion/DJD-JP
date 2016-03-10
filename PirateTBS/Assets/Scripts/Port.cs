using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Port : NetworkBehaviour
{
    [SyncVar]
    public string PortName;
    public Nationality PortNationality;
    public HexTile SpawnTile;

    [SyncVar]
    public Cargo Market;
    public Fleet Shipyard;

    public GameObject FleetPrefab;

    static int port_id = 0;

    void Start()
    {

    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        PortName = string.Format("Port{0}", ++port_id);

        Shipyard = Instantiate(FleetPrefab).GetComponent<Fleet>();
        Shipyard.Name = string.Format("{0}Shipyard", PortName);
        Shipyard.transform.position = new Vector3(0.0f, 0.0f, 1000.0f);

        NetworkServer.Spawn(Shipyard.gameObject);

        RpcSetShipyard(Shipyard.Name);
    }

    [ClientRpc]
    void RpcSetShipyard(string name)
    {
        Shipyard = GameObject.Find(name).GetComponent<Fleet>();
    }

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
    
    void SendSystemMessage(string message)
    {
        //global message box
        //append message
    }
}
