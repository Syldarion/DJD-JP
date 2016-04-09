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

    //Combat tech modifiers

    public float HealthMultiplier = 1.0f;
    public float DamageMultiplier = 1.0f;
    public float ReloadSpeedMultiplier = 1.0f;
    public int MaxCannonModifier = 0;
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
    public int MaxFleetSizeModifier = 0;
    public int ViewRadiusModifier = 0;
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
}
