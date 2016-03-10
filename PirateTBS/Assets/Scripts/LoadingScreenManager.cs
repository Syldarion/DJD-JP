using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadingScreenManager : MonoBehaviour
{
    [HideInInspector]
    public static LoadingScreenManager Instance;

    public RectTransform ProgressBar;
    public Text LoadingMessage;

	void Start()
    {
        Instance = this;
	}
	
	void Update()
    {

	}

    public void SetProgress(float progress)
    {
        StartCoroutine(MoveProgressBar(progress));
    }

    public void SetMessage(string message)
    {
        LoadingMessage.text = message;
    }

    IEnumerator MoveProgressBar(float progress)
    {
        float progress_bar_width = ProgressBar.sizeDelta.x;
        while(progress_bar_width < progress * 10.0f - 5.0f)
        {
            progress_bar_width = ProgressBar.sizeDelta.x;
            progress_bar_width = Mathf.Lerp(progress_bar_width, progress * 10.0f, 0.95f * Time.deltaTime);
            ProgressBar.sizeDelta = new Vector2(progress_bar_width, ProgressBar.sizeDelta.y);

            Debug.Log(string.Format("{0} : {1}", progress_bar_width, ProgressBar.sizeDelta.x));

            yield return null;
        }

        if(progress == 100.0f)
        {
            SetMessage("Done!");
            StartCoroutine(CloseLoadingScreen());
        }
    }

    IEnumerator CloseLoadingScreen()
    {
        while(GetComponent<CanvasGroup>().alpha > 0)
        {
            GetComponent<CanvasGroup>().alpha -= Time.deltaTime;
            yield return null;
        }

        GetComponent<CanvasGroup>().interactable = false;
        GetComponent<CanvasGroup>().blocksRaycasts = false;

        StopAllCoroutines();
    }
}
