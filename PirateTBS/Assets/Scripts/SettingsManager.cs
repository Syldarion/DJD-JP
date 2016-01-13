using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using BeardedManStudios.Network;

public class SettingsManager : NetworkedMonoBehavior
{
    public Dropdown MapTypeSelection;
    public Dropdown MapSizeSelection;
    public Dropdown GamePaceSelection;
    
    public int MapTypeIndex = 0;
    public int MapSizeIndex = 0;
    public int GamePaceIndex = 0;
    public int MapSeed;
    public int MapWidth = 40;
    public int MapHeight = 24;

    void Awake()
    {
        AddNetworkVariable(() => MapTypeIndex, x => UpdateMapType((int)x));
        AddNetworkVariable(() => MapSizeIndex, x => UpdateMapSize((int)x));
        AddNetworkVariable(() => GamePaceIndex, x => UpdateGamePace((int)x));
        AddNetworkVariable(() => MapSeed, x => MapSeed = (int)x);
    }

    void Start()
    {
        SetMapSeed();
    }
    
    void SetMapSeed()
    {
        MapSeed = Random.Range(0, 1000000);
    }
    
    public void UpdateMapType(int map_type_index)
    {
        if (Networking.PrimarySocket.IsServer)
            RPC("OnMapTypeChange", map_type_index);
    }

    public void UpdateMapSize(int map_size_index)
    {
        if (Networking.PrimarySocket.IsServer)
            RPC("OnMapSizeChange", map_size_index);
    }

    public void UpdateGamePace(int game_pace_index)
    {
        if (Networking.PrimarySocket.IsServer)
            RPC("OnGamePaceChange", game_pace_index);
    }

    [BRPC]
    void OnMapTypeChange(int map_type_index)
    {
        MapTypeIndex = map_type_index;
        MapTypeSelection.value = MapTypeIndex;
    }
    
    [BRPC]
    void OnMapSizeChange(int map_size_index)
    {
        MapSizeIndex = map_size_index;
        MapSizeSelection.value = MapSizeIndex;

        switch(MapSizeSelection.value)
        {
            case 0://Duel
                MapWidth = 40;
                MapHeight = 24;
                break;
            case 1://Tiny
                MapWidth = 56;
                MapHeight = 36;
                break;
            case 2://Small
                MapWidth = 66;
                MapHeight = 42;
                break;
            case 3://Standard
                MapWidth = 80;
                MapHeight = 52;
                break;
            case 4://Large
                MapWidth = 104;
                MapHeight = 64;
                break;
            case 5://Huge
                MapWidth = 128;
                MapHeight = 80;
                break;
        }
    }
    
    [BRPC]
    void OnGamePaceChange(int game_pace_index)
    {
        GamePaceIndex = game_pace_index;
        GamePaceSelection.value = GamePaceIndex;
    }

    public void StartGame()
    {
        if (SceneManager.GetSceneByName("main").IsValid())
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("main"));
        else
            SceneManager.LoadScene("main");
        Networking.ChangeClientScene(Networking.PrimarySocket.Port, "main");
    }
}
