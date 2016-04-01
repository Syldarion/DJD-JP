using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatMovementManager : MonoBehaviour
{
    [HideInInspector]
    public static CombatMovementManager Instance;

    public PlayerScript ReferencePlayer;
    public Ship SelectedShip;

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

    public void SelectShip(Ship selected_ship)
    {
        if (CombatManager.Instance.PlayerFleet.Ships.Contains(selected_ship))
            SelectedShip = selected_ship;
    }

    public void MoveShip()
    {
        if(!SelectedShip)
        {
            ClearQueue();
            return;
        }

        if(!CombatHexGrid.MovementHex(SelectedShip.CurrentPosition, 1).Contains(MovementQueue[0]))
        {
            ClearQueue();
            return;
        }

        SelectedShip.MovementQueue = MovementQueue;

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
