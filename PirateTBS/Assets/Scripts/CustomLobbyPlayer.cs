using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class CustomLobbyPlayer : NetworkLobbyPlayer
{
    public InputField PlayerNameText;
    public Image PlayerNationalityFlag;
    public Dropdown PlayerNationalitySelection;
<<<<<<< HEAD
    public Toggle PlayerReadyStateButton;
    //public InputField ChatMessageInput;

    public Sprite[] Flags;
=======
    public InputField ChatMessageInput;
>>>>>>> origin/master

    [SyncVar(hook = "OnNameChange")]
    public string PlayerName = "";
    [SyncVar(hook = "OnNationalityChange")]
    public int NationalityIndex = 0;
<<<<<<< HEAD
    [SyncVar(hook = "OnReadyStateChange")]
    public bool IsReady = false;

    ColorBlock NotReadyColorBlock;
    ColorBlock ReadyColorBlock;

    //Some serious bullshit just to workaround Unity's stupid beta networking code
    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }
=======
>>>>>>> origin/master

    public override void OnStartClient()
    {
        base.OnStartClient();
<<<<<<< HEAD

        OnNameChange(PlayerName);
        OnNationalityChange(NationalityIndex);
        OnReadyStateChange(IsReady);
=======
        OnNameChange(PlayerName);
        OnNationalityChange(NationalityIndex);
>>>>>>> origin/master
    }

    //this is called for every player object when a player joins the lobby
    public override void OnClientEnterLobby()
    {
        base.OnClientEnterLobby();

        transform.SetParent(GameObject.Find("LobbyPlayerPanel").transform, false);

        OnNameChange(PlayerName);
        OnNationalityChange(NationalityIndex);
<<<<<<< HEAD
        OnReadyStateChange(IsReady);
=======
>>>>>>> origin/master
    }

    public override void OnStartLocalPlayer()
    {
        CmdNameChanged("Player " + this.netId.ToString());

        PlayerNameText.onEndEdit.RemoveAllListeners();
        PlayerNameText.onEndEdit.AddListener(OnNameTextChange);

        PlayerNationalitySelection.onValueChanged.RemoveAllListeners();
        PlayerNationalitySelection.onValueChanged.AddListener(OnNationSelectionChange);

<<<<<<< HEAD
        PlayerReadyStateButton.onValueChanged.RemoveAllListeners();
        PlayerReadyStateButton.onValueChanged.AddListener(OnReadyStateValueChange);

        //ChatMessageInput = GameObject.Find("LobbyChatInput").GetComponent<InputField>();

        GameObject.Find("LobbyChatInput").GetComponent<InputField>().onEndEdit.RemoveAllListeners();
        GameObject.Find("LobbyChatInput").GetComponent<InputField>().onEndEdit.AddListener(SendChatMessage);

        PlayerNameText.interactable = true;
        PlayerNationalitySelection.interactable = true;
        PlayerReadyStateButton.interactable = true;

        EnableHostControls();
    }

    [Server]
    public void EnableHostControls()
    {
        GameObject.Find("LobbySettingsPanel").GetComponent<CanvasGroup>().interactable = true;
=======
        ChatMessageInput = GameObject.Find("LobbyChatInput").GetComponent<InputField>();

        ChatMessageInput.onEndEdit.RemoveAllListeners();
        ChatMessageInput.onEndEdit.AddListener(SendChatMessage);

        PlayerNameText.interactable = true;
        PlayerNationalitySelection.interactable = true;
>>>>>>> origin/master
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
<<<<<<< HEAD
        PlayerNationalityFlag.sprite = Flags[nation_index];
    }

    public void OnReadyStateChange(bool is_ready)
    {
        IsReady = is_ready;
        readyToBegin = is_ready;
        if(is_ready)
        {
            PlayerReadyStateButton.GetComponentInChildren<Text>().text = "READY";
            PlayerReadyStateButton.GetComponent<Image>().color = Color.green;
        }
        else
        {
            PlayerReadyStateButton.GetComponentInChildren<Text>().text = "NOT READY";
            PlayerReadyStateButton.GetComponent<Image>().color = Color.red;
        }
=======
>>>>>>> origin/master
    }

    public void OnNameTextChange(string text)
    {
        CmdNameChanged(text);
    }

    public void OnNationSelectionChange(int index)
    {
        CmdNationalityChanged(index);
    }

<<<<<<< HEAD
    public void OnReadyStateValueChange(bool is_ready)
    {
        CmdReadyStateChanged(is_ready);
    }

=======
>>>>>>> origin/master
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
<<<<<<< HEAD
    public void CmdReadyStateChanged(bool is_ready)
    {
        IsReady = is_ready;
    }

    [Command]
=======
>>>>>>> origin/master
    public void CmdSendChatMessage(string message)
    {
        GameObject.Find("ChatManager").GetComponent<ChatManager>().SendChatMessage(PlayerName, message);
    }
}
