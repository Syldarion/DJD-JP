using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class CustomLobbyManager : NetworkLobbyManager
{
    public string ServerPassword;

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
        

        return new_obj;
    }
}
