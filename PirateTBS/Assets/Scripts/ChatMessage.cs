using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ChatMessage : NetworkBehaviour
{
    public ChatFilter Filter;           //Filter to apply to chat message

    [SyncVar]
    public string Sender;               //Sender of message
    [SyncVar]
    public string Message;              //Message text

    void Start()
    {
        GameObject chatParent = ChatManager.Instance.MessageList.gameObject;

        transform.SetParent(chatParent.transform, false);
        transform.localScale = new Vector3(1, 1, 1);

        if (chatParent.transform.childCount > 20)
            Destroy(chatParent.transform.GetChild(0).gameObject);

        UpdateMessage();
    }

    /// <summary>
    /// Updates chat message with values from server
    /// </summary>
    public void UpdateMessage()
    {
        transform.GetChild(0).GetComponent<Text>().text = Sender;
        transform.GetChild(1).GetComponent<Text>().text = Message;
    }
}
