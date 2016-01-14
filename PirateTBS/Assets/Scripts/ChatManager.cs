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
        ChatMessageScript newChatMessage = Instantiate(ChatMessagePrefab).GetComponent<ChatMessageScript>();

        newChatMessage.Sender = sender;
        newChatMessage.Message = message;
    }
}
