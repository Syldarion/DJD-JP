using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerInfoPanelController : MonoBehaviour
{
    RectTransform ActivePanel;

	void Start()
	{
        ActivePanel = null;
	}
	
	void Update()
	{

	}

    public void SwitchTo(RectTransform new_panel)
    {
        if (!ActivePanel || ActivePanel == new_panel || !new_panel.GetComponent<CanvasGroup>())
            return;

        ActivePanel.GetComponent<CanvasGroup>().alpha = 0;
        ActivePanel.GetComponent<CanvasGroup>().interactable = false;
        ActivePanel.GetComponent<CanvasGroup>().blocksRaycasts = false;

        ActivePanel = new_panel;

        ActivePanel.GetComponent<CanvasGroup>().alpha = 1;
        ActivePanel.GetComponent<CanvasGroup>().interactable = true;
        ActivePanel.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
}
