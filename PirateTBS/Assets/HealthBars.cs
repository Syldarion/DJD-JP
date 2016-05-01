using UnityEngine;
using UnityEngine.UI;

public class HealthBars : MonoBehaviour {

    public GameObject sailHealthBar;
    public GameObject HullHealthBar;
    public Text ShipName;

    public float sailHealth;
    public float hullHealth;

    public Ship refship;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        UpdateHealthBars();
	}

    public void UpdateHealthBars()
    {
        if (refship)
        {
            sailHealth = refship.SailHealth / refship.SailMaxHealth;
            hullHealth = refship.HullHealth / refship.HullMaxHealth;
            if (sailHealthBar && HullHealthBar)
            {
                sailHealthBar.transform.localScale = new Vector3(sailHealth, sailHealthBar.transform.localScale.y, sailHealthBar.transform.localScale.z);
                HullHealthBar.transform.localScale = new Vector3(hullHealth, HullHealthBar.transform.localScale.y, sailHealthBar.transform.localScale.z);
            }
        }
    }
    public void SetRefship(Ship ship)
    {
        refship = ship;
        if (refship)
        {
            ShipName.text = refship.name;
        }
    }
}
