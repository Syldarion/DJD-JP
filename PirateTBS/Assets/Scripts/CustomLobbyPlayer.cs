using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class CustomLobbyPlayer : NetworkLobbyPlayer
{
    public InputField PlayerNameText;
    public Image PlayerNationalityFlag;
    public Dropdown PlayerNationalitySelection;
    public InputField ChatMessageInput;

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

        OnNameChange(PlayerName);
        OnNationalityChange(NationalityIndex);
    }

    public override void OnStartLocalPlayer()
    {
        CmdNameChanged("Player " + this.netId.ToString());

        PlayerNameText.onEndEdit.RemoveAllListeners();
        PlayerNameText.onEndEdit.AddListener(OnNameTextChange);

        PlayerNationalitySelection.onValueChanged.RemoveAllListeners();
        PlayerNationalitySelection.onValueChanged.AddListener(OnNationSelectionChange);

        ChatMessageInput = GameObject.Find("LobbyChatInput").GetComponent<InputField>();

        ChatMessageInput.onEndEdit.RemoveAllListeners();
        ChatMessageInput.onEndEdit.AddListener(SendChatMessage);

        PlayerNameText.interactable = true;
        PlayerNationalitySelection.interactable = true;
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

    public void SendChatMessage(string message)
    {
        if (message != string.Empty)
            CmdSendChatMessage(message);
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

    [Command]
    public void CmdSendChatMessage(string message)
    {
        GameObject.Find("ChatManager").GetComponent<ChatManager>().SendChatMessage(PlayerName, message);
    }
}
