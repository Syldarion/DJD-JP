using UnityEngine;
using System.Collections;

public enum ResourceType
{
    Food,
    Gold
}

public class LandHex : HexTile
{
    public bool CoastalTile;

    public bool HasPort;
    public Cargo TileResources;
    public ResourceType ResourceType;
    public PlayerScript Owner;
    public int ResourceLevel;

    void Start()
    {
        InitializeTile();
    }

    void Update()
    {
        
    }

    public override void InitializeTile()
    {
        IsWater = false;

        TileResources = new Cargo();
    }

    public bool IsCoastal()
    {
        HexTile neighbor = null;
        foreach (HexCoordinate hc in Directions)
        {
            neighbor = GetNeighbor(hc);
            if (neighbor && neighbor.GetComponent<HexTile>().IsWater)
            {
                CoastalTile = true;
                return true;
            }
        }
        CoastalTile = false;
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