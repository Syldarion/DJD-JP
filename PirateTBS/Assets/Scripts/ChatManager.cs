using UnityEngine;
using UnityEngine.Networking;

public class ChatManager : NetworkBehaviour
{
    public GameObject ChatMessagePrefab;
    
    [Command]
    public void CmdNewMessage(string sender, string message)
    {
        RpcSendChatMessage(sender, message);
    }

    [ClientRpc]
    public void RpcSendChatMessage(string sender, string message)
    {
        ChatMessage newChatMessage = Instantiate(ChatMessagePrefab).GetComponent<ChatMessage>();

        newChatMessage.Sender = sender;
        newChatMessage.Message = message;
    }
}
