using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class CustomLobbyManager : NetworkLobbyManager
{
	void Start()
    {
        //StartHost();
	}
	
	void Update()
    {

	}

    public override GameObject OnLobbyServerCreateLobbyPlayer(NetworkConnection conn, short playerControllerId)
    {
        //return base.OnLobbyServerCreateLobbyPlayer(conn, playerControllerId);

        GameObject new_obj = Instantiate(this.lobbyPlayerPrefab.gameObject);

        new_obj.transform.SetParent(GameObject.Find("LobbyPlayerPanel").transform, false);

        return new_obj;
    }

    public void HostServer()
    {
        StartHost();
    }

    public void JoinServer()
    {
        StartClient();
    }
}
