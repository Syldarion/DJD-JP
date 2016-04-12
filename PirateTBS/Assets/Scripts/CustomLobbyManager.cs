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
    public CanvasGroup MainMenuBasePanel;           //Reference to main menu base panel
    public CanvasGroup LobbyBasePanel;              //Reference to lobby base panel

    [Space]
    public string ServerPassword;                   //Password for server
    public GameObject ChatMessagePrefab;            //Reference to prefab for instantiating chat messages

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
            PanelUtilities.ActivatePanel(GetComponent<CanvasGroup>());
        }
        else
        {
            dontDestroyOnLoad = false;
            SceneManager.LoadScene("menu");
        }
    }

    public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
    {
        gamePlayer.GetComponent<PlayerScript>().Name = lobbyPlayer.GetComponent<CustomLobbyPlayer>().PlayerName;

        return true;
    }

    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        base.OnClientSceneChanged(conn);

        if (networkSceneName == "main")
        {
            
            PanelUtilities.DeactivatePanel(GetComponent<CanvasGroup>());
        }
        else if (networkSceneName == "menu")
        {
            PanelUtilities.DeactivatePanel(LobbyBasePanel);
            PanelUtilities.ActivatePanel(MainMenuBasePanel);
            PanelUtilities.ActivatePanel(GetComponent<CanvasGroup>());
        }
    }

    /// <summary>
    /// Moves the game into the main scene
    /// </summary>
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
        }

        CheckReadyToBegin();
        //ServerChangeScene("main");
    }
}
