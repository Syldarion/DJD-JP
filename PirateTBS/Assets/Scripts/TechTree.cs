using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TechTree : MonoBehaviour
{
    [HideInInspector]
    public static TechTree Instance;

    public List<TechNode> Nodes;

    public bool ModifyingTree;

    public CanvasGroup ActiveTechPanel = null;
    public Image LinePrefab;

    public int TechCount = 30;

	void Start()
    {
        Instance = this;
        Nodes = new List<TechNode>();

        ModifyingTree = false;
	}
	
	void Update()
    {

	}

    public void AddNode(TechNode node)
    {
        Nodes.Add(node);
        if (Nodes.Count >= TechCount)
            foreach (TechNode tnode in Nodes)
                tnode.InitializeNode();
    }

    public void ModifyStat(string modify_string)
    {
        string[] split = modify_string.Split(' ');
        if (split.Length != 4)
            return;

        string rest = string.Format("{0} {1} {2}", split[1], split[2], split[3]);

        switch(split[0])
        {
            case "Ship":
                foreach (Fleet f in PlayerScript.MyPlayer.Fleets)
                    foreach (Ship s in f.Ships)
                        s.CmdUpdateStat(rest);
                break;
            case "Fleet":
                foreach (Fleet f in PlayerScript.MyPlayer.Fleets)
                    f.CmdUpdateStat(rest);
                break;
            case "Player":
                PlayerScript.MyPlayer.CmdUpdateStat(rest);
                break;
            default:
                return;
        }
    }

    public void SwitchTechPanel(CanvasGroup new_panel)
    {
        ActiveTechPanel.alpha = 0;
        ActiveTechPanel.interactable = false;
        ActiveTechPanel.blocksRaycasts = false;

        ActiveTechPanel = new_panel;

        ActiveTechPanel.alpha = 1;
        ActiveTechPanel.interactable = true;
        ActiveTechPanel.blocksRaycasts = true;
    }

    public void DrawLine(TechNode from, TechNode to, float width)
    {
        Vector3 diff = to.transform.position - from.transform.position;

        Image new_line = Instantiate(LinePrefab);
        new_line.rectTransform.SetParent(from.transform.parent);
        new_line.rectTransform.SetAsFirstSibling();
        new_line.rectTransform.sizeDelta = new Vector2(diff.magnitude, width);
        new_line.rectTransform.pivot = new Vector2(0.0f, 0.5f);
        new_line.rectTransform.position = from.transform.position;

        float angle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        new_line.transform.rotation = Quaternion.Euler(0, 0, angle);

        new_line.name = string.Format("{0}to{1}", from.NodeCode, to.NodeCode);
    }
}
