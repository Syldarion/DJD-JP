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
