using UnityEngine;
using System.Collections;

public class MainMenuCanvasController : MonoBehaviour
{
    public RectTransform ActiveCanvas;
    public RectTransform FadingInCanvas;
    public RectTransform FadingOutCanvas;

    float fadeTime;

	void Start()
    {
        ActiveCanvas = null;
        FadingInCanvas = null;
        FadingOutCanvas = null;

        fadeTime = 0.25f;
	}
	
	void Update()
    {

	}

    public void SwitchTo(RectTransform new_canvas)
    {
        if (ActiveCanvas != new_canvas)
        {
            if (ActiveCanvas && !FadingOutCanvas)
                StartCoroutine("FadeOutCanvas", ActiveCanvas);

            if (!FadingInCanvas)
                StartCoroutine("FadeInCanvas", new_canvas);
        }
        else if (!FadingOutCanvas)
            StartCoroutine("FadeOutCanvas", ActiveCanvas);
    }

    public IEnumerator FadeInCanvas(RectTransform canvas)
    {
        FadingInCanvas = canvas;
        FadingInCanvas.gameObject.SetActive(true);

        while(canvas.GetComponent<CanvasGroup>().alpha < 1)
        {
            canvas.GetComponent<CanvasGroup>().alpha += Time.deltaTime / fadeTime;
            canvas.transform.Translate(400.0f * (Time.deltaTime / fadeTime), 0.0f, 0.0f);

            yield return null;
        }

        ActiveCanvas = canvas;
        FadingInCanvas = null;
    }

    public IEnumerator FadeOutCanvas(RectTransform canvas)
    {
        FadingOutCanvas = canvas;
        ActiveCanvas = null;

        while (canvas.GetComponent<CanvasGroup>().alpha > 0)
        {
            canvas.GetComponent<CanvasGroup>().alpha -= Time.deltaTime / fadeTime;
            canvas.transform.Translate(-400.0f * (Time.deltaTime / fadeTime), 0.0f, 0.0f);

            yield return null;
        }

        FadingOutCanvas.gameObject.SetActive(false);
        FadingOutCanvas = null;
    }
}
