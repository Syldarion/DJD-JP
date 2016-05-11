using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TechNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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

    public List<string> ActivationString;       //Commands to parse on activation
    public List<string> DeactivationString;     //Commands to parse on deactivation

	void Start()
    {
        Name = GetComponentInChildren<Text>().text;
        NodeCode = name.Remove(0, 8);

        TechTree.Instance.AddNode(this);
	}
	
	void Update()
    {

	}

    public void InitializeNode()
    {
        foreach (TechNode node in ChildNodes)
            TechTree.Instance.DrawLine(this, node, 10.0f);

        Icon = GetComponent<Image>();

        GetComponent<Button>().onClick.AddListener(SwitchNode);
    }

    public void SwitchNode()
    {
        if (!TechTree.Instance.ModifyingTree)
            return;

        if (IsActive)
            DeactivateNode();
        else
            ActivateNode();
    }

    public void ActivateNode()
    {
        foreach (TechNode node in ParentNodes)
            if (!node.IsActive)
                return;

        Icon.color = ActivatedColor;

        IsActive = true;

        foreach (TechNode node in ChildNodes)
            GameObject.Find(string.Format("{0}to{1}", NodeCode, node.NodeCode)).GetComponent<Image>().color = Color.yellow;

        foreach (string s in ActivationString)
            TechTree.Instance.ModifyStat(s);
    }

    public void DeactivateNode()
    {
        foreach (TechNode node in ChildNodes)
            if (node.IsActive)
                node.DeactivateNode();

        Icon.color = DeactivatedColor;

        IsActive = false;

        foreach (TechNode node in ChildNodes)
            GameObject.Find(string.Format("{0}to{1}", NodeCode, node.NodeCode)).GetComponent<Image>().color = Color.gray;

        foreach (string s in DeactivationString)
            TechTree.Instance.ModifyStat(s);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Tooltip.Instance.EnableTooltip(true);
        Tooltip.Instance.UpdateTooltip(EffectText);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Tooltip.Instance.EnableTooltip(false);
    }
}
