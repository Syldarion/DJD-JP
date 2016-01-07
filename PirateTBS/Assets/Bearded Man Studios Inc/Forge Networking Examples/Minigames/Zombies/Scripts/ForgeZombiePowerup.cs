﻿using BeardedManStudios.Network;
using UnityEngine;
using System.Collections;

public class ForgeZombiePowerup : NetworkedMonoBehavior 
{
	public void OnTriggerEnter(Collider other)
	{
		if (NetworkingManager.IsOnline && !NetworkingManager.Socket.IsServer)
			return;

		if (other.gameObject.name.Contains("CubePlayerGuy"))
		{
			ForgePlayer_Zombie player = other.GetComponent<ForgePlayer_Zombie>();
			//A player hit this powerup!
			player.RPC("EnableRapidFire", NetworkReceivers.All);
			Debug.Log("Powerup Triggered for " + other.gameObject.name);

			Networking.Destroy(this);
		}
    }
}
