using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadingScreenManager : MonoBehaviour
{
    [HideInInspector]
    public static LoadingScreenManager Instance;

    public RectTransform ProgressBar;           //Reference to progress bar UI
    public Text ProgressMessage;                //Reference to text showing progress message

	void Start()
    {
        Instance = this;
	}
	
	void Update()
    {

	}

    /// <summary>
    /// Set progress of progress bar
    /// </summary>
    /// <param name="percent">Current progress from 0-100%</param>
    public void SetProgress(float percent)
    {
        ProgressBar.sizeDelta = new Vector2(percent * 10.0f, ProgressBar.sizeDelta.y);

        if (percent == 100.0f)
            StartCoroutine(CloseLoadingScreen());
    }

    /// <summary>
    /// Set message of progress bar
    /// </summary>
    /// <param name="message">Message to display</param>
    public void SetMessage(string message)
    {
        ProgressMessage.text = message;
    }

    /// <summary>
    /// Fades out loading screen
    /// </summary>
    /// <returns></returns>
    IEnumerator CloseLoadingScreen()
    {
        while(GetComponent<CanvasGroup>().alpha > 0)
        {
            GetComponent<CanvasGroup>().alpha -= Time.deltaTime;
            yield return null;
        }
    }
}
