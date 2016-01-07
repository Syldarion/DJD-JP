using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using BeardedManStudios.Network;

public class NetworkInfoManager : MonoBehaviour
{
    public GameObject LobbyPlayer;

    bool IsHosting;
    string ServerIP;
    int ServerPort;
    string Password;
    int MaxPlayers;

    NetWorker Socket;

	void Start()
    {
        DontDestroyOnLoad(this);
    }

	void Update()
    {

	}

    public void UpdateServerAndRun(bool hosting)
    {
        IsHosting = hosting;

        if (ServerIP != string.Empty)
        {
            if (hosting)
            {
                ServerIP = "127.0.0.1";
                ServerPort = int.Parse(GameObject.Find("ServerPortInput").GetComponent<InputField>().text);
                Password = GameObject.Find("ServerPasswordInput").GetComponent<InputField>().text;
                MaxPlayers = (int)GameObject.Find("PlayerCountSlider").GetComponent<Slider>().value;

                Socket = Networking.Host((ushort)ServerPort, Networking.TransportationProtocolType.UDP, MaxPlayers);
            }
            else
            {
                ServerIP = GameObject.Find("ServerIPInput").GetComponent<InputField>().text;
                if (ServerIP == "localhost")
                    ServerIP = "127.0.0.1";
                ServerPort = int.Parse(GameObject.Find("ServerPortInput").GetComponent<InputField>().text);
                Password = GameObject.Find("ServerPasswordInput").GetComponent<InputField>().text;

                Socket = Networking.Connect(ServerIP, (ushort)ServerPort, Networking.TransportationProtocolType.UDP);
            }

            Networking.SetPrimarySocket(Socket);

            //Networking.Sockets[(ushort)ServerPort].connected += delegate
            Socket.connected += delegate
            {
                if (SceneManager.GetSceneByName("lobby").IsValid())
                    SceneManager.SetActiveScene(SceneManager.GetSceneByName("lobby"));
                else
                    SceneManager.LoadScene("lobby");
                if (Socket.IsServer)
                    Networking.ChangeClientScene(Socket, "lobby");
            };
            //Networking.Sockets[(ushort)ServerPort].disconnected += delegate
            Socket.disconnected += delegate
            {
                if (SceneManager.GetSceneByName("menu").IsValid())
                    SceneManager.SetActiveScene(SceneManager.GetSceneByName("menu"));
                else
                    SceneManager.LoadScene("menu");
                if (Socket.IsServer)
                    Networking.ChangeClientScene(Socket, "menu");
            };
            Socket.serverDisconnected += delegate
            {
                if (SceneManager.GetSceneByName("menu").IsValid())
                    SceneManager.SetActiveScene(SceneManager.GetSceneByName("menu"));
                else
                    SceneManager.LoadScene("menu");
                if (Socket.IsServer)
                    Networking.ChangeClientScene(Socket, "menu");
            };
        }
    }
}
