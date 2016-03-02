using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class CustomLobbyManager : NetworkLobbyManager
{
    [HideInInspector]
    public static CustomLobbyManager Instance;

    [Header("UI References")]
    public CanvasGroup MainMenuBasePanel;
    public CanvasGroup LobbyBasePanel;

    [Space]
    public string ServerPassword;
    public GameObject ChatMessagePrefab;

    void Start()
    {
        Instance = this;
    }

    public override void OnLobbyClientConnect(NetworkConnection conn)
    {
        base.OnLobbyClientConnect(conn);

        if (SceneManager.GetActiveScene().name == "menu")
        {
            PanelUtilities.DeactivatePanel(MainMenuBasePanel);
            PanelUtilities.ActivatePanel(LobbyBasePanel);
        }
        else
        {
            SceneManager.LoadScene("menu");
            PanelUtilities.DeactivatePanel(MainMenuBasePanel);
            PanelUtilities.ActivatePanel(LobbyBasePanel);
        }
    }

    public override void OnLobbyClientDisconnect(NetworkConnection conn)
    {
        base.OnLobbyClientDisconnect(conn);

        if (SceneManager.GetActiveScene().name == "menu")
        {
            PanelUtilities.DeactivatePanel(LobbyBasePanel);
            PanelUtilities.ActivatePanel(MainMenuBasePanel);
        }
        else
            SceneManager.LoadScene("menu");
    }

    public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
    {
        Debug.Log("Entered lobbyserversceneloadedforplayer");

        gamePlayer = Instantiate(gamePlayerPrefab);
        gamePlayer.GetComponent<PlayerScript>().Name = lobbyPlayer.GetComponent<CustomLobbyPlayer>().PlayerName;

        return base.OnLobbyServerSceneLoadedForPlayer(lobbyPlayer, gamePlayer);
    }

    public void StartGame()
    {
        MainMenuBasePanel = null;
        LobbyBasePanel = null;

        SettingsManager.Instance.SettingsPanel = null;
        SettingsManager.Instance.MapTypeSelection = null;
        SettingsManager.Instance.MapSizeSelection = null;
        SettingsManager.Instance.GamePaceSelection = null;

        foreach (CustomLobbyPlayer go in GameObject.FindObjectsOfType<CustomLobbyPlayer>())
        {
            go.readyToBegin = true;
            go.transform.SetParent(null, false);
        }

        //CheckReadyToBegin();

        ServerChangeScene("main");
    }
}
