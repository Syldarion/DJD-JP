using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InGameMenuController : MonoBehaviour
{
    [HideInInspector]
    public static InGameMenuController Instance;

    public Text NetworkIPText;
    public Text NetworkPingText;

    PlayerScript ReferencePlayer;

	void Start()
    {
        Instance = this;
        StartCoroutine(WaitForPlayer());
	}
	
	void Update()
    {

	}

    public void LeaveGame()
    {
        if (ReferencePlayer.connectionToServer.isConnected)
            ReferencePlayer.connectionToServer.Disconnect();
        else
            CustomLobbyManager.singleton.StopHost();
    }

    IEnumerator WaitForPlayer()
    {
        while (!PlayerScript.MyPlayer)
            yield return null;
        ReferencePlayer = PlayerScript.MyPlayer;

        NetworkIPText.text = ReferencePlayer.connectionToServer.address;

        StartCoroutine(UpdatePing());
    }

    IEnumerator UpdatePing()
    {
        while(ReferencePlayer.connectionToServer.isConnected)
        {
            Ping ping_to_host = new Ping(ReferencePlayer.connectionToServer.address);
            while (!ping_to_host.isDone)
                yield return null;

            NetworkPingText.text = ping_to_host.time.ToString();

            yield return new WaitForSeconds(10.0f);
        }

        NetworkPingText.text = "0";
    }
}
