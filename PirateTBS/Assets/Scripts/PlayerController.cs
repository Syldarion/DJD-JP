using UnityEngine;
using System.Collections;
using BeardedManStudios.Network;

public class PlayerController : NetworkedMonoBehavior
{
    public GameObject ShipPrefab;

    PortScript PortToSpawnAt;

	void Start()
    {
        PortScript[] ports = GameObject.Find("Grid").GetComponent<HexGrid>().ports.ToArray();
        Debug.Log(ports.Length);
        PortToSpawnAt = ports[Random.Range(0, ports.Length - 1)];

        if (Networking.PrimarySocket.Connected)
            SpawnShip();
        else
            Networking.PrimarySocket.connected += SpawnShip;
	}
	
	void Update()
    {

	}

    void SpawnShip()
    {
        Networking.PrimarySocket.connected -= SpawnShip;
        Networking.Instantiate(ShipPrefab, NetworkReceivers.AllBuffered, callback: OnShipSpawn);
    }

    void OnShipSpawn(SimpleNetworkedMonoBehavior new_ship)
    {
        MoveShip(new_ship.GetComponent<ShipScript>(), PortToSpawnAt.SpawnTile);
    }

    void MoveShip(ShipScript ship, HexTile new_hex)
    {
        ship.transform.SetParent(new_hex.transform, false);
        ship.transform.localPosition = new Vector3(0.0f, 0.25f, 0.0f);
    }
}
