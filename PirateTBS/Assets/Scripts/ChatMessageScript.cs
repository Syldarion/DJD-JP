using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using BeardedManStudios.Network;

public class ChatMessageScript : NetworkedMonoBehavior
{
    public ChatFilter Filter;
    public string Sender;
    public string Message;

    void Awake()
    {
        AddNetworkVariable(() => Sender, x => Sender = (string)x);
        AddNetworkVariable(() => Message, x => Message = (string)x);
    }

	void Start()
    {
        //Filter = GameObject.Find("ChatFilter").GetComponent<ChatFilter>();

        GameObject chatParent = GameObject.Find("LobbyChat/Messages");

        transform.SetParent(chatParent.transform, false);
        transform.localScale = new Vector3(1, 1, 1);

        if (chatParent.transform.childCount > 20)
            Destroy(chatParent.transform.GetChild(0).gameObject);
        //Message = Filter.PirateFilter(Message);

        UpdateMessage();
    }
	
	void Update()
    {

	}

    public void UpdateMessage()
    {
        transform.GetChild(0).GetComponent<Text>().text = Sender;
        transform.GetChild(1).GetComponent<Text>().text = Message;
    }
}
