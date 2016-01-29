using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Network;

public class Port : NetworkedMonoBehavior
{
    [NetSync] public string PortName;
    public Nationality PortNationality;
    public HexTile SpawnTile;
    public Cargo Market;
    public List<Ship> Shipyard;

	void Start()
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

    void OnMouseDown()
    {
        GameObject.Find("PortShopBasePanel").GetComponent<PortShopManager>().CurrentPort = this;
        GameObject.Find("PortShopBasePanel").GetComponent<PortShopManager>().OpenShop();
    }
}
