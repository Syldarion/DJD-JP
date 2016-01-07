using UnityEngine;
using System.Collections;
using BeardedManStudios.Network;

public class LobbyPlayerSpawner : NetworkedMonoBehavior
{
    public GameObject LobbyPlayer;

	void Start()
    {
        	
	}
	
	void Update()
    {

	}

    protected override void NetworkStart()
    {
        base.NetworkStart();

        Networking.Instantiate(LobbyPlayer, NetworkReceivers.All, callback: OnLobbyPlayerCreated);
    }

    void OnLobbyPlayerCreated(GameObject new_lobby_player)
    {
        new_lobby_player.transform.SetParent(GameObject.Find("LobbyPlayerList").transform, false);
    }
}
