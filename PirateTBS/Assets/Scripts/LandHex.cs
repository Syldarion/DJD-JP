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

        TileResources = new Cargo();
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
                
                break;
            case ResourceType.Gold:
                
                break;
        }
    }
}