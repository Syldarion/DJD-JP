using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Slider))]
public class VisibleValueSlider : MonoBehaviour
{
    public Text ValueText;

    public void UpdateValue(float new_value)
    {
        ValueText.text = ((int)new_value).ToString();
    }
}
