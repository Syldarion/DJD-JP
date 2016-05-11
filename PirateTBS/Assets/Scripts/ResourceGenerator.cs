using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ResourceGenerator : MonoBehaviour
{
    public int[] ResourcesPerTurn = new int[3];     //0 - Food, 1 - Sugar, 2 - Spice
    public Cargo GeneratedResources;                //Current contents of the generator
    public PlayerScript Owner = null;               //Current generator owner
    public int Cost = 0;                            //Cost of the generator

    public Text InfoText;

	void Start()
	{
        InitializeGenerator();
	}
	
	void Update()
	{
		
	}

    public void InitializeGenerator()
    {
        ResourcesPerTurn[0] = Random.Range(10, 20);
        ResourcesPerTurn[1] = Random.Range(5, 8);
        ResourcesPerTurn[2] = Random.Range(1, 2);

        GeneratedResources = new Cargo(ResourcesPerTurn[0], 0, ResourcesPerTurn[1], ResourcesPerTurn[2], 0);

        Cost = ResourcesPerTurn[0] + (ResourcesPerTurn[1] * 2) + (ResourcesPerTurn[2] * 10);

        InfoText.text = this.ToString();
    }

    public void OnTurnEnd()
    {
        GeneratedResources.Food += ResourcesPerTurn[0];
        GeneratedResources.Sugar += ResourcesPerTurn[1];
        GeneratedResources.Spice += ResourcesPerTurn[2];
    }

    public void Purchase()
    {

    }

    public override string ToString()
    {
        return string.Format("{0} ({1}) / {2} ({3}) / {4} ({5})", GeneratedResources.Food, ResourcesPerTurn[0], GeneratedResources.Sugar, ResourcesPerTurn[1], GeneratedResources.Spice, ResourcesPerTurn[2]);
    }
}