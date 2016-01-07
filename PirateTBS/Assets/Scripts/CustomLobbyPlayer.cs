using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using BeardedManStudios.Network;

public class CustomLobbyPlayer : NetworkedMonoBehavior
{
    //public GameObject LobbyPlayerPanel;
    public InputField PlayerNameText;
    public Image PlayerNationalityFlag;
    public Dropdown PlayerNationalitySelection;
    public Toggle PlayerReadyStateButton;

    public Sprite[] Flags;
    public InputField ChatMessageInput;
    
    public string PlayerName = "";
    public int NationalityIndex = 0;
    public bool IsReady = false;

    ColorBlock NotReadyColorBlock;
    ColorBlock ReadyColorBlock;

    protected override void NetworkStart()
    {
        base.NetworkStart();

        PlayerNameText = transform.FindChild("PlayerNameInput").GetComponent<InputField>();
        PlayerNationalityFlag = transform.FindChild("PlayerNationalityFlag").GetComponent<Image>();
        PlayerNationalitySelection = transform.FindChild("PlayerNationalitySelection").GetComponent<Dropdown>();
        PlayerReadyStateButton = transform.FindChild("PlayerReadyStateButton").GetComponent<Toggle>();

        RPC("OnNameChange", PlayerName);
        RPC("OnNationalityChange", NationalityIndex);
        RPC("OnReadyStateChange", IsReady);
    }

    void Start()
    {

    }

    void OnPanelCreate(GameObject new_panel)
    {
        if(new_panel != null)
        {
            Debug.Log("New panel exists");

            new_panel.transform.SetParent(GameObject.Find("LobbyPlayerList").transform, false);
            PlayerNameText = new_panel.GetComponentInChildren<InputField>();
            PlayerNationalityFlag = new_panel.GetComponentInChildren<Image>();
            PlayerNationalitySelection = new_panel.GetComponentInChildren<Dropdown>();
            PlayerReadyStateButton = new_panel.GetComponentInChildren<Toggle>();

            OnNameChange(PlayerName);
            OnNationalityChange(NationalityIndex);
            OnReadyStateChange(IsReady);
        }
    }

    public void UpdateName(string name)
    {
        RPC("OnNameChange", name);
    }

    public void UpdateNationality(int nation_index)
    {
        RPC("OnNationalityChange", nation_index);
    }

    public void UpdateReadyState(bool is_ready)
    {
        RPC("OnReadyStateChange", is_ready);
    }

    [BRPC]
    public void OnNameChange(string name)
    {
        Debug.Log(name);
        PlayerName = name;
        PlayerNameText.text = PlayerName;
    }

    [BRPC]
    public void OnNationalityChange(int nation_index)
    {
        Debug.Log(nation_index);
        NationalityIndex = nation_index;
        PlayerNationalitySelection.value = NationalityIndex;
        Debug.Log(PlayerNationalitySelection.value);
        PlayerNationalityFlag.sprite = Flags[nation_index];
    }

    [BRPC]
    public void OnReadyStateChange(bool is_ready)
    {
        Debug.Log(is_ready);
        IsReady = is_ready;

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
    }
}
