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
    [SyncVar(hook = "OnNationalityChange")]
    public int NationalityIndex = 0;

    public override void OnStartClient()
    {
        base.OnStartClient();
        OnNameChange(PlayerName);
        OnNationalityChange(NationalityIndex);
    }

    //this is called for every player object when a player joins the lobby
    public override void OnClientEnterLobby()
    {
        base.OnClientEnterLobby();

        transform.SetParent(GameObject.Find("LobbyPlayerPanel").transform, false);

        if (isLocalPlayer)
        {
            PlayerNameText.interactable = true;
            PlayerNationalitySelection.interactable = true;
        }
        else
        {
            //PlayerNameText.interactable = false;
            //PlayerNationalitySelection.interactable = false;
        }

        OnNameChange(PlayerName);
    }

    public override void OnStartLocalPlayer()
    {
        PlayerNameText.onEndEdit.RemoveAllListeners();
        PlayerNameText.onEndEdit.AddListener(OnNameTextChange);

        PlayerNationalitySelection.onValueChanged.RemoveAllListeners();
        PlayerNationalitySelection.onValueChanged.AddListener(OnNationalityChange);
    }

    public void OnNameChange(string name)
    {
        PlayerName = name;
        PlayerNameText.text = PlayerName;
    }

    public void OnNationalityChange(int nation_index)
    {
        NationalityIndex = nation_index;
        PlayerNationalitySelection.value = NationalityIndex;
    }

    public void OnNameTextChange(string text)
    {
        CmdNameChanged(text);
    }

    public void OnNationSelectionChange(int index)
    {
        CmdNationalityChanged(index);
    }

    [Command]
    public void CmdNameChanged(string name)
    {
        PlayerName = name;
    }

    [Command]
    public void CmdNationalityChanged(int nation_index)
    {
        NationalityIndex = nation_index;
    }
}
