using UnityEngine;
using System.Collections;

public class MainMenuPanelController : MonoBehaviour
{
    [HideInInspector]
    public static MainMenuPanelController Instance;

    public RectTransform ActivePanel;               //Reference to panel that is currently active
    public RectTransform FadingInPanel;             //Reference to panel that is currently fading in
    public RectTransform FadingOutPanel;            //Reference to panel that is currently fading out

    public float FadeTime;                          //Time in seconds it takes to fade a panel

    void Start()
    {
        Instance = this;
    }

    /// <summary>
    /// Switch to new panel, fading it in, and fading out currently active panel
    /// </summary>
    /// <param name="new_panel">Panel to switch to</param>
    public void SwitchTo(RectTransform new_panel)
    {
        if (ActivePanel != new_panel)
        {
            if (ActivePanel && !FadingOutPanel)
                StartCoroutine(FadeOutPanel(ActivePanel));

            if (!FadingInPanel)
                StartCoroutine(FadeInPanel(new_panel));
        }
        else if (!FadingOutPanel)
            StartCoroutine(FadeOutPanel(ActivePanel));
    }

    /// <summary>
    /// Fade in panel
    /// </summary>
    /// <param name="panel">Panel to fade in</param>
    /// <returns></returns>
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

    /// <summary>
    /// Fade out panel
    /// </summary>
    /// <param name="panel">Panel to fade out</param>
    /// <returns></returns>
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
