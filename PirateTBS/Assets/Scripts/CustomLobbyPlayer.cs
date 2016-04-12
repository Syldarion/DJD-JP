using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class CustomLobbyPlayer : NetworkLobbyPlayer
{
    public static CustomLobbyPlayer MyPlayer;           //Reference to the local lobby player

    [SyncVar(hook = "OnNameChanged")]
    public string PlayerName = "";                      //Lobby player username
    public int NationalityIndex = 0;                    //Index of nationality
    public bool IsReady = false;                        //Is this player ready to play?

    public override void OnClientEnterLobby()
    {
        base.OnClientEnterLobby();

        if (isLocalPlayer)
            SetupLocalPlayer();
        else
            SetupOtherPlayer();

        OnNameChanged(PlayerName);
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();

        SetupLocalPlayer();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        transform.SetParent(GameObject.Find("LobbyPlayerList").transform, false);
    }

    void SetupLocalPlayer()
    {
        MyPlayer = this;

        if (GetComponentInChildren<Text>().text == string.Empty)
            CmdUpdateName(CustomLobbyManager.Instance.GetComponent<NetworkInitializer>().PlayerName);
    }

    void SetupOtherPlayer()
    {

    }

    /// <summary>
    /// Callback when username is changed
    /// </summary>
    /// <param name="name">New player name</param>
    void OnNameChanged(string name)
    {
        PlayerName = name;
        GetComponentInChildren<Text>().text = name;
    }

    /// <summary>
    /// Server-side command to change player name
    /// </summary>
    /// <param name="name">New player name</param>
    [Command]
    public void CmdUpdateName(string name)
    {
        PlayerName = name;
        this.name = PlayerName + "Panel";
    }

    /// <summary>
    /// Server-side command to send chat message
    /// </summary>
    /// <param name="message">Message to send to chat</param>
    [Command]
    public void CmdSendMessage(string message)
    {
        ChatManager.Instance.NewMessage(PlayerName, message);
    }
}
