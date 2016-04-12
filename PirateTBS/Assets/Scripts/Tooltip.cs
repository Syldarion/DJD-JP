using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Tooltip : MonoBehaviour
{
    [HideInInspector]
    public static Tooltip Instance;

    static bool active;                 //Is the tooltip currently active

    RectTransform my_transform;         //The transform of the tooltip

	void Start()
	{
        Instance = this;
        active = false;
        my_transform = GetComponent<RectTransform>();
	}

	void Update()
	{
        if (active)
        {
            if(Input.mousePosition.x <= Screen.width / 2)
                my_transform.pivot = new Vector2(0.0f, 1.0f);
            else
                my_transform.pivot = new Vector2(1.0f, 1.0f);

            transform.position = Input.mousePosition + new Vector3(10.0f - (10.0f * my_transform.pivot.x), -10.0f);
        }
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

        active = enable;
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
