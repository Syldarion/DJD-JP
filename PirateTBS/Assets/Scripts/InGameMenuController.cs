using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InGameMenuController : MonoBehaviour
{
    [HideInInspector]
    public static InGameMenuController Instance;

    public Text NetworkIPText;              //Reference to text displaying IP address
    public Text NetworkPingText;            //Reference to text displaying ping

    PlayerScript ReferencePlayer;           //Reference to player in control of this menu

    GameSettingsManager SettingsManager;

	void Start()
    {
        Instance = this;
        StartCoroutine(WaitForPlayer());
	}
	
	void Update()
    {

	}

    /// <summary>
    /// Leave the current game
    /// </summary>
    public void LeaveGame()
    {
        if (ReferencePlayer.connectionToServer.isConnected)
            ReferencePlayer.connectionToServer.Disconnect();
        else
            CustomLobbyManager.singleton.StopHost();
    }

    /// <summary>
    /// Wait for local player to be setup
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitForPlayer()
    {
        while (!PlayerScript.MyPlayer)
            yield return null;
        ReferencePlayer = PlayerScript.MyPlayer;

        NetworkIPText.text = ReferencePlayer.connectionToServer.address;

        StartCoroutine(UpdatePing());
    }

    /// <summary>
    /// Update ping every 10 seconds while connected to server
    /// </summary>
    /// <returns></returns>
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

    public void CloseInGameMenu()
    {
        PanelUtilities.DeactivatePanel(GetComponent<CanvasGroup>());
        if (PlayerScript.MyPlayer)
            PlayerScript.MyPlayer.OpenUI = null;
    }
}
