using UnityEngine;
using System.Collections;

public class MovementManager : MonoBehaviour
{
    [HideInInspector]
    public static MovementManager Instance;

    public PlayerScript ReferencePlayer;
    public FleetManager FleetManager;

	void Start()
    {
        Instance = this;
        StartCoroutine(WaitForPlayer());
	}
	
	void Update()
    {

	}

    public void SelectFleet(Fleet selected_fleet)
    {
        if (ReferencePlayer.Fleets.Contains(selected_fleet))
            ReferencePlayer.ActiveFleet = selected_fleet;
    }

    public void MoveFleet(HexTile new_tile)
    {
        if (!ReferencePlayer.ActiveFleet)
            return;

        Fleet tile_fleet = new_tile.GetComponentInChildren<Fleet>();

        if (!tile_fleet && HexGrid.MovementHex(ReferencePlayer.ActiveFleet.CurrentPosition, ReferencePlayer.ActiveFleet.FleetSpeed).Contains(new_tile))
            ReferencePlayer.ActiveFleet.CmdMoveFleet(new_tile.HexCoord.Q, new_tile.HexCoord.R);
        else if(HexGrid.MovementHex(ReferencePlayer.ActiveFleet.CurrentPosition, 1).Contains(new_tile))
        {
            if (ReferencePlayer.Fleets.Contains(tile_fleet))
                FleetManager.PopulateFleetManager(ReferencePlayer.ActiveFleet, tile_fleet);
            else
            { } //Combat
        }
    }

    IEnumerator WaitForPlayer()
    {
        while (!PlayerScript.MyPlayer)
            yield return null;
        ReferencePlayer = PlayerScript.MyPlayer;
    }
}