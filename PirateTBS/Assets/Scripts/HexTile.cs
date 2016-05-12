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
    public MeshRenderer MeshRenderer;   //Reference to tile's mesh
    [HideInInspector]
    public HexCoordinate HexCoord;      //Q, R coordinates for the tile
    [HideInInspector]
    public HexCoordinate[] Directions 
        = new HexCoordinate[6];         //Constant coordinates pointing to tile neighbors

    public bool IsWater = false;        //Is the tile water?
    public bool Fog = true;             //Is the tile fogged?
    public bool Discovered = false;     //Is the tile discovered?

    public Material DefaultMaterial;    //Material for discovered / unfogged
    public Material FogMaterial;        //Material for discovered / fogged
    public Material CloudMaterial;      //Material for undiscovered
    public Material HighlightMaterial;  //Material for highlighted

    public virtual void InitializeTile() { }

    /// <summary>
    /// Discover tile, removing cloud cover
    /// </summary>
    public void DiscoverTile()
    {
        Discovered = true;
        MeshRenderer.material = DefaultMaterial;
        MiniMap.Instance.transform.FindChild(name).GetComponent<MeshRenderer>().material = DefaultMaterial;
    }

    /// <summary>
    /// Reveal tile, removing fog cover
    /// </summary>
    public void RevealTile()
    {
        Fog = false;
        MeshRenderer.material = DefaultMaterial;
        MiniMap.Instance.transform.FindChild(name).GetComponent<MeshRenderer>().material = DefaultMaterial;
    }

    /// <summary>
    /// Hide tile, adding fog cover
    /// </summary>
    public void HideTile()
    {
        Fog = true;
        MeshRenderer.material = FogMaterial;
        MiniMap.Instance.transform.FindChild(name).GetComponent<MeshRenderer>().material = FogMaterial;
    }

    /// <summary>
    /// Copy coordinates and neighbor coordinates of another tile
    /// </summary>
    /// <param name="other">Tile to copy</param>
    public void CopyTile(HexTile other)
    {
        HexCoord = new HexCoordinate(other.HexCoord.Q, other.HexCoord.R);
        for(int i = 0; i < other.Directions.Length; i++)
            Directions[i] = new HexCoordinate(other.Directions[i].Q, other.Directions[i].R);
    }

    /// <summary>
    /// Get distance from this tile to another
    /// </summary>
    /// <param name="dest">Other tile</param>
    /// <returns>Distance from this to dest</returns>
    float DistanceToTile(HexTile dest)
    {
        return (Mathf.Abs(HexCoord.Q - dest.HexCoord.Q)
            + Mathf.Abs(HexCoord.Q + HexCoord.R - dest.HexCoord.Q - dest.HexCoord.R)
            + Mathf.Abs(HexCoord.R - dest.HexCoord.R)) / 2.0f;
    }

    /// <summary>
    /// Sets direction coordinates
    /// </summary>
    /// <param name="is_even">Is the tile an "even" tile?</param>
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

    /// <summary>
    /// Get a specific neighbor
    /// </summary>
    /// <param name="direction">The direction the neighbor is in</param>
    /// <returns>Hextile in the specified direction, if it exists</returns>
    public HexTile GetNeighbor(HexCoordinate direction)
    {
        string hex_name = string.Format("{0}/{1},{2}", transform.parent.name, HexCoord.Q + direction.Q, HexCoord.R + direction.R);
        if (GameObject.Find(hex_name))
            return GameObject.Find(hex_name).GetComponent<HexTile>();
        else
            return null;
    }
}