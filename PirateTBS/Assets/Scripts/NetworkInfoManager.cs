using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using BeardedManStudios.Network;

public class NetworkInfoManager : MonoBehaviour
{
    bool IsHosting;
    string ServerIP;
    string Password;
    int MaxPlayers;
    string PlayerName;

    NetWorker Socket;

    void Start()
    {
        MaxPlayers = 2;
    }

    public void UpdateMaxPlayerCount(int modifier)
    {
        MaxPlayers = Mathf.Clamp(MaxPlayers + modifier, 2, 16);
        GameObject.Find("PlayerCountBackText").GetComponent<Text>().text = MaxPlayers.ToString();
        GameObject.Find("PlayerCountFrontText").GetComponent<Text>().text = MaxPlayers.ToString();
    }

    public void UpdateServerAndRun(bool hosting)
    {
        IsHosting = hosting;

        if (ServerIP != string.Empty)
        {
            if (hosting)
            {
                ServerIP = "127.0.0.1";
                Password = GameObject.Find("HostServerPasswordInput").GetComponent<InputField>().text;
                PlayerName = GameObject.Find("HostPlayerNameInput").GetComponent<InputField>().text;
            }
            else
            {
                ServerIP = GameObject.Find("JoinServerIPInput").GetComponent<InputField>().text;
                if (ServerIP == "localhost")
                    ServerIP = "127.0.0.1";
                Password = GameObject.Find("JoinServerPasswordInput").GetComponent<InputField>().text;
                PlayerName = GameObject.Find("JoinPlayerNameInput").GetComponent<InputField>().text;
            }

            ConnectToNetwork();
        }
    }

    void ConnectToNetwork()
    {
        if (SceneManager.GetSceneByName("lobby").IsValid())
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("lobby"));
        else
            SceneManager.LoadScene("lobby");

        if (IsHosting)
            Socket = Networking.Host(5666, Networking.TransportationProtocolType.UDP, MaxPlayers);
        else
            Socket = Networking.Connect(ServerIP, 5666, Networking.TransportationProtocolType.UDP);

        Networking.SetPrimarySocket(Socket);

        if (Networking.PrimarySocket.Connected)
            SetNameAndChangeScene();
        else
            Networking.PrimarySocket.connected += SetNameAndChangeScene;
    }

    void SetNameAndChangeScene()
    {
        Networking.PrimarySocket.connected -= SetNameAndChangeScene;
        Networking.PrimarySocket.Me.SetName(PlayerName);
        if (Socket.IsServer)
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
