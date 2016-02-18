using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsManager : NetworkBehaviour
{
    public RectTransform SettingsPanel;

    public Dropdown MapTypeSelection;
    public Dropdown MapSizeSelection;
    public Dropdown GamePaceSelection;
    
    [SyncVar(hook = "RpcOnMapTypeChange")]
    public int MapTypeIndex = 0;
    [SyncVar(hook = "RpcOnMapSizeChange")]
    public int MapSizeIndex = 0;
    [SyncVar(hook = "RpcOnGamePaceChange")]
    public int GamePaceIndex = 0;
    [SyncVar]
    public int MapSeed;
    public int MapWidth = 40;
    public int MapHeight = 24;
    public int MapControlPoints = 64;

    public override void OnStartServer()
    {
        base.OnStartServer();

        SettingsPanel.GetComponent<CanvasGroup>().interactable = true;

        MapSeed = Random.Range(0, 1000000);

        DontDestroyOnLoad(this);
    }
    
    [Server]
    public void UpdateMapType(int map_type_index)
    {
        MapTypeIndex = map_type_index;
    }

    [Server]
    public void UpdateMapSize(int map_size_index)
    {
        MapSizeIndex = map_size_index;
    }

    [Server]
    public void UpdateGamePace(int game_pace_index)
    {
        GamePaceIndex = game_pace_index;
    }

    [ClientRpc]
    void RpcOnMapTypeChange(int map_type_index)
    {
        MapTypeIndex = map_type_index;
        MapTypeSelection.value = MapTypeIndex;
    }
    
    [ClientRpc]
    void RpcOnMapSizeChange(int map_size_index)
    {
        MapSizeIndex = map_size_index;
        MapSizeSelection.value = MapSizeIndex;

        switch(MapSizeSelection.value)
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
                MapControlPoints = 64;
                break;
            case 3://Standard
                MapWidth = 80;
                MapHeight = 52;
                MapControlPoints = 64;
                break;
            case 4://Large
                MapWidth = 104;
                MapHeight = 64;
                MapControlPoints = 128;
                break;
            case 5://Huge
                MapWidth = 128;
                MapHeight = 80;
                MapControlPoints = 128;
                break;
        }
    }
    
    [ClientRpc]
    void RpcOnGamePaceChange(int game_pace_index)
    {
        GamePaceIndex = game_pace_index;
        GamePaceSelection.value = GamePaceIndex;
    }

    [Server]
    public void StartGame()
    {
        NetworkManager.singleton.ServerChangeScene("main");
    }
}
