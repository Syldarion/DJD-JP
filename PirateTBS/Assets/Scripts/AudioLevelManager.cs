using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AudioLevelManager : MonoBehaviour
{
	void Start()
    {

	}

	void Update()
    {

	}

    public void UpdateAudioLevel(string audio_type)
    {
        switch(audio_type)
        {
            case "General":
                break;
            case "Music":
                break;
            case "Effects":
                break;
        }

        this.transform.FindChild("Level").GetComponent<Text>().text
            = this.transform.FindChild("Slider").GetComponent<Slider>().value.ToString();
    }
}
