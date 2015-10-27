using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HostPanelScript : MonoBehaviour
{
	void Start()
    {

	}
	
	void Update()
    {

	}

    public void UpdatePlayerCount()
    {
        GameObject.Find("PlayerCountText").GetComponent<Text>().text = string.Format("Player Count: {0}",
            GameObject.Find("PlayerCountSlider").GetComponent<Slider>().value.ToString());
    }
}
