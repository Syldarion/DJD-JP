using UnityEngine;
using System.Collections;

public class LandHex : HexTile
{
    public bool Has_Port;

    void Start()
    {
        MeshRenderer = GetComponent<SkinnedMeshRenderer>();
        baseColor = Color.green;
        _TileType = TileType.Land;

        MeshRenderer.material.color = baseColor;
    }

    void Update()
    {

    }

    public bool IsCoastal()
    {
        foreach (HexCoordinate hc in Directions)
            if (GetNeighbor(hc).GetComponent<WaterHex>())
                return true;
        return false;
    }

    void OnMouseEnter()
    {
        
    }

    void OnMouseExit()
    {

    }
}