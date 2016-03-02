using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerInfoPanelController : MonoBehaviour
{
    [HideInInspector]
    public static PlayerInfoPanelController Instance;

    RectTransform ActivePanel;

	void Start()
	{
        Instance = this;
        ActivePanel = null;
	}
	
	void Update()
	{

	}

    public void SwitchTo(RectTransform new_panel)
    {
        if (ActivePanel)
        {
            if (ActivePanel == new_panel)
                return;

            if (ActivePanel.GetComponent<CanvasGroup>())
                PanelUtilities.DeactivatePanel(ActivePanel.GetComponent<CanvasGroup>());
        }

        ActivePanel = new_panel;

        if (ActivePanel.GetComponent<CanvasGroup>())
            PanelUtilities.ActivatePanel(ActivePanel.GetComponent<CanvasGroup>());
    }
}
