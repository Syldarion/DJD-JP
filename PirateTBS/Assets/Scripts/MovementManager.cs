using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MovementManager : MonoBehaviour
{
    [HideInInspector]
    public static MovementManager Instance;

    public PlayerScript ReferencePlayer;            //Reference to player this script manages movement for

    public List<WaterHex> MovementQueue;            //List of water tiles to move along

	void Start()
    {
        Instance = this;
        StartCoroutine(WaitForPlayer());

        MovementQueue = new List<WaterHex>();
	}
	
	void Update()
    {

	}

    /// <summary>
    /// Remove all tiles from movement queue
    /// </summary>
    public void ClearQueue()
    {
        foreach (WaterHex hex in MovementQueue)
        {
            if (hex.Discovered)
                hex.GetComponent<MeshRenderer>().sharedMaterial = hex.CloudMaterial;
            else if (hex.Fog)
                hex.GetComponent<MeshRenderer>().sharedMaterial = hex.FogMaterial;
            else
                hex.GetComponent<MeshRenderer>().sharedMaterial = hex.DefaultMaterial;
        }
        MovementQueue.Clear();
    }

    /// <summary>
    /// Select a new active ship for player
    /// </summary>
    /// <param name="selected_ship">Ship to select</param>
    public void SelectShip(Ship selected_ship)
    {
        if (ReferencePlayer.Ships.Contains(selected_ship))
            ReferencePlayer.ActiveShip = selected_ship;
    }

    /// <summary>
    /// Move active ship if it can move along the given path
    /// </summary>
    public void MoveShip()
    {
        if(!ReferencePlayer.ActiveShip)
        {
            ClearQueue();
            return;
        }

        if (!HexGrid.MovementHex(ReferencePlayer.ActiveShip.CurrentPosition, 1).Contains(MovementQueue[0]))
        {
            ClearQueue();
            return;
        }

        int remaining_moves = ReferencePlayer.ActiveShip.Speed;
        WaterHex next_tile;
        Ship tile_ship;

        for(int i = 0; i < MovementQueue.Count && i < remaining_moves; i++)
        {
            next_tile = MovementQueue[i];
            tile_ship = next_tile.GetComponentInChildren<Ship>();

            if (!tile_ship)
                ReferencePlayer.ActiveShip.CmdQueueMove(next_tile.HexCoord.Q, next_tile.HexCoord.R);
            else
            {
                if (ReferencePlayer.Ships.Contains(tile_ship))
                {
                    //i dunno
                }
                else
                {
                    //do combat
                }
            }
        }

        ReferencePlayer.ActiveShip.CmdMoveShip();
        ClearQueue();
    }

    /// <summary>
    /// Wait for local player to exist
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitForPlayer()
    {
        while (!PlayerScript.MyPlayer)
            yield return null;
        ReferencePlayer = PlayerScript.MyPlayer;
    }
}