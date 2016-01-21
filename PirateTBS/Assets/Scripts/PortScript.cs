using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BeardedManStudios.Network;

public class PortScript : NetworkedMonoBehavior
{
    public string PortName;
    public Nationality PortNationality;
    public HexTile SpawnTile;

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

    [BRPC]
    public void UpdateName(string new_name)
    {
        PortName = new_name;
    }
}
