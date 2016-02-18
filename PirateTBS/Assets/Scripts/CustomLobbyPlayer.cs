using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CustomLobbyPlayer : NetworkLobbyPlayer
{
    public string PlayerName = "";
    public int NationalityIndex = 0;
    public bool IsReady = false;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        CmdUpdateName(GameObject.Find("NetworkManager").GetComponent<NetworkInitializer>().PlayerName);
        CmdMovePanel();
    }

    [Command]
    public void CmdUpdateName(string name)
    {
        RpcOnNameChange(name);
    }

    [ClientRpc]
    void RpcOnNameChange(string name)
    {
        PlayerName = name;

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
