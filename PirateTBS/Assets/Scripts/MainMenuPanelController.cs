using UnityEngine;
using System.Collections;

public class MainMenuPanelController : MonoBehaviour
{
    public RectTransform ActivePanel;
    public RectTransform FadingInPanel;
    public RectTransform FadingOutPanel;

    public float FadeTime;

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
            panel.GetComponent<CanvasGroup>().alpha += Time.deltaTime / FadeTime;

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
            panel.GetComponent<CanvasGroup>().alpha -= Time.deltaTime / FadeTime;

            yield return null;
        }

        FadingOutPanel.gameObject.SetActive(false);
        FadingOutPanel = null;
    }
}
