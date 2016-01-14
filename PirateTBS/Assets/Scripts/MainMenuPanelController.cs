using UnityEngine;
using System.Collections;

public class MainMenuPanelController : MonoBehaviour
{
    public RectTransform ActivePanel;
    public RectTransform FadingInPanel;
    public RectTransform FadingOutPanel;

    Vector2 AnchorPosition;
    Vector2 BasePosition;

    float fadeTime;

	void Start()
    {
        ActivePanel = null;
        FadingInPanel = null;
        FadingOutPanel = null;

        AnchorPosition = new Vector2(-400, this.transform.position.y);
        BasePosition = new Vector2(0, this.transform.position.y);

        fadeTime = 0.25f;
	}
	
	void Update()
    {

	}

    public void SwitchTo(RectTransform new_panel)
    {
        if (ActivePanel != new_panel)
        {
            if (ActivePanel && !FadingOutPanel)
                StartCoroutine("FadeOutPanel", ActivePanel);

            if (!FadingInPanel)
                StartCoroutine("FadeInPanel", new_panel);
        }
        else if (!FadingOutPanel)
            StartCoroutine("FadeOutPanel", ActivePanel);
    }

    public IEnumerator FadeInPanel(RectTransform panel)
    {
        FadingInPanel = panel;
        FadingInPanel.gameObject.SetActive(true);

        while(panel.GetComponent<CanvasGroup>().alpha < 1)
        {
            panel.GetComponent<CanvasGroup>().alpha += Time.deltaTime / fadeTime;
            panel.transform.position = Vector3.Lerp(this.transform.position, AnchorPosition, 0.5f);

            yield return null;
        }

        ActivePanel = panel;
        FadingInPanel = null;
    }

    public IEnumerator FadeOutPanel(RectTransform panel)
    {
        FadingOutPanel = panel;
        ActivePanel = null;

        while (panel.GetComponent<CanvasGroup>().alpha > 0)
        {
            panel.GetComponent<CanvasGroup>().alpha -= Time.deltaTime / fadeTime;
            panel.transform.position = Vector3.Lerp(this.transform.position, BasePosition, 0.5f);

            yield return null;
        }

        FadingOutPanel.gameObject.SetActive(false);
        FadingOutPanel = null;
    }
}
