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
    GameSettingsManager ReferenceGameSettingsManager; //Reference to the Game Settings Manager

    public Text ResolutionText;
    public Text CurrentAAText;
    public Slider FullscreenSlider;

    void Start()
    {
        Instance = this;
        StartCoroutine(WaitForPlayer());
        ReferenceGameSettingsManager = GameObject.Find("GameSettingsManager").GetComponentInChildren<GameSettingsManager>();

        if (ReferenceGameSettingsManager)
        {
            //ResolutionText.text = ReferenceGameSettingsManager.ResolutionText.text;
            //CurrentAAText.text = ReferenceGameSettingsManager.CurrentAAText.text;
            FullscreenSlider.value = ReferenceGameSettingsManager.FullscreenSlider.value;
            ReferenceGameSettingsManager.ResolutionText = ResolutionText;
            ReferenceGameSettingsManager.CurrentAAText = CurrentAAText;
            ReferenceGameSettingsManager.FullscreenSlider = FullscreenSlider;
        }
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

    public void ApplyChanges()
    {
        ReferenceGameSettingsManager.ApplyChanges();
    }
    public void ModifyAA(int change)
    {
        ReferenceGameSettingsManager.ModifyAA(change);
    }
    public void ModifyResolution(int change)
    {
        ReferenceGameSettingsManager.ModifyResolution(change);
    }
    public void ModifyMasterVolume(float value)
    {
        ReferenceGameSettingsManager.ModifyMasterVolume(value);
    }
    public void ModifyMusicVolume(float value)
    {
        ReferenceGameSettingsManager.ModifyMusicVolume(value);
    }
    public void ModifyEffectsVolume(float value)
    {
        ReferenceGameSettingsManager.ModifyEffectsVolume(value);
    }
    }
