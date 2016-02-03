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
        Debug.Log("PlayerSwitchTo");

        if (ActivePanel)
        {
            if (ActivePanel == new_panel)
                return;

            if (ActivePanel.GetComponent<CanvasGroup>())
            {
                ActivePanel.GetComponent<CanvasGroup>().alpha = 0;
                ActivePanel.GetComponent<CanvasGroup>().interactable = false;
                ActivePanel.GetComponent<CanvasGroup>().blocksRaycasts = false;
            }
        }

        ActivePanel = new_panel;

        if (ActivePanel.GetComponent<CanvasGroup>())
        {
            ActivePanel.GetComponent<CanvasGroup>().alpha = 1;
            ActivePanel.GetComponent<CanvasGroup>().interactable = true;
            ActivePanel.GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
    }
}
