using UnityEngine;
using UnityEngine.Networking;

public class ChatManager : NetworkBehaviour
{
    [HideInInspector]
    public static ChatManager Instance;

    public RectTransform MessageList;               //Reference to container for chat message
    public GameObject ChatMessagePrefab;            //Reference to prefab for instantiating chat messages
    
    void Start()
    {
        Instance = this;
    }

    /// <summary>
    /// Start sending message to server
    /// </summary>
    /// <param name="message">Message text</param>
    public void InitializeSend(string message)
    {
        CustomLobbyPlayer.MyPlayer.CmdSendMessage(message);
    }

    /// <summary>
    /// Create new message
    /// </summary>
    /// <param name="sender">Name of sender</param>
    /// <param name="message">Message text</param>
    public void NewMessage(string sender, string message)
    {
        ChatMessage newChatMessage = Instantiate(ChatMessagePrefab).GetComponent<ChatMessage>();

        newChatMessage.Sender = sender;
        newChatMessage.Message = message;

        NetworkServer.Spawn(newChatMessage.gameObject);
    }

}
