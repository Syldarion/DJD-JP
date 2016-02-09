using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class HexTile : MonoBehaviour
{
    public struct HexCoordinate
    {
        public int Q;
        public int R;

        public HexCoordinate(int q, int r)
        {
            Q = q;
            R = r;
        }
    }

    public HexCoordinate HexCoord;

    protected float hoverTimer = 0.5f;

    public HexCoordinate[] Directions = new HexCoordinate[6];

    [HideInInspector]
    public SkinnedMeshRenderer MeshRenderer;
    protected Color baseColor;

    public enum TileType
    {
        Water,
        Land,
        Port,
        Fort
    }
    public TileType _TileType;

    public void CopyTile(HexTile other)
    {
        HexCoord = other.HexCoord;
        Directions = other.Directions;
    }

    float DistanceToTile(HexTile dest)
    {
        return (Mathf.Abs(HexCoord.Q - dest.HexCoord.Q)
            + Mathf.Abs(HexCoord.Q + HexCoord.R - dest.HexCoord.Q - dest.HexCoord.R)
            + Mathf.Abs(HexCoord.R - dest.HexCoord.R)) / 2.0f;
    }

    public void SetDirections(bool is_even)
    {
        Directions[0] = new HexCoordinate(0, 1);
        Directions[1] = new HexCoordinate(1, 1);
        Directions[2] = new HexCoordinate(1, 0);
        Directions[3] = new HexCoordinate(0, -1);
        Directions[4] = new HexCoordinate(-1, 0);
        Directions[5] = new HexCoordinate(-1, 1);

        if(is_even)
        {
            Directions[1].R--;
            Directions[2].R--;
            Directions[4].R--;
            Directions[5].R--;
        }
    }

    public HexTile GetNeighbor(HexCoordinate direction)
    {
        string hex_name = string.Format("{0},{1}", HexCoord.Q + direction.Q, HexCoord.R + direction.R);
        if (GameObject.Find(hex_name))
            return GameObject.Find(hex_name).GetComponent<HexTile>();
        else
            return null;
    }

    public IEnumerator SwitchToBaseColor(float wait_time)
    {
        yield return new WaitForSeconds(wait_time);
        MeshRenderer.material.color = baseColor;
    }

    void OnMouseEnter()
    {
    }

    void OnMouseExit()
    {
        MeshRenderer.material.color = baseColor;
    }
}
