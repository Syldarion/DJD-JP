using UnityEngine;
using UnityEngine.Networking;

public class ChatManager : NetworkBehaviour
{
    [HideInInspector]
    public static ChatManager Instance;

    public RectTransform MessageList;
    public GameObject ChatMessagePrefab;
    
    void Start()
    {
        Instance = this;
    }

    public void InitializeSend(string message)
    {
        CustomLobbyPlayer.MyPlayer.CmdSendMessage(message);
    }

    public void NewMessage(string sender, string message)
    {
        ChatMessage newChatMessage = Instantiate(ChatMessagePrefab).GetComponent<ChatMessage>();

        newChatMessage.Sender = sender;
        newChatMessage.Message = message;

        NetworkServer.Spawn(newChatMessage.gameObject);
    }

}
