using UnityEngine;
<<<<<<< HEAD
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class FleetScript : NetworkBehaviour
{
    public ShipScript Flagship { get; private set; }
    public List<ShipScript> Ships { get; private set; }

	void Start()
    {
        Ships = new List<ShipScript>();
=======
using System.Collections;

public class FleetScript : MonoBehaviour
{
	void Start()
    {
        	
>>>>>>> origin/master
	}

	void Update()
    {

	}
<<<<<<< HEAD

    public void SetFlagship(ShipScript ship)
    {
        if (ship != Flagship && Ships.Contains(ship))
            Flagship = ship;
    }

    public void RemoveShip(ShipScript ship)
    {
        if (Ships.Contains(ship))
            Ships.Remove(ship);
    }

    public void AddShip(ShipScript ship)
    {
        if (!Ships.Contains(ship))
            Ships.Add(ship);
    }
=======
>>>>>>> origin/master
}
