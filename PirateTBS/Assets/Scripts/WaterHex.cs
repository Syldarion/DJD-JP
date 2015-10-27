using UnityEngine;
using System.Collections;

public class WaterHex : HexTile
{
	void Start()
    {
        MeshRenderer = GetComponent<SkinnedMeshRenderer>();
        baseColor = Color.cyan;
        _TileType = TileType.Water;

        MeshRenderer.material.color = baseColor;
    }

	void Update()
    {

    }

    void OnMouseOver()
    {
        float lerp = Mathf.PingPong(Time.time, hoverTimer) / hoverTimer;
        MeshRenderer.material.color = Color.Lerp(baseColor, Color.yellow, lerp);

        if(Input.GetKeyDown(KeyCode.M))
        {
            foreach (HexTile ht in HexGrid.MovementHex(this, 4))
            {
                ht.GetComponent<SkinnedMeshRenderer>().material.color = Color.red;
                ht.StartCoroutine("SwitchToBaseColor", 2.0f);
            }
        }
    }
}