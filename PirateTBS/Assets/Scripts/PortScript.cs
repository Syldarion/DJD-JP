using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PortScript : MonoBehaviour
{
    //Ports have to be networked brah
    //Time to start that shit

    string portName;
    CanvasGroup portPanel;

	void Start()
    {
        portPanel = GameObject.Find("PortPanel").GetComponent<CanvasGroup>();
    }
	
	void Update()
    {

    }

    void OnMouseEnter()
    {
        portPanel.alpha = 1;
        portPanel.GetComponent<Text>().text = portName;
    }

    void OnMouseExit()
    {
        portPanel.alpha = 0;
    }
}
