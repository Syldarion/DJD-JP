using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Network;

public class Port : NetworkedMonoBehavior
{
    public string PortName;
    public Nationality PortNationality;
    public HexTile SpawnTile;
    public Cargo Market;
    public List<Ship> Shipyard;

    void Start()
    {

    }

    void InitializePort()
    {
        foreach (HexTile.HexCoordinate hc in GetComponentInParent<HexTile>().Directions)
            if (GetComponentInParent<HexTile>().GetNeighbor(hc)._TileType == HexTile.TileType.Water)
            {
                SpawnTile = GetComponentInParent<HexTile>().GetNeighbor(hc);
                break;
            }
    }

    void Update()
    {

    }

    [BRPC]
    void SpawnPortOthers(string parent_tile)
    {
        transform.SetParent(GameObject.Find(parent_tile).transform);
        transform.localPosition = new Vector3(0.0f, 0.25f, 0.0f);
        HexGrid.ports.Add(this);

        InitializePort();
    }

    void OnMouseDown()
    {
        Fleet current_fleet = GameObject.Find(Networking.PrimarySocket.Me.Name + "Controller").GetComponent<PlayerScript>().ActiveFleet;
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

    [BRPC]
    void SendSystemMessage(string message)
    {
        //global message box
        //append message
    }
}
