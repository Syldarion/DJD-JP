using UnityEngine;
using UnityEngine.Networking;

public class ChatManager : NetworkBehaviour
{
    [HideInInspector]
    public static ChatManager Instance;

    public GameObject ChatMessagePrefab;
    
    void Start()
    {
        Instance = this;
    }

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
