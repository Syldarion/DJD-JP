using UnityEngine;
using System.Collections;

public enum ResourceType
{
    Food,
    Gold
}

public class LandHex : HexTile
{
    public bool Has_Port;
    public Cargo TileResources;
    public ResourceType ResourceType;
    public PlayerScript Owner;
    public int ResourceLevel;

    void Start()
    {
        MeshRenderer = GetComponent<SkinnedMeshRenderer>();
        baseColor = Color.green;
        _TileType = TileType.Land;

        MeshRenderer.material.color = baseColor;

        TileResources = new Cargo(0, 0);
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

    public void GenerateResources()
    {
        switch(ResourceType)
        {
            case ResourceType.Food:
                TileResources.TakeCargo(new Cargo(100 * ResourceLevel, 0), 100 * ResourceLevel, 0);
                break;
            case ResourceType.Gold:
                TileResources.TakeCargo(new Cargo(0, 100 * ResourceLevel), 0, 100 * ResourceLevel);
                break;
        }
    }
}