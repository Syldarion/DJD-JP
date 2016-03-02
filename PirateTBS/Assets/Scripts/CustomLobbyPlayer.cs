using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CustomLobbyPlayer : NetworkLobbyPlayer
{
    [SyncVar(hook = "OnNameChanged")]
    public string PlayerName = "";
    public int NationalityIndex = 0;
    public bool IsReady = false;

    void Start()
    {
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();

        PlayerName = CustomLobbyManager.Instance.GetComponent<NetworkInitializer>().PlayerName;
    }


    public override void OnStartClient()
    {
        base.OnStartClient();

        transform.SetParent(GameObject.Find("LobbyPlayerList").transform, false);
    }

    void OnNameChanged(string name)
    {
        CmdUpdateName(name);
    }

    [Command]
    public void CmdUpdateName(string name)
    {
        RpcOnNameChange(name);
    }

    [ClientRpc]
    void RpcOnNameChange(string name)
    {
        GetComponentInChildren<Text>().text = name;
    }

    [Command]
    void CmdMovePanel()
    {
        RpcMovePanel();
    }

    [ClientRpc]
    void RpcMovePanel()
    {
        transform.SetParent(GameObject.Find("LobbyPlayerList").transform, false);
    }
}
