using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CustomLobbyPlayer : NetworkLobbyPlayer
{
    public GameObject LobbyPlayerPanelPrefab;
    GameObject my_panel;

    [SyncVar(hook = "OnNameChanged")]
    public string PlayerName = "";
    public int NationalityIndex = 0;
    public bool IsReady = false;

    void Start()
    {
        CmdMovePanel();
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();

        my_panel = Instantiate(LobbyPlayerPanelPrefab);
        NetworkServer.Spawn(my_panel);

        CmdUpdateName(GameObject.Find("NetworkManager").GetComponent<NetworkInitializer>().PlayerName);
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
        PlayerName = name;

        my_panel.GetComponentInChildren<Text>().text = name;
    }

    [Command]
    void CmdMovePanel()
    {
        RpcMovePanel();
    }

    [ClientRpc]
    void RpcMovePanel()
    {
        my_panel.transform.SetParent(GameObject.Find("LobbyPlayerList").transform, false);
    }
}
