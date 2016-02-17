using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CustomLobbyPlayer : NetworkLobbyPlayer
{
    public string PlayerName = "";
    public int NationalityIndex = 0;
    public bool IsReady = false;

    ColorBlock NotReadyColorBlock;
    ColorBlock ReadyColorBlock;

    public override void OnClientEnterLobby()
    {
        base.OnClientEnterLobby();

        CmdUpdateName(GameObject.Find("NetworkManager").GetComponent<NetworkInitializer>().PlayerName);
    }

    [Command]
    public void CmdUpdateName(string name)
    {
        RpcOnNameChange(name);
    }

    [ClientRpc]
    void RpcOnNameChange(string name)
    {
        Debug.Log(name);
        PlayerName = name;

        GetComponentInChildren<Text>().text = name;
    }
}
