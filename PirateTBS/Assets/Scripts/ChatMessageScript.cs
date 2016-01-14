using UnityEngine;
using UnityEngine.UI;

public class ChatMessageScript : MonoBehaviour
{
    public ChatFilter Filter;
    public string Sender;
    public string Message;

	void Start()
    {
        GameObject chatParent = GameObject.Find("LobbyChat/Messages");

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
