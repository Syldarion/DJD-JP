using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatMovementManager : MonoBehaviour
{
    [HideInInspector]
    public static CombatMovementManager Instance;

    public PlayerScript ReferencePlayer;
    public CombatShip SelectedShip;

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
        foreach (WaterHex hex in MovementQueue)
            hex.MeshRenderer.sharedMaterial = hex.DefaultMaterial;
        MovementQueue.Clear();
    }

    public void SelectShip(CombatShip selected_ship)
    {
        if (PlayerScript.MyPlayer.ActiveFleet.Ships.Contains(selected_ship.LinkedShip))
            SelectedShip = selected_ship;
    }

    public void MoveShip()
    {
        if (!SelectedShip)
        {
            ClearQueue();
            return;
        }

        if (!CombatHexGrid.MovementHex(SelectedShip.CurrentPosition, 1).Contains(MovementQueue[0]))
        {
            ClearQueue();
            return;
        }
        
        WaterHex next_tile;
        CombatShip tile_ship;

        for (int i = 0; i < MovementQueue.Count; i++)
        {
            next_tile = MovementQueue[i];
            tile_ship = next_tile.GetComponentInChildren<CombatShip>();

            if (!tile_ship)
                SelectedShip.CmdQueueMove(next_tile.HexCoord.Q, next_tile.HexCoord.R);
        }

        SelectedShip.CmdMoveShip();
        ClearQueue();
    }

    IEnumerator WaitForPlayer()
    {
        while (!PlayerScript.MyPlayer)
            yield return null;
        ReferencePlayer = PlayerScript.MyPlayer;
    }
}
