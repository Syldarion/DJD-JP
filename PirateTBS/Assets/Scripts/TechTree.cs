using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TechTree : MonoBehaviour
{
    [HideInInspector]
    public static TechTree Instance;

    public Dictionary<string, TechNode> Nodes;

    public bool ModifyingTree;

    public CanvasGroup ActiveTechPanel = null;
    public Image LinePrefab;

    //Combat tech modifiers

    public float HealthMultiplier = 1.0f;
    public float DamageMultiplier = 1.0f;
    public float ReloadSpeedMultiplier = 1.0f;
    public float MaxCannonModifier = 0.0f;
    public float DodgeChanceMultiplier = 1.0f;

    //Mercantile tech modifiers

    public float CargoSizeMultiplier = 1.0f;
    public float CrewNeedMultiplier = 1.0f;
    public float ResourceCostMultiplier = 1.0f;
    public float ResourceGenMultiplier = 1.0f;
    public float ShipCostMultiplier = 1.0f;

    //Exploration tech modifiers

    public float ShipSpeedMultiplier = 1.0f;
    public float ReputationGainMultiplier = 1.0f;
    public float MaxFleetSizeModifier = 0.0f;
    public float ViewRadiusModifier = 0.0f;
    public float MoraleMultiplier = 1.0f;

	void Start()
    {
        Instance = this;
        Nodes = new Dictionary<string, TechNode>();

        ModifyingTree = false;
	}
	
	void Update()
    {

	}

    void SetupDelegates()
    {

    }

    public void ModifyStat(string modify_string)
    {
        if (modify_string == string.Empty)
            return;

        string[] split = modify_string.Split(' ');

        if (split.Length != 2)
            return;

        string var = split[0];
        float val = float.Parse(split[1]);

        GetType().GetField(var).SetValue(this, val);
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
