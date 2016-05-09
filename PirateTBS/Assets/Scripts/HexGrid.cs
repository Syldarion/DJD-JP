using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class HexGrid : NetworkBehaviour
{
    [HideInInspector]
    public static HexGrid Instance;

    public WaterHex WaterHexPrefab;             //Reference to prefab for instantiating water hexes
    public LandHex LandHexPrefab;               //Reference to prefab for instantiating land hexes
    public GameObject PortPrefab;               //Reference to prefab for instantiating ports
    public static List<Port> Ports;             //List of ports on the grid

    public float HexWidth;                      //Width of a single hex tile

    public int GridWidth;                       //Width of the grid
    public int GridHeight;                      //Height of the grid
    int controlPoints;                          //How many control points to use in generation

    public List<LandHex> LandTiles;             //List of land tiles on the grid
    public List<WaterHex> WaterTiles;           //List of water tiles on the grid

    public bool server_side = false;            //Is this grid on the server?

	void Start()
    {
        Instance = this;

        Ports = new List<Port>();

        LandTiles = new List<LandHex>();
        WaterTiles = new List<WaterHex>();

        HexWidth = WaterHexPrefab.GetComponent<MeshRenderer>().bounds.size.x;

        SettingsManager settings = SettingsManager.Instance;

        GridWidth = settings.MapWidth;
        GridHeight = settings.MapHeight;
        Random.seed = settings.MapSeed;
        controlPoints = settings.MapControlPoints;

        GenerateGrid(GridWidth, GridHeight, controlPoints);
	}

    public override void OnStartServer()
    {
        base.OnStartServer();

        server_side = true;
    }

    void Update()
    {

	}

    /// <summary>
    /// Generates a new grid
    /// </summary>
    /// <param name="x">Width for new grid</param>
    /// <param name="y">Height for new grid</param>
    /// <param name="control_points">Control point count for new grid</param>
    void GenerateGrid(int x, int y, int control_points)
    {
        LoadingScreenManager.Instance.SetMessage("Starting generation...");
        LoadingScreenManager.Instance.SetProgress(0.0f);

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

        StartCoroutine(DropControlPoints(control_points));
    }

    /// <summary>
    /// Drops control points for map generation
    /// </summary>
    /// <param name="control_points">Number of control points</param>
    /// <returns></returns>
    IEnumerator DropControlPoints(int control_points)
    {
        LoadingScreenManager.Instance.SetMessage("Dropping control points...");
        LoadingScreenManager.Instance.SetProgress(5.0f);

        float total_width = HexWidth * (GridWidth - 4);
        float total_height = HexWidth * (GridHeight - 4);

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

            sphere_radius = Random.Range(1, 6) * 10;
            sphere_position = new Vector3(Random.Range(-total_width / 2, total_width / 2), 0, Random.Range(-total_height / 2, total_height / 2));
            
            foreach(Collider other in Physics.OverlapSphere(sphere_position, sphere_radius))
            {
                if (other.GetComponent<HexTile>())
                {
                    if (water_point && !other.GetComponent<HexTile>().IsWater)
                    {
                        string new_name = other.name;

                        GameObject new_hex = Instantiate(WaterHexPrefab).gameObject;

                        new_hex.transform.SetParent(this.transform);
                        new_hex.transform.localPosition = other.transform.localPosition;
                        new_hex.GetComponent<WaterHex>().CopyTile(other.GetComponent<LandHex>());

                        Destroy(other.gameObject);

                        new_hex.name = new_name;
                    }
                    else if (!water_point && other.GetComponent<HexTile>().IsWater)
                    {
                        string new_name = other.name;

                        GameObject new_hex = Instantiate(LandHexPrefab).gameObject;

                        new_hex.transform.SetParent(this.transform);
                        new_hex.transform.localPosition = other.transform.localPosition;
                        new_hex.GetComponent<LandHex>().CopyTile(other.GetComponent<WaterHex>());

                        Destroy(other.gameObject);

                        new_hex.name = new_name;
                    }
                }
            }

            yield return new WaitForEndOfFrame();
        }

        PopulateTileLists();

        yield return new WaitForSeconds(1.0f);

        if (server_side)
            StartCoroutine(CreatePorts(GridWidth / 4));
        else
        {
            LoadingScreenManager.Instance.SetMessage("Done!");
            LoadingScreenManager.Instance.SetProgress(100.0f);
        }
    }

    /// <summary>
    /// Fills tiles lists with tiles present on the grid
    /// </summary>
    void PopulateTileLists()
    {
        LoadingScreenManager.Instance.SetMessage("Populating tile lists...");
        LoadingScreenManager.Instance.SetProgress(35.0f);

        for(int i = -GridWidth / 2; i < GridWidth / 2; i++)
        {
            for(int j = -GridHeight / 2; j < GridHeight / 2; j++)
            {
                Transform tile = transform.FindChild(string.Format("{0},{1}", i, j));

                if (tile.GetComponent<LandHex>())
                {
                    LandTiles.Add(tile.GetComponent<LandHex>());
                    tile.Translate(new Vector3(0.0f, .15f, 0.0f));
                }
                else if (tile.GetComponent<WaterHex>())
                    WaterTiles.Add(tile.GetComponent<WaterHex>());
            }
        }

        MiniMap.Instance.CopyHexGridToMap();
    }

    /// <summary>
    /// Create ports on grid
    /// </summary>
    /// <param name="number_of_ports">Number of ports to create</param>
    /// <returns></returns>
    IEnumerator CreatePorts(int number_of_ports)
    {
        LoadingScreenManager.Instance.SetMessage("Creating ports...");
        LoadingScreenManager.Instance.SetProgress(70.0f);

        List<LandHex> coastal_tiles = new List<LandHex>();

        foreach(LandHex lh in LandTiles)
        {
            if(lh.IsCoastal())
            {
                coastal_tiles.Add(lh);
            }
        }

        Debug.Log(string.Format("{0} coastal tiles", coastal_tiles.Count));

        int selected_tile;

        for (int i = 0; i < number_of_ports; i++)
        {
            selected_tile = Random.Range(0, coastal_tiles.Count);

            Port new_port = Instantiate(PortPrefab).GetComponent<Port>();
            new_port.transform.SetParent(coastal_tiles[selected_tile].transform, false);
            new_port.transform.localPosition = Vector3.zero;
            new_port.transform.localScale = new Vector3(.25f, 2.5f, .25f);

            new_port.InitializePort();

            NetworkServer.Spawn(new_port.gameObject);

            new_port.RpcSpawnPortOthers(coastal_tiles[selected_tile].name);

            coastal_tiles[selected_tile].HasPort = true;
            coastal_tiles.RemoveAt(selected_tile);

            yield return new WaitForEndOfFrame();
        }

        LoadingScreenManager.Instance.SetMessage("Done!");
        LoadingScreenManager.Instance.SetProgress(100.0f);
    }

    /// <summary>
    /// Find water tiles within a range
    /// </summary>
    /// <param name="start">Tile to start search on</param>
    /// <param name="movement">Movement range</param>
    /// <returns>List of water tiles within range</returns>
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

    /// <summary>
    /// Find all tiles within a range
    /// </summary>
    /// <param name="start">Tile to start search on</param>
    /// <param name="range">Movement range</param>
    /// <returns>List of tiles within range</returns>
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