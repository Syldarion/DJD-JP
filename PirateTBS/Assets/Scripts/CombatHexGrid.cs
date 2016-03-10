using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CombatHexGrid : MonoBehaviour
{
    [HideInInspector]
    public static CombatHexGrid Instance;

    public WaterHex WaterHexPrefab;

    public float HexWidth;

    public const int GridWidth = 10;
    public const int GridHeight = 10;

    public List<WaterHex> WaterTiles;

    void Start()
    {
        Instance = this;

        WaterTiles = new List<WaterHex>();

        HexWidth = WaterHexPrefab.GetComponent<MeshRenderer>().bounds.size.x;

        GenerateGrid(GridWidth, GridHeight);
    }

    void GenerateGrid(int x, int y)
    {
        int half_grid_x = x / 2;
        int half_grid_y = y / 2;

        for (int i = -half_grid_x; i < half_grid_x; i++)
        {
            for (int j = -half_grid_y; j < half_grid_y; j++)
            {
                Transform new_hex = Instantiate(WaterHexPrefab).transform;
                new_hex.GetComponent<WaterHex>().InitializeTile();

                new_hex.parent = this.transform;
                new_hex.localPosition = new Vector3(i * (HexWidth * 0.76f), 0.0f, j * (0.876f * HexWidth));

                if (i % 2 != 0)
                    new_hex.Translate(0.0f, 0.0f, 0.4325f * HexWidth, this.transform);

                new_hex.GetComponent<HexTile>().HexCoord.Q = i;
                new_hex.GetComponent<HexTile>().HexCoord.R = j;

                new_hex.name = string.Format("{0},{1}", i, j);

                new_hex.GetComponent<HexTile>().SetDirections(i % 2 == 0);
            }
        }
        PopulateTileLists();
    }

    void PopulateTileLists()
    {
        for (int i = -GridWidth / 2; i < GridWidth / 2; i++)
        {
            for (int j = -GridHeight / 2; j < GridHeight / 2; j++)
            {
                Transform tile = transform.FindChild(string.Format("{0},{1}", i, j));

                if (tile.GetComponent<WaterHex>())
                    WaterTiles.Add(tile.GetComponent<WaterHex>());
            }
        }
    }

    public static List<HexTile> MovementHex(HexTile start, int movement)
    {
        List<HexTile> visited = new List<HexTile>();
        visited.Add(start);

        List<List<HexTile>> fringes = new List<List<HexTile>>();
        fringes.Add(new List<HexTile>());
        fringes[0].Add(start);
        for (int i = 1; i <= movement; i++)
        {
            fringes.Add(new List<HexTile>());

            foreach (HexTile ht in fringes[i - 1])
            {
                for (int j = 0; j < 6; j++)
                {
                    HexTile neighbor_tile = ht.GetNeighbor(ht.Directions[j]);
                    if (neighbor_tile != null)
                        if (!visited.Contains(neighbor_tile) && neighbor_tile.IsWater)
                        {
                            visited.Add(neighbor_tile);
                            fringes[i].Add(neighbor_tile);
                        }
                }
            }
        }

        return visited;
    }

    public static List<HexTile> HexesWithinRange(HexTile start, int range)
    {
        List<HexTile> visited = new List<HexTile>();
        visited.Add(start);

        List<List<HexTile>> fringes = new List<List<HexTile>>();
        fringes.Add(new List<HexTile>());
        fringes[0].Add(start);
        for (int i = 1; i <= range; i++)
        {
            fringes.Add(new List<HexTile>());

            foreach (HexTile ht in fringes[i - 1])
            {
                for (int j = 0; j < 6; j++)
                {
                    HexTile neighbor_tile = ht.GetNeighbor(ht.Directions[j]);
                    if (neighbor_tile != null)
                        if (!visited.Contains(neighbor_tile))
                        {
                            visited.Add(neighbor_tile);
                            fringes[i].Add(neighbor_tile);
                        }
                }
            }
        }

        return visited;
    }
}
