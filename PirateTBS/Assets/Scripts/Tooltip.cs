using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Tooltip : MonoBehaviour
{
    public static Tooltip tooltip;

	void Start()
	{
        tooltip = this;
	}

	void Update()
	{
        if (GetComponent<CanvasGroup>().alpha != 0)
            transform.position = Input.mousePosition + new Vector3(10.0f, -10.0f);
	}

    /// <summary>
    /// Enables / Disables the tooltip
    /// </summary>
    /// <param name="enable">true enables, false disables</param>
    public static void EnableTooltip(bool enable)
    {
        if(enable)
        {
            tooltip.GetComponent<CanvasGroup>().alpha = 1;
            tooltip.GetComponent<CanvasGroup>().interactable = true;
            tooltip.GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
        else
        {
            tooltip.GetComponent<CanvasGroup>().alpha = 0;
            tooltip.GetComponent<CanvasGroup>().interactable = false;
            tooltip.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }

    /// <summary>
    /// Set the text of the tooltip
    /// </summary>
    /// <param name="tip">Tooltip text</param>
    public static void UpdateTooltip(string tip)
    {
        tooltip.GetComponentInChildren<Text>().text = tip;
    }
}
