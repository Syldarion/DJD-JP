using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using BeardedManStudios.Network;

public class ChatManager : NetworkedMonoBehavior
{
    public GameObject ChatMessagePrefab;

	void Start()
    {

	}
	
	void Update()
    {

	}
    
    public void SendChatMessage(string sender, string message)
    {
        ChatMessageScript newChatMessage = Instantiate(ChatMessagePrefab).GetComponent<ChatMessageScript>();

        newChatMessage.Sender = sender;
        newChatMessage.Message = message;

        Networking.Instantiate(newChatMessage.gameObject);
    }
}
