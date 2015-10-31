using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NetworkInfoManager : MonoBehaviour
{
    CustomLobbyManager LManager;
    bool IsHosting;
    string ServerIP;
    int ServerPort;
    string Password;
    int MaxPlayers;

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

        if (hosting)
        {
            ServerIP = "localhost";
            ServerPort = int.Parse(GameObject.Find("ServerPortInput").GetComponent<InputField>().text);
            Password = GameObject.Find("ServerPasswordInput").GetComponent<InputField>().text;
            MaxPlayers = (int)GameObject.Find("PlayerCountSlider").GetComponent<Slider>().value;
        }
        else
        {
            ServerIP = GameObject.Find("ServerIPInput").GetComponent<InputField>().text;
            ServerPort = int.Parse(GameObject.Find("ServerPortInput").GetComponent<InputField>().text);
            Password = GameObject.Find("ServerPasswordInput").GetComponent<InputField>().text;
        }

        if (ServerIP != "")
        {
            Application.LoadLevel("lobby");
            StartCoroutine(WaitForManager());
        }
    }

    public void ConnectToServer()
    {
        LManager.networkAddress = ServerIP;
        LManager.networkPort = ServerPort;

        if (IsHosting)
        {
            LManager.ServerPassword = Password;
            LManager.maxPlayers = MaxPlayers;
            LManager.StartHost();
            Destroy(this.gameObject);
        }
        else if ((LManager.ServerPassword == "" || Password == LManager.ServerPassword)
            && LManager.numPlayers < LManager.maxPlayers)
        {
            LManager.StartClient();
            Destroy(this.gameObject);
        }
        else
            Application.LoadLevel("menu");
    }

    public IEnumerator WaitForManager()
    {
        while (!GameObject.Find("NetworkManager"))
            yield return new WaitForSeconds(0.1f);
        Debug.Log("Made it");
        LManager = GameObject.Find("NetworkManager").GetComponent<CustomLobbyManager>();
        Debug.Log("Got the manager");
        ConnectToServer();
    }
}
