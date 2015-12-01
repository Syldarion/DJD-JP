using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class SettingsManager : NetworkBehaviour
{
    public Dropdown MapTypeSelection;
    public Dropdown MapSizeSelection;
    public Dropdown GamePaceSelection;

    [SyncVar(hook = "OnMapTypeChange")]
    public int MapTypeIndex = 0;
    [SyncVar(hook = "OnMapSizeChange")]
    public int MapSizeIndex = 0;
    [SyncVar(hook = "OnGamePaceChange")]
    public int GamePaceIndex = 0;

    [SyncVar]
    public int MapSeed;
    public int MapWidth = 40;
    public int MapHeight = 24;

    void Start()
    {
        SetMapSeed();
        DontDestroyOnLoad(this.gameObject);
    }

    [Server]
    void SetMapSeed()
    {
        MapSeed = Random.Range(0, 1000000);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        OnMapTypeChange(MapTypeIndex);
        OnMapSizeChange(MapSizeIndex);
        OnGamePaceChange(GamePaceIndex);
    }

    public void OnMapTypeChange(int map_type_index)
    {
        MapTypeIndex = map_type_index;
        MapTypeSelection.value = MapTypeIndex;
    }

    public void OnMapSizeChange(int map_size_index)
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

    public void OnGamePaceChange(int game_pace_index)
    {
        GamePaceIndex = game_pace_index;
        GamePaceSelection.value = GamePaceIndex;
    }

    public void OnTypeChange(int index)
    {
        CmdMapTypeChanged(index);
    }

    public void OnSizeChange(int index)
    {
        CmdMapSizeChanged(index);
    }

    public void OnPaceChange(int index)
    {
        CmdGamePaceChanged(index);
    }

    [Command]
    public void CmdMapTypeChanged(int map_type_index)
    {
        MapTypeIndex = map_type_index;
    }

    [Command]
    public void CmdMapSizeChanged(int map_size_index)
    {
        MapSizeIndex = map_size_index;
    }

    [Command]
    public void CmdGamePaceChanged(int game_pace_index)
    {
        GamePaceIndex = game_pace_index;
    }
}
