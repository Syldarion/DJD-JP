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

    public GameObject CannonballPrefab;

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

    public override void OnStartAuthority()
    {
        
    }

    [Command]
    public void CmdFireCannons(Vector3 target)
    {
        GameObject new_cannonball = Instantiate(CannonballPrefab);

        if(Vector3.Distance(target, transform.position + new Vector3(1, 0, 0)) > Vector3.Distance(target, transform.position - new Vector3(1, 0, 0)))
        {
            new_cannonball.GetComponent<Rigidbody>().AddForce(transform.right * 100 + transform.up * 100, ForceMode.Impulse);
        }
        else
        {
            new_cannonball.GetComponent<Rigidbody>().AddForce(-transform.right * 100 + transform.up * 100, ForceMode.Impulse);
        }

        NetworkServer.Spawn(new_cannonball);
    }

    Vector3 BallisticVelocity(Vector3 target, float angle)
    {
        var dir = target - transform.position;
        var h = dir.y;
        dir.y = 0;
        var dist = dir.magnitude;
        var a = angle * Mathf.Deg2Rad;
        dir.y = dist * Mathf.Tan(a);
        dist += h / Mathf.Tan(a);
        var vel = Mathf.Sqrt(dist * Physics.gravity.magnitude / Mathf.Sin(2 * a));

        return vel * dir.normalized;
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
            transform.LookAt(destination);

            Vector3 direction = (destination - transform.position) / 20.0f;
            float step = direction.magnitude;

            for (int i = 0; i < 20; i++)
            {
                transform.Translate(Vector3.forward * step);
                yield return new WaitForSeconds(0.05f / Speed);
            }

            transform.position = destination;

            yield return null;
        }

        MovementQueue.Clear();

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

    void OnRightClick()
    {
        Ray click = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if(Physics.Raycast(click, out hit))
        {
            Debug.Log("Hello?");
            if (hit.collider == GetComponent<BoxCollider>())
            {
                Debug.Log(hit.collider.name);
                CombatMovementManager.Instance.SelectedShip.CmdFireCannons(transform.position);
            }
        }
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
            CombatMovementManager.Instance.SelectedShip.CmdFireCannons(transform.position);
    }

    void OnMouseDown()
    {
        if (CombatSceneManager.Instance)
            CombatMovementManager.Instance.SelectShip(this);
    }
}