using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadingScreenManager : MonoBehaviour
{
    [HideInInspector]
    public static LoadingScreenManager Instance;

    public RectTransform ProgressBar;
    public Text ProgressMessage;

	void Start()
    {
        Instance = this;
	}
	
	void Update()
    {

	}

    public void SetProgress(float percent)
    {
        ProgressBar.sizeDelta = new Vector2(percent * 10.0f, ProgressBar.sizeDelta.y);

        if (percent == 100.0f)
            StartCoroutine(CloseLoadingScreen());
    }

    public void SetMessage(string message)
    {
        ProgressMessage.text = message;
    }

    IEnumerator CloseLoadingScreen()
    {
        while(GetComponent<CanvasGroup>().alpha > 0)
        {
            GetComponent<CanvasGroup>().alpha -= Time.deltaTime;
            yield return null;
        }
    }
}
