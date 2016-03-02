using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Tooltip : MonoBehaviour
{
    [HideInInspector]
    public static Tooltip Instance;

	void Start()
	{
        Instance = this;
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
            PanelUtilities.ActivatePanel(Instance.GetComponent<CanvasGroup>());
        else
            PanelUtilities.DeactivatePanel(Instance.GetComponent<CanvasGroup>());
    }

    /// <summary>
    /// Set the text of the tooltip
    /// </summary>
    /// <param name="tip">Tooltip text</param>
    public static void UpdateTooltip(string tip)
    {
        Instance.GetComponentInChildren<Text>().text = tip;
    }
}
