using UnityEngine;
using BeardedManStudios.Network;

public class ChatManager : NetworkedMonoBehavior
{
    public GameObject ChatMessagePrefab;
    
    public void NewMessage(string message)
    {
        RPC("SendChatMessage", Networking.PrimarySocket.Me.Name, message);
    }
    
    [BRPC]
    public void SendChatMessage(string sender, string message)
    {
        ChatMessage newChatMessage = Instantiate(ChatMessagePrefab).GetComponent<ChatMessage>();

        newChatMessage.Sender = sender;
        newChatMessage.Message = message;
    }
}
