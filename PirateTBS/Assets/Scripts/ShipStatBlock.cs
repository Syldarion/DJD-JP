using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShipStatBlock : MonoBehaviour
{
	void Start()
	{

	}
	
	void Update()
	{

	}

    public void PopulateStatBlock(ShipScript ship)
    {
        transform.FindChild("ShipNameText").GetComponent<Text>().text = ship.name;
        transform.FindChild("ShipTypeText").GetComponent<Text>().text = ship.ShipType;

        Transform ship_stats_transform = transform.FindChild("ShipStats");
        ship_stats_transform.FindChild("HealthText").GetComponent<Text>().text = string.Format("{0} | {1}", ship.HullHealth, ship.SailHealth);
        ship_stats_transform.FindChild("CargoText").GetComponent<Text>().text = string.Format("{0}/{1}", ship.Cargo.Size().ToString(), ship.CargoSpace.ToString());
        ship_stats_transform.FindChild("SpeedText").GetComponent<Text>().text = ship.FullSpeed.ToString();
        ship_stats_transform.FindChild("CannonText").GetComponent<Text>().text = ship.Cannons.ToString();
    }
}