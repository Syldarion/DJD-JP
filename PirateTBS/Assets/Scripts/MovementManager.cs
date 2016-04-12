using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MovementManager : MonoBehaviour
{
    [HideInInspector]
    public static MovementManager Instance;

    public PlayerScript ReferencePlayer;            //Reference to player this script manages movement for
    public FleetManager FleetManager;               //Reference to existing fleet manager

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
    /// Select a new active fleet for player
    /// </summary>
    /// <param name="selected_fleet">Fleet to select</param>
    public void SelectFleet(Fleet selected_fleet)
    {
        if (ReferencePlayer.Fleets.Contains(selected_fleet))
            ReferencePlayer.ActiveFleet = selected_fleet;
    }

    /// <summary>
    /// Move active fleet if it can move along the given path
    /// </summary>
    public void MoveFleet()
    {
        if(!ReferencePlayer.ActiveFleet)
        {
            ClearQueue();
            return;
        }

        if (!HexGrid.MovementHex(ReferencePlayer.ActiveFleet.CurrentPosition, 1).Contains(MovementQueue[0]))
        {
            ClearQueue();
            return;
        }

        int remaining_moves = ReferencePlayer.ActiveFleet.FleetSpeed;
        WaterHex next_tile;
        Fleet tile_fleet;

        for(int i = 0; i < MovementQueue.Count && i < remaining_moves; i++)
        {
            next_tile = MovementQueue[i];
            tile_fleet = next_tile.GetComponentInChildren<Fleet>();

            if (!tile_fleet)
                ReferencePlayer.ActiveFleet.CmdQueueMove(next_tile.HexCoord.Q, next_tile.HexCoord.R);
            else
            {
                if (ReferencePlayer.Fleets.Contains(tile_fleet))
                {
                    FleetManager.PopulateFleetManager(ReferencePlayer.ActiveFleet, tile_fleet);
                    remaining_moves = 0;
                }
                else
                {
                    CombatManager.Instance.PopulateFleetLists(ReferencePlayer.ActiveFleet, tile_fleet);
                    CombatManager.Instance.OpenCombatPanel();
                    remaining_moves = 0;
                }
            }
        }

        ReferencePlayer.ActiveFleet.CmdMoveFleet();
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