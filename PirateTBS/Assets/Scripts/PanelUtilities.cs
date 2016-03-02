using UnityEngine;
using System.Collections;

public class PanelUtilities : MonoBehaviour
{
	void Start()
    {

	}
	
	void Update()
    {
        	
    }

    /// <summary>
    /// Activates a panel, revealing it and making it interactable
    /// </summary>
    /// <param name="panel">Panel to activate</param>
    public static void ActivatePanel(CanvasGroup panel)
    {
        panel.alpha = 1;
        panel.interactable = true;
        panel.blocksRaycasts = true;
    }

    /// <summary>
    /// Deactivates a panel, hiding it and making it non-interactable
    /// </summary>
    /// <param name="panel">Panel to deactivate</param>
    public static void DeactivatePanel(CanvasGroup panel)
    {
        panel.alpha = 0;
        panel.interactable = false;
        panel.blocksRaycasts = false;
    }

    /// <summary>
    /// Reveals a panel
    /// </summary>
    /// <param name="panel">Panel to reveal</param>
    public static void RevealPanel(CanvasGroup panel)
    {
        panel.alpha = 1;
    }

    /// <summary>
    /// Hides a panel
    /// </summary>
    /// <param name="panel">Panel to hide</param>
    public static void HidePanel(CanvasGroup panel)
    {
        panel.alpha = 0;
    }
}