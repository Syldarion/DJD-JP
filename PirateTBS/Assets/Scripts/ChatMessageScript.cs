using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ChatMessageScript : NetworkBehaviour
{
    public ChatFilter Filter;

    [SyncVar]
    public string Sender;

    [SyncVar]
    public string Message;

	void Start()
    {
        Filter = GameObject.Find("ChatFilter").GetComponent<ChatFilter>();

        GameObject chatParent = GameObject.Find("LobbyChat/Messages");

        transform.SetParent(chatParent.transform, false);
        transform.localScale = new Vector3(1, 1, 1);

        if (chatParent.transform.childCount > 20)
            Destroy(chatParent.transform.GetChild(0).gameObject);

        Message = Filter.ProfanityFilter(Message);
        Message = Filter.PirateFilter(Message);

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
