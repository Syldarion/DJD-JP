using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MovementManager : MonoBehaviour
{
    [HideInInspector]
    public static MovementManager Instance;

    public PlayerScript ReferencePlayer;
    public FleetManager FleetManager;

    public List<WaterHex> MovementQueue;

	void Start()
    {
        Instance = this;
        StartCoroutine(WaitForPlayer());

        MovementQueue = new List<WaterHex>();
	}
	
	void Update()
    {

	}

    public void ClearQueue()
    {
        foreach(WaterHex hex in MovementQueue)
            hex.GetComponent<MeshRenderer>().sharedMaterial = hex.DefaultMaterial;
        MovementQueue.Clear();
    }

    public void SelectFleet(Fleet selected_fleet)
    {
        if (ReferencePlayer.Fleets.Contains(selected_fleet))
            ReferencePlayer.ActiveFleet = selected_fleet;
    }

    public IEnumerator MoveFleet()
    {
        if(!ReferencePlayer.ActiveFleet)
        {
            ClearQueue();
            yield break;
        }

        if (!HexGrid.MovementHex(ReferencePlayer.ActiveFleet.CurrentPosition, 1).Contains(MovementQueue[0]))
            yield break;

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

            yield return null;
        }

        ReferencePlayer.ActiveFleet.CmdMoveFleet();
        ClearQueue();
    }

    IEnumerator WaitForPlayer()
    {
        while (!PlayerScript.MyPlayer)
            yield return null;
        ReferencePlayer = PlayerScript.MyPlayer;
    }
}