using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Network;

public class HexGrid : MonoBehaviour
{
    public WaterHex WaterHexPrefab;
    public LandHex LandHexPrefab;
    public GameObject PortPrefab;
    public static List<Port> ports;

    float hexWidth;

    int gridWidth;
    int gridHeight;
    int controlPoints;

    List<HexTile> tiles;

    string parent_tile;

	void Start()
    {
        ports = new List<Port>();
        tiles = new List<HexTile>();

        hexWidth = WaterHexPrefab.GetComponent<SkinnedMeshRenderer>().bounds.size.x;

        SettingsManager settings = GameObject.Find("SettingsManager").GetComponent<SettingsManager>();

        gridWidth = settings.MapWidth;
        gridHeight = settings.MapHeight;
        Random.seed = settings.MapSeed;
        controlPoints = settings.MapControlPoints;

        GenerateGrid(gridWidth, gridHeight, controlPoints);
	}
	
	void Update()
    {

	}

    //odd-q layout
    //odd x-values are offset
    void GenerateGrid(int x, int y, int control_points)
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

        StartCoroutine("DropControlPoints", control_points);
    }

    IEnumerator DropControlPoints(int control_points)
    {
        float total_width = hexWidth * (gridWidth - 4);
        float total_height = hexWidth * (gridHeight - 4);

        bool water_point = false;

        int sphere_radius = 0;
        Vector3 sphere_position = Vector3.zero;

        for (int i = 0; i < control_points; i++)
        {
            //75% chance for water control point
            if (Random.Range(0, 4) > 0)
                water_point = true;
            else
                water_point = false;

            sphere_radius = Random.Range(1, 6) * 100;
            sphere_position = new Vector3(Random.Range(-total_width / 2, total_width / 2), 0, Random.Range(-total_height / 2, total_height / 2));
            
            foreach(Collider other in Physics.OverlapSphere(sphere_position, sphere_radius))
            {
                if(water_point && !other.GetComponent<HexTile>().IsWater)
                {
                    if (!other.GetComponent<WaterHex>())
                        other.gameObject.AddComponent<WaterHex>();
                    other.GetComponent<WaterHex>().CopyTile(other.GetComponent<LandHex>());
                    other.GetComponent<LandHex>().enabled = false;
                    Destroy(other.GetComponent<LandHex>());
                    other.GetComponent<WaterHex>().InitializeTile();
                }
                else if(!water_point && other.GetComponent<HexTile>().IsWater)
                {
                    if (!other.GetComponent<LandHex>())
                        other.gameObject.AddComponent<LandHex>();
                    other.GetComponent<LandHex>().CopyTile(other.GetComponent<WaterHex>());
                    other.GetComponent<WaterHex>().enabled = false;
                    Destroy(other.GetComponent<WaterHex>());
                    other.GetComponent<LandHex>().InitializeTile();
                }
            }

            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(1.0f);

        if (NetworkingManager.Instance.OwningNetWorker.IsServer)
            CreatePorts(gridWidth / 4);
    }

    //i = x + width * y
    void CreatePorts(int number_of_ports)
    {
        List<LandHex> coastal_tiles = new List<LandHex>();

        for (int i = 0; i < gridWidth; i++)
        {
            for(int j = 0; j < gridHeight; j++)
            {
                try {
                    if (tiles[i + gridWidth * j].GetComponent<LandHex>() && tiles[i + gridWidth * j].GetComponent<LandHex>().IsCoastal())
                    {
                        coastal_tiles.Add(tiles[i + gridWidth * j].GetComponent<LandHex>());
                    }
                }
                catch(MissingReferenceException ex)
                {
                    Debug.Log(tiles[i + gridWidth * j].name);
                }
            }
        }

        Debug.Log(string.Format("{0} coastal tiles", coastal_tiles.Count));

        int selected_tile;

        for (int i = 0; i < number_of_ports; i++)
        {
            selected_tile = Random.Range(0, coastal_tiles.Count);

            parent_tile = coastal_tiles[selected_tile].name;
            Networking.Instantiate(PortPrefab, NetworkReceivers.All, callback: SpawnPortCallback);

            coastal_tiles[selected_tile].HasPort = true;
            coastal_tiles.RemoveAt(selected_tile);
        }
    }

    void SpawnPortCallback(SimpleNetworkedMonoBehavior new_port)
    {
        new_port.RPC("SpawnPortOthers", NetworkReceivers.All, parent_tile);
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
            for(int j = 0; j < x; j++)
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
        for(int i = 1; i <= movement; i++)
        {
            fringes.Add(new List<HexTile>());

            foreach(HexTile ht in fringes[i - 1])
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