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
    public CombatShip CombatShipPrefab;

    public float HexWidth;

    public int GridWidth = 10;
    public int GridHeight = 10;

    public List<WaterHex> WaterTiles;

    public List<WaterHex> PlayerSpawnTiles;
    public List<WaterHex> EnemySpawnTiles;

    public Material WaterMaterial;

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
                WaterHex new_hex = Instantiate(WaterHexPrefab);
                new_hex.Discovered = true;
                new_hex.Fog = false;
                new_hex.GetComponent<MeshRenderer>().sharedMaterial = WaterMaterial;

                new_hex.transform.parent = this.transform;
                new_hex.transform.localPosition = new Vector3(i * (HexWidth * 0.76f), 0.0f, j * (0.876f * HexWidth));

                if (i % 2 != 0)
                    new_hex.transform.Translate(0.0f, 0.0f, 0.4325f * HexWidth, this.transform);

                new_hex.GetComponent<HexTile>().HexCoord.Q = i;
                new_hex.GetComponent<HexTile>().HexCoord.R = j;

                new_hex.name = string.Format("{0},{1}", i, j);

                new_hex.GetComponent<HexTile>().SetDirections(i % 2 == 0);

                WaterTiles.Add(new_hex);

                if (i == -5)
                    PlayerSpawnTiles.Add(new_hex);
                if (i == 4)
                    EnemySpawnTiles.Add(new_hex);
            }
        }

        PlaceShips();
    }

    void PlaceShips()
    {
        Fleet player_fleet = CombatManager.Instance.PlayerFleet;
        Fleet enemy_fleet = CombatManager.Instance.EnemyFleet;

        for(int i = 0; i < player_fleet.Ships.Count; i++)
        {
            CombatShip new_ship = Instantiate(CombatShipPrefab).GetComponent<CombatShip>();
            new_ship.CopyShip(player_fleet.Ships[i]);
            new_ship.LinkedShip = player_fleet.Ships[i];

            new_ship.transform.SetParent(PlayerSpawnTiles[i].transform, false);
            new_ship.transform.localScale = new Vector3(0.1f, 1.0f, 0.1f);

            new_ship.CurrentPosition = PlayerSpawnTiles[i];

            NetworkServer.SpawnWithClientAuthority(new_ship.gameObject, player_fleet.connectionToClient);
        }
        for(int i = 0; i < enemy_fleet.Ships.Count; i++)
        {
            CombatShip new_ship = Instantiate(CombatShipPrefab).GetComponent<CombatShip>();
            new_ship.CopyShip(enemy_fleet.Ships[i]);
            new_ship.LinkedShip = enemy_fleet.Ships[i];

            new_ship.transform.SetParent(EnemySpawnTiles[i].transform, false);
            new_ship.transform.localScale = new Vector3(0.1f, 1.0f, 0.1f);

            new_ship.CurrentPosition = EnemySpawnTiles[i];

            NetworkServer.SpawnWithClientAuthority(new_ship.gameObject, enemy_fleet.connectionToClient);
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
}
