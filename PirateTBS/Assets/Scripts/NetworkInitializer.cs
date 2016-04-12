using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;

public class NetworkInitializer : MonoBehaviour
{
    [Header("Network Info")]
    public string PlayerName;               //Desired username
    public string IPAddress;                //Server IP address
    public int Port;                        //Server port
    public string Password;                 //Password for server

    [Space]
    [Header("UI References")]
    public Text PlayerCountText;            //Reference to text showing selected max player count

    void Start()
    {
        PlayerName = "Default";
        IPAddress = "localhost";
        Port = 5666;
        Password = string.Empty;
	}
	
	void Update()
    {
        	
	}

    /// <summary>
    /// Set desired username
    /// </summary>
    /// <param name="name">New username</param>
    public void SetPlayerName(string name)
    {
        PlayerName = name;
    }

    /// <summary>
    /// Set server IP address to connect to
    /// </summary>
    /// <param name="address">IP address to parse</param>
    public void SetIPAddress(string address)
    {
        IPAddress = System.Net.IPAddress.Parse(address).ToString();
    }

    /// <summary>
    /// Set server password
    /// </summary>
    /// <param name="password">Password for server</param>
    public void SetPassword(string password)
    {
        Password = password;
    }

    /// <summary>
    /// Change max player count
    /// </summary>
    /// <param name="change">Amount to change count by</param>
    public void ChangePlayerCount(int change)
    {
        CustomLobbyManager.Instance.maxPlayers = Mathf.Clamp(CustomLobbyManager.Instance.maxPlayers + change, 2, 16);
        PlayerCountText.text = CustomLobbyManager.Instance.maxPlayers.ToString();
    }

    /// <summary>
    /// Connect to server as host
    /// </summary>
    public void ConnectAsHost()
    {
        NetworkManager.singleton.networkAddress = "localhost";
        NetworkManager.singleton.networkPort = 5666;
        NetworkManager.singleton.StartHost();
    }

    /// <summary>
    /// Connect to server as client
    /// </summary>
    public void ConnectAsClient()
    {
        NetworkManager.singleton.networkAddress = IPAddress;
        NetworkManager.singleton.networkPort = Port;
        NetworkManager.singleton.StartClient();
    }
}
