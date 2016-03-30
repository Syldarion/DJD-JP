using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ChatMessage : NetworkBehaviour
{
    public ChatFilter Filter;

    [SyncVar]
    public string Sender;
    [SyncVar]
    public string Message;

    void Start()
    {
        GameObject chatParent = ChatManager.Instance.MessageList.gameObject;

        transform.SetParent(chatParent.transform, false);
        transform.localScale = new Vector3(1, 1, 1);

        if (chatParent.transform.childCount > 20)
            Destroy(chatParent.transform.GetChild(0).gameObject);

        UpdateMessage();
    }

    public void UpdateMessage()
    {
        transform.GetChild(0).GetComponent<Text>().text = Sender;
        transform.GetChild(1).GetComponent<Text>().text = Message;
    }
}
