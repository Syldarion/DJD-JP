using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MovementManager : MonoBehaviour
{
    [HideInInspector]
    public static MovementManager Instance;

	void Start()
    {
        Instance = this;
        StartCoroutine(WaitForPlayer());
	}
	
	void Update()
    {

	}

    /// <summary>
    /// Remove all tiles from movement queue
    /// </summary>
    public void ClearQueue()
    {
        foreach (WaterHex hex in PlayerScript.MyPlayer.ActiveShip.MovementQueue)
        {
            if (hex.Discovered)
                hex.GetComponent<MeshRenderer>().sharedMaterial = hex.CloudMaterial;
            else if (hex.Fog)
                hex.GetComponent<MeshRenderer>().sharedMaterial = hex.FogMaterial;
            else
                hex.GetComponent<MeshRenderer>().sharedMaterial = hex.DefaultMaterial;
        }
        PlayerScript.MyPlayer.ActiveShip.MovementQueue.Clear();
    }

    /// <summary>
    /// Select a new active ship for player
    /// </summary>
    /// <param name="selected_ship">Ship to select</param>
    public void SelectShip(Ship selected_ship)
    {
        if (PlayerScript.MyPlayer.Ships.Contains(selected_ship))
            PlayerScript.MyPlayer.ActiveShip = selected_ship;
    }

    /// <summary>
    /// Move active ship if it can move along the given path
    /// </summary>
    public void MoveShip()
    {
        if(!PlayerScript.MyPlayer.ActiveShip)
        {
            ClearQueue();
            return;
        }

        if (!HexGrid.MovementHex(PlayerScript.MyPlayer.ActiveShip.CurrentPosition, 1).Contains(PlayerScript.MyPlayer.ActiveShip.MovementQueue[0]))
        {
            ClearQueue();
            return;
        }

        int remaining_moves = PlayerScript.MyPlayer.ActiveShip.Speed;
        WaterHex next_tile;
        Ship tile_ship;

        for(int i = 0; i < PlayerScript.MyPlayer.ActiveShip.MovementQueue.Count && i < remaining_moves; i++)
        {
            next_tile = PlayerScript.MyPlayer.ActiveShip.MovementQueue[i];
            tile_ship = next_tile.GetComponentInChildren<Ship>();

            if (!tile_ship)
                PlayerScript.MyPlayer.ActiveShip.CmdQueueMove(next_tile.HexCoord.Q, next_tile.HexCoord.R);
            else
            {
                if (PlayerScript.MyPlayer.Ships.Contains(tile_ship))
                {
                    //open cargo manager
                }
                else
                {
                    CombatManager.Instance.OpenCombatPanel();
                    CombatManager.Instance.StartCombat(PlayerScript.MyPlayer.ActiveShip, tile_ship);
                }
            }
        }

        PlayerScript.MyPlayer.ActiveShip.CmdMoveShip();
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
    }
}