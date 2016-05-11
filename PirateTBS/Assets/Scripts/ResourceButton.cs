using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ResourceButton : MonoBehaviour
{
    public Text CostText;
    public Text FoodGenText;
    public Text SugarGenText;
    public Text SpiceGenText;

	void Start()
	{
		
	}
	
	void Update()
	{
		
	}

    public void PopulateButton(ResourceGenerator reference_generator)
    {
        CostText.text = string.Format("Cost: {0} Gold", reference_generator.Cost);
        FoodGenText.text = string.Format("{0} Food / Turn", reference_generator.ResourcesPerTurn[0]);
        SugarGenText.text = string.Format("{0} Sugar / Turn", reference_generator.ResourcesPerTurn[1]);
        SpiceGenText.text = string.Format("{0} Spice / Turn", reference_generator.ResourcesPerTurn[2]);
    }
}