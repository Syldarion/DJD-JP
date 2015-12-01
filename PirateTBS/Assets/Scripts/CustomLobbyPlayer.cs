using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class CustomLobbyPlayer : NetworkLobbyPlayer
{
    public InputField PlayerNameText;
    public Image PlayerNationalityFlag;
    public Dropdown PlayerNationalitySelection;

    [SyncVar(hook = "OnNameChange")]
    public string PlayerName = "";

    public override void OnStartClient()
    {
        base.OnStartClient();
        
        OnNameChange(PlayerName);
    }

    public override void OnClientEnterLobby()
    {
        base.OnClientEnterLobby();
    }

    public override void OnStartLocalPlayer()
    {
        PlayerNameText = transform.GetChild(0).GetComponent<InputField>();
        PlayerNationalityFlag = transform.GetChild(1).GetComponent<Image>();
        PlayerNationalitySelection = transform.GetChild(2).GetComponent<Dropdown>();

        PlayerNameText.onEndEdit.RemoveAllListeners();
        PlayerNameText.onEndEdit.AddListener(OnNameTextChange);
    }

    public void OnNameChange(string name)
    {
        PlayerName = name;
        PlayerNameText.text = PlayerName;
    }

    public void OnNameTextChange(string text)
    {
        CmdNameChanged(text);
    }

    [Command]
    public void CmdNameChanged(string name)
    {
        PlayerName = name;
    }
}
