using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class CustomLobbyManager : NetworkLobbyManager
{
    public string ServerPassword;
<<<<<<< HEAD
    public GameObject ChatMessagePrefab;
=======

    public GameObject ChatMessagePrefab;

	void Start()
    {
        //StartHost();
	}
	
	void Update()
    {

	}
>>>>>>> origin/master

    //I hate you Unity
    public void StartGame()
    {
<<<<<<< HEAD
        foreach (CustomLobbyPlayer t in GameObject.FindObjectsOfType<CustomLobbyPlayer>())
        {
            if (t.readyToBegin)
            {
                t.transform.SetParent(null, false);
            }
            else
            {
                foreach (CustomLobbyPlayer t2 in GameObject.FindObjectsOfType<CustomLobbyPlayer>())
                    t.transform.SetParent(GameObject.Find("LobbyPlayerPanel").transform, false);
                break;
            }
        }

        CheckReadyToBegin();
    }

    public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
    {
        Debug.Log("MAKING GAME PLAYER");

        CustomLobbyPlayer lobbyPlayerScript = lobbyPlayer.GetComponent<CustomLobbyPlayer>();
        PlayerScript gamePlayerScript = gamePlayer.GetComponent<PlayerScript>();

        gamePlayerScript.Name = lobbyPlayerScript.PlayerName;

        return true;
    }
=======
        //return base.OnLobbyServerCreateLobbyPlayer(conn, playerControllerId);

        GameObject new_obj = Instantiate(this.lobbyPlayerPrefab.gameObject);
        

        return new_obj;
    }
>>>>>>> origin/master
}
