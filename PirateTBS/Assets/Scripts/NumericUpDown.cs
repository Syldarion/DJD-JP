using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class NumericUpDown : MonoBehaviour, IScrollHandler
{
    public int Value;
    public int MaxValue;

	void Start()
	{
        UpdateValue(0);
	}

	void Update()
	{
		
	}

    public void SetMaxValue(int value)
    {
        MaxValue = value;
    }

    public void UpdateValue(string value)
    {
        if (value != string.Empty)
            UpdateValue(int.Parse(value));
    }

    public void UpdateValue(int value)
    {
        Value = Mathf.Clamp(value, 0, MaxValue);
        GetComponentInChildren<InputField>().text = Value.ToString();
    }

    public void IncreaseValue()
    {
        UpdateValue(++Value);
    }

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
