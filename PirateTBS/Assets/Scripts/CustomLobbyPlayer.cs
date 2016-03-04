using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class CustomLobbyPlayer : NetworkLobbyPlayer
{
    [SyncVar(hook = "OnNameChanged")]
    public string PlayerName = "";
    public int NationalityIndex = 0;
    public bool IsReady = false;

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
        if (GetComponentInChildren<Text>().text == string.Empty)
            CmdUpdateName(CustomLobbyManager.Instance.GetComponent<NetworkInitializer>().PlayerName);
    }

    void SetupOtherPlayer()
    {

    }

    void OnNameChanged(string name)
    {
        PlayerName = name;
        GetComponentInChildren<Text>().text = name;
    }

    [Command]
    public void CmdUpdateName(string name)
    {
        PlayerName = name;
        this.name = PlayerName + "Panel";
    }
}
