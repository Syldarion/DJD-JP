using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class CombatShip : Ship
{
    public Ship LinkedShip;                     //Main scene ship this ship links to
    public List<WaterHex> MovementQueue;        //List of tiles to move ship along
    public HexTile CurrentPosition;             //Current parent tile of ship

    void Start()
    {

    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        MovementQueue = new List<WaterHex>();
    }

    void Update()
    {

    }

    /// <summary>
    /// Server-side command to add tile to movement queue
    /// </summary>
    /// <param name="x">Q coordinate of tile</param>
    /// <param name="y">R coordinate of tile</param>
    [Command]
    public void CmdQueueMove(int x, int y)
    {
        WaterHex new_tile = GameObject.Find(string.Format("CombatGrid/{0},{1}", x, y)).GetComponent<WaterHex>();
        if (new_tile)
            MovementQueue.Add(new_tile);
    }

    /// <summary>
    /// Server-side command to run ship through movement queue
    /// </summary>
    [Command]
    public void CmdMoveShip()
    {
        transform.SetParent(MovementQueue[MovementQueue.Count - 1].transform, true);
        CurrentPosition = MovementQueue[MovementQueue.Count - 1];

        RpcMoveShip(MovementQueue[MovementQueue.Count - 1].HexCoord.Q, MovementQueue[MovementQueue.Count - 1].HexCoord.R);

        StopAllCoroutines();
        StartCoroutine(SmoothMove());
    }

    /// <summary>
    /// Smoothly runs ship through all tiles in movement queue
    /// </summary>
    /// <returns></returns>
    public IEnumerator SmoothMove()
    {
        foreach (HexTile dest_tile in MovementQueue)
        {
            Vector3 destination = dest_tile.transform.position + new Vector3(0.0f, 0.25f, 0.0f);

            Vector3 direction = (destination - transform.position) / 20.0f;

            for (int i = 0; i < 20; i++)
            {
                transform.Translate(direction);
                yield return new WaitForSeconds(0.05f / Speed);
            }

            transform.position = destination;

            yield return null;
        }

        transform.localPosition = new Vector3(0.0f, 0.25f, 0.0f);
    }

    /// <summary>
    /// Client-side command to move ship to a tile
    /// </summary>
    /// <param name="x">Q coordinate of tile</param>
    /// <param name="y">R coordinate of tile</param>
    [ClientRpc]
    public void RpcMoveShip(int x, int y)
    {
        HexTile new_tile = GameObject.Find(string.Format("CombatGrid/{0},{1}", x, y)).GetComponent<HexTile>();

        transform.SetParent(new_tile.transform, true);
        CurrentPosition = new_tile;
    }

    void OnMouseDown()
    {
        if (CombatSceneManager.Instance)
            CombatMovementManager.Instance.SelectShip(this);
    }
}