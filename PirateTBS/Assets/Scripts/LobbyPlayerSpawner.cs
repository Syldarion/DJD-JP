using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BeardedManStudios.Network;

public class LobbyPlayerSpawner : NetworkedMonoBehavior
{
    public GameObject LobbyPlayer;
    
	void Start()
    {
        if (Networking.PrimarySocket.Connected)
            RPC("CreatePanel", NetworkReceivers.AllBuffered, Networking.PrimarySocket.Me.Name);
        else
        {
            Networking.PrimarySocket.connected += SpawnPlayer;
        }
        Networking.PrimarySocket.disconnected += delegate
        {
            RPC("DestroyPanel", NetworkReceivers.AllBuffered, Networking.PrimarySocket.Me.Name);
        };
    }
    
    void SpawnPlayer()
    {
        Networking.PrimarySocket.connected -= SpawnPlayer;
        RPC("CreatePanel", NetworkReceivers.AllBuffered, Networking.PrimarySocket.Me.Name);
    }

    [BRPC]
    void CreatePanel(string player_name)
    {
        GameObject new_panel = Instantiate(LobbyPlayer);
        new_panel.name = string.Format("{0}Panel", player_name);
        new_panel.transform.SetParent(GameObject.Find("LobbyPlayerList").transform, false);
        new_panel.transform.FindChild("PlayerName").GetComponent<Text>().text = player_name;
    }

    [BRPC]
    void DestroyPanel(string player_name)
    {
        Destroy(GameObject.Find(string.Format("{0}Panel", player_name)));
    }
}
