using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Network;

public class MiniMap : SimpleNetworkedMonoBehavior
{
    public WaterHex WaterHexPrefab;
    public LandHex LandHexPrefab;
    public GameObject PortPrefab;
    public List<Port> ports;

    float hexWidth;

    int gridWidth;
    int gridHeight;

    List<HexTile> tiles;

    void Start()
    {
        hexWidth = WaterHexPrefab.GetComponent<SkinnedMeshRenderer>().bounds.size.x;

        gridWidth = GameObject.Find("SettingsManager").GetComponent<SettingsManager>().MapWidth;
        gridHeight = GameObject.Find("SettingsManager").GetComponent<SettingsManager>().MapHeight;

        tiles = new List<HexTile>();

        Random.seed = GameObject.Find("SettingsManager").GetComponent<SettingsManager>().MapSeed;

        GenerateGrid(gridWidth, gridHeight, 64);
    }

    void Update()
    {

    }

    //odd-q layout
    //odd x-values are offset
    void GenerateGrid(int x, int y, int control_points)
    {
        int[][] generated_grid = VoronoiGrid(x, y, control_points);

        int half_grid_x = x / 2;
        int half_grid_y = y / 2;

        for (int i = -half_grid_x; i < half_grid_x; i++)
        {
            for (int j = -half_grid_y; j < half_grid_y; j++)
            {
                Transform new_hex;

                if (generated_grid[j + half_grid_y][i + half_grid_x] == 0)
                {
                    new_hex = Instantiate(WaterHexPrefab).transform;
                    new_hex.GetComponent<WaterHex>()._TileType = HexTile.TileType.Water;
                }
                else
                {
                    new_hex = Instantiate(LandHexPrefab).transform;
                    new_hex.GetComponent<LandHex>()._TileType = HexTile.TileType.Land;
                }

                new_hex.parent = this.transform;
                new_hex.localPosition = new Vector3(i * (hexWidth * 0.76f), 0.0f, j * (0.876f * hexWidth));

                if (i % 2 != 0)
                    new_hex.Translate(0.0f, 0.0f, 0.4325f * hexWidth, this.transform);

                new_hex.GetComponent<HexTile>().HexCoord.Q = i;
                new_hex.GetComponent<HexTile>().HexCoord.R = j;

                new_hex.name = string.Format("{0},{1}", i, j);

                new_hex.GetComponent<HexTile>().SetDirections(i % 2 == 0);

                tiles.Add(new_hex.GetComponent<HexTile>());
            }
        }

        if (NetworkingManager.Instance.OwnerId == 0)
            CreatePorts(gridWidth / 4);
    }
    //i = x + width * y
    void CreatePorts(int number_of_ports)
    {
        List<LandHex> coastal_tiles = new List<LandHex>();

        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                if (tiles[i + gridWidth * j].GetComponent<LandHex>() && tiles[i + gridWidth * j].GetComponent<LandHex>().IsCoastal())
                {
                    coastal_tiles.Add(tiles[i + gridWidth * j].GetComponent<LandHex>());
                }
            }
        }

        Debug.Log(string.Format("{0} coastal tiles", coastal_tiles.Count));

        int selected_tile;

        for (int i = 0; i < number_of_ports; i++)
        {
            selected_tile = Random.Range(0, coastal_tiles.Count);

            RPC("SpawnPort", NetworkReceivers.AllBuffered, coastal_tiles[selected_tile].name);

            coastal_tiles[selected_tile].Has_Port = true;
            coastal_tiles.RemoveAt(selected_tile);
        }
    }

    [BRPC]
    void SpawnPort(string parent_name)
    {
        GameObject new_port = Instantiate(PortPrefab);
        new_port.transform.SetParent(GameObject.Find(parent_name).transform);
        new_port.transform.localPosition = new Vector3(0.0f, 0.25f, 0.0f);
        ports.Add(new_port.GetComponent<Port>());
    }

    struct Point
    {
        public int x;
        public int y;

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public float Distance(Point other)
        {
            return Mathf.Sqrt(Mathf.Pow(other.x - x, 2) + Mathf.Pow(other.y - y, 2));
        }
    }

    int[][] VoronoiGrid(int x, int y, int control_points)
    {
        int[][] voronoi = new int[y][];
        for (int i = 0; i < y; i++)
            voronoi[i] = new int[x];

        //size of water border around map
        int water_border = 2;

        List<Point> points = new List<Point>();

        for (int i = 0; i < control_points; i++)
        {
            points.Add(new Point(Random.Range(water_border, x - water_border), Random.Range(water_border, y - water_border)));
            if (Random.Range(0, 3) > 0)
                voronoi[points[i].y][points[i].x] = 0;
            else
                voronoi[points[i].y][points[i].x] = 1;
        }

        for (int i = 0; i < y; i++)
        {
            for (int j = 0; j < x; j++)
            {
                Point closest = points[0];

                foreach (Point p in points)
                {
                    if (p.Distance(new Point(i, j)) < closest.Distance(new Point(i, j)))
                    {
                        closest = p;
                    }
                }

                voronoi[i][j] = voronoi[closest.y][closest.x];
            }
        }

        //finally, make sure you have a contiguous strip of water around the map, because the water_border isn't perfect
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < water_border; j++)
            {
                voronoi[j][i] = 0;
                voronoi[y - j - 1][i] = 0;
            }
        }
        for (int i = 0; i < y; i++)
        {
            for (int j = 0; j < water_border; j++)
            {
                voronoi[i][j] = 0;
                voronoi[i][x - j - 1] = 0;
            }
        }

        return voronoi;
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
                        if (!visited.Contains(neighbor_tile) && neighbor_tile._TileType == HexTile.TileType.Water)
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
