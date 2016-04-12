using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsManager : NetworkBehaviour
{
    [HideInInspector]
    public static SettingsManager Instance;

    public RectTransform SettingsPanel;         //Reference to lobby settings panel

    public Dropdown MapTypeSelection;           //Reference to map type selection
    public Dropdown MapSizeSelection;           //Reference to map size selection
    public Dropdown GamePaceSelection;          //Reference to game pace selection
    
    [SyncVar]
    public int MapTypeIndex = 0;                //Current map type selected index
    [SyncVar]
    public int MapSizeIndex = 0;                //Current map size selected index
    [SyncVar]
    public int GamePaceIndex = 0;               //Current game pace selected index
    [SyncVar]
    public int MapSeed;                         //Current map seed
    [SyncVar]
    public int MapWidth = 40;                   //Current map width
    [SyncVar]
    public int MapHeight = 24;                  //Current map height
    [SyncVar]
    public int MapControlPoints = 64;           //Current map control points

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        Instance = this;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        SettingsPanel.GetComponent<CanvasGroup>().interactable = true;

        MapSeed = Random.Range(0, 1000000);
        //MapSeed = 514504;

        DontDestroyOnLoad(this);
    }
    
    /// <summary>
    /// Server-side function to update the selected map type
    /// </summary>
    /// <param name="map_type_index">Map type index</param>
    [Server]
    public void UpdateMapType(int map_type_index)
    {
        MapTypeIndex = map_type_index;
        RpcOnMapTypeChange(map_type_index);
    }

    /// <summary>
    /// Server-side function to update the selected map size
    /// </summary>
    /// <param name="map_size_index">Map size index</param>
    [Server]
    public void UpdateMapSize(int map_size_index)
    {
        MapSizeIndex = map_size_index;

        switch (MapSizeSelection.value)
        {
            case 0://Duel
                MapWidth = 40;
                MapHeight = 24;
                MapControlPoints = 32;
                break;
            case 1://Tiny
                MapWidth = 56;
                MapHeight = 36;
                MapControlPoints = 32;
                break;
            case 2://Small
                MapWidth = 66;
                MapHeight = 42;
                MapControlPoints = 48;
                break;
            case 3://Standard
                MapWidth = 80;
                MapHeight = 52;
                MapControlPoints = 48;
                break;
            case 4://Large
                MapWidth = 104;
                MapHeight = 64;
                MapControlPoints = 64;
                break;
            case 5://Huge
                MapWidth = 128;
                MapHeight = 80;
                MapControlPoints = 64;
                break;
        }

        RpcOnMapSizeChange(map_size_index);
    }

    /// <summary>
    /// Server-side function to update the game pace
    /// </summary>
    /// <param name="game_pace_index">Game pace index</param>
    [Server]
    public void UpdateGamePace(int game_pace_index)
    {
        GamePaceIndex = game_pace_index;
        RpcOnGamePaceChange(game_pace_index);
    }

    /// <summary>
    /// Client-side function to update the map type
    /// </summary>
    /// <param name="map_type_index">Map type index</param>
    [ClientRpc]
    void RpcOnMapTypeChange(int map_type_index)
    {
        MapTypeSelection.value = MapTypeIndex;
    }
    
    /// <summary>
    /// Client-side function to update the map size
    /// </summary>
    /// <param name="map_size_index">Map size index</param>
    [ClientRpc]
    void RpcOnMapSizeChange(int map_size_index)
    {
        MapSizeSelection.value = MapSizeIndex;
    }
    
    /// <summary>
    /// Client-side function to update the game pace
    /// </summary>
    /// <param name="game_pace_index">Game pace index</param>
    [ClientRpc]
    void RpcOnGamePaceChange(int game_pace_index)
    {
        GamePaceSelection.value = GamePaceIndex;
    }
}
