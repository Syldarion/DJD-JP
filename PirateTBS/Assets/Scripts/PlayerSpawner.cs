using UnityEngine;
using System.Collections;
using BeardedManStudios.Network;

public class PlayerSpawner : NetworkedMonoBehavior
{
    public GameObject PlayerController;

	void Start()
    {
        if (Networking.PrimarySocket.Connected)
            PrimarySocket_connected();
        else
            Networking.PrimarySocket.connected += PrimarySocket_connected;
	}

    private void PrimarySocket_connected()
    {
        Networking.PrimarySocket.connected -= PrimarySocket_connected;

        Networking.Instantiate(PlayerController, NetworkReceivers.All, callback: SpawnController);
    }

    void Update()
    {

	}

    void SpawnController(SimpleNetworkedMonoBehavior new_controller)
    {
        new_controller.name = Networking.PrimarySocket.Me.Name + "Controller";
    }
}
