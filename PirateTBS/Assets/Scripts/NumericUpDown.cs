using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class NumericUpDown : MonoBehaviour, IScrollHandler
{
    public int Value;               //Current value for control
    public int MaxValue;            //Max value for control

	void Start()
	{
        UpdateValue(0);
	}

	void Update()
	{
		
	}

    /// <summary>
    /// Set maximum value of the control
    /// </summary>
    /// <param name="value">New max value</param>
    public void SetMaxValue(int value)
    {
        MaxValue = value;
    }

    /// <summary>
    /// Update the current value of the control
    /// </summary>
    /// <param name="value">String representation of new value</param>
    public void UpdateValue(string value)
    {
        if (value != string.Empty)
            UpdateValue(int.Parse(value));
    }

    /// <summary>
    /// Update the current value of the control
    /// </summary>
    /// <param name="value">New value</param>
    public void UpdateValue(int value)
    {
        Value = Mathf.Clamp(value, 0, MaxValue);
        GetComponentInChildren<InputField>().text = Value.ToString();
    }

    /// <summary>
    /// Increase current value of control by 1
    /// </summary>
    public void IncreaseValue()
    {
        UpdateValue(++Value);
    }

    /// <summary>
    /// Decrease current value of control by 1
    /// </summary>
    public void DecreaseValue()
    {
        UpdateValue(--Value);
    }

    public void OnScroll(PointerEventData data)
    {
        if (data.scrollDelta.y > 0)
            UpdateValue(++Value);
        else if (data.scrollDelta.y < 0)
            UpdateValue(--Value);
    }
}
