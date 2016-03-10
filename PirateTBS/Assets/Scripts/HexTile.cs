using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

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

public class HexTile : MonoBehaviour
{
    [HideInInspector]
    public MeshRenderer MeshRenderer;
    [HideInInspector]
    public HexCoordinate HexCoord;
    [HideInInspector]
    public HexCoordinate[] Directions = new HexCoordinate[6];

    public bool IsWater = false;
    public bool Fog = true;
    public bool Discovered = false;

    public Material DefaultMaterial;
    public Material FogMaterial;
    public Material CloudMaterial;

    public virtual void InitializeTile() { }

    public void DiscoverTile()
    {
        Discovered = true;
        MeshRenderer.material = DefaultMaterial;
        MiniMap.Instance.transform.FindChild(name).GetComponent<MeshRenderer>().material = DefaultMaterial;
    }

    public void RevealTile()
    {
        Fog = false;
        MeshRenderer.material = DefaultMaterial;
        MiniMap.Instance.transform.FindChild(name).GetComponent<MeshRenderer>().material = DefaultMaterial;
    }

    public void HideTile()
    {
        Fog = true;
        MeshRenderer.material = FogMaterial;
        MiniMap.Instance.transform.FindChild(name).GetComponent<MeshRenderer>().material = FogMaterial;
    }

    public void CopyTile(HexTile other)
    {
        HexCoord = new HexCoordinate(other.HexCoord.Q, other.HexCoord.R);
        for(int i = 0; i < other.Directions.Length; i++)
            Directions[i] = new HexCoordinate(other.Directions[i].Q, other.Directions[i].R);
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
}