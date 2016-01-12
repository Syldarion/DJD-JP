using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using BeardedManStudios.Network;

public class NetworkInfoManager : MonoBehaviour
{
    bool IsHosting;
    string ServerIP;
    int ServerPort;
    string Password;
    int MaxPlayers;

    NetWorker Socket;

    public void UpdateServerAndRun(bool hosting)
    {
        IsHosting = hosting;

        if (ServerIP != string.Empty)
        {
            if (hosting)
            {
                ServerIP = "127.0.0.1";
                ServerPort = int.Parse(GameObject.Find("ServerPortInput").GetComponent<InputField>().text);
                //Password = GameObject.Find("ServerPasswordInput").GetComponent<InputField>().text;
                MaxPlayers = (int)GameObject.Find("PlayerCountSlider").GetComponent<Slider>().value;
            }
            else
            {
                ServerIP = GameObject.Find("ServerIPInput").GetComponent<InputField>().text;
                if (ServerIP == "localhost")
                    ServerIP = "127.0.0.1";
                ServerPort = int.Parse(GameObject.Find("ServerPortInput").GetComponent<InputField>().text);
                //Password = GameObject.Find("ServerPasswordInput").GetComponent<InputField>().text;
            }

            LoadLobbyScene();
        }
    }

    void LoadLobbyScene()
    {
        if (SceneManager.GetSceneByName("lobby").IsValid())
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("lobby"));
        else
            SceneManager.LoadScene("lobby");

        if(IsHosting)
            Socket = Networking.Host((ushort)ServerPort, Networking.TransportationProtocolType.UDP, MaxPlayers);
        else
            Socket = Networking.Connect(ServerIP, (ushort)ServerPort, Networking.TransportationProtocolType.UDP);

        Networking.SetPrimarySocket(Socket);

        if (Socket.Connected && Socket.IsServer)
            Networking.ChangeClientScene(Socket, "lobby");
    }

    void LoadMenuScene()
    {
        if (SceneManager.GetSceneByName("menu").IsValid())
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("menu"));
        else
            SceneManager.LoadScene("menu");
        if (Socket.IsServer)
            Networking.ChangeClientScene(Socket, "menu");
    }

    void ServerDisconnect(string message)
    {
        if (SceneManager.GetSceneByName("menu").IsValid())
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("menu"));
        else
            SceneManager.LoadScene("menu");
        if (Socket.IsServer)
            Networking.ChangeClientScene(Socket, "menu");
    }
}
