using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public delegate void OnActivateDelegate();

public class TechNode : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public string Name;                         //Name of technology
    public string NodeCode;                     //Code representing node eg. C01
    public string EffectText;                   //Description of tech effect
    public Image Icon;                          //Image representation of node

    public Color ActivatedColor;                //Color of node icon while activated
    public Color DeactivatedColor;              //Color of node icon while deactivated

    public List<TechNode> ParentNodes;          //Nodes that need to be active to use this node
    public List<TechNode> ChildNodes;           //Nodes that require this node to be active
    public bool IsActive;                       //Is this an active node?

    public OnActivateDelegate OnActivation;     //Delegate to call when node is activated
    public OnActivateDelegate OnDeactivation;   //Delegate to call when node is deactivated

	void Start()
    {
        NodeCode = name.Remove(0, 8);
        TechTree.Instance.Nodes[NodeCode] = this;

        Icon = GetComponent<Image>();
	}
	
	void Update()
    {

	}

    public void ActivateNode()
    {
        foreach (TechNode node in ParentNodes)
            if (!node.IsActive)
                return;

        if (IsActive)
            return;

        Icon.color = ActivatedColor;

        IsActive = true;
        if (OnActivation != null)
            OnActivation.Invoke();

        foreach (TechNode node in ChildNodes)
            GameObject.Find(string.Format("{0}to{1}", NodeCode, node.NodeCode)).GetComponent<Image>().color = Color.yellow;
    }

    public void DeactivateNode()
    {
        if (!IsActive)
            return;

        Icon.color = DeactivatedColor;

        IsActive = false;

        if (OnDeactivation != null)
            OnDeactivation.Invoke();

        foreach (TechNode node in ChildNodes)
            GameObject.Find(string.Format("{0}to{1}", NodeCode, node.NodeCode)).GetComponent<Image>().color = Color.black;

        foreach (TechNode node in ChildNodes)
            node.DeactivateNode();
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
