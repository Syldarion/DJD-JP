using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System;

public delegate void OnActivateDelegate();

public class TechNode : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public string Name;
    public string EffectText;
    public Image Icon;

    public Color ActivatedColor;
    public Color DeactivatedColor;

    public TechNode PreliminaryNode;
    public bool IsActive;

    public OnActivateDelegate OnActivation;

	void Start()
    {
        if (Name != string.Empty)
            TechTree.Instance.Nodes[Name] = this;
        else
            Destroy(gameObject);
	}
	
	void Update()
    {

	}

    public void ActivateNode()
    {
        if (IsActive || (PreliminaryNode && !PreliminaryNode.IsActive))
            return;

        Icon.color = ActivatedColor;

        IsActive = true;
        OnActivation.Invoke();
    }

    public void DeactivateNode()
    {
        Icon.color = DeactivatedColor;

        IsActive = false;
    }

    public void AddActivationAction(OnActivateDelegate action)
    {
        OnActivation += action;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!TechTree.Instance.ModifyingTree)
            return;

        if (IsActive)
            DeactivateNode();
        else
            ActivateNode();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Tooltip.EnableTooltip(true);
        Tooltip.UpdateTooltip(string.Format("{0}: {1}", Name, EffectText));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Tooltip.EnableTooltip(false);
    }
}
