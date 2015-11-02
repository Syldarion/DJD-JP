using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class ChatMessageScript : NetworkBehaviour
{
    [SyncVar]
    public string Sender;

    [SyncVar]
    public string Message;

	void Start()
    {
        transform.SetParent(GameObject.Find("LobbyChatPanel").transform, false);
        transform.localScale = new Vector3(1, 1, 1);

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
