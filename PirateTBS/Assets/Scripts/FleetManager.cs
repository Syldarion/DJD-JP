using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FleetManager : MonoBehaviour
{
    FleetScript FleetA, FleetB;

	void Start()
	{
		
	}

	void Update()
	{
		
	}

    public void TransferShip(FleetScript fleet_from, FleetScript fleet_to, ShipScript ship)
    {
        if(fleet_from.Ships.Contains(ship))
        {
            fleet_to.AddShip(ship);
            fleet_from.Ships.Remove(ship);
        }
    }
}