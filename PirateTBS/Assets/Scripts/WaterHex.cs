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
            foreach (HexTile ht in HexGrid.MovementHex(this, 5))
            {
                ht.GetComponent<SkinnedMeshRenderer>().material.color = Color.red;
                ht.StartCoroutine("SwitchToBaseColor", 2.0f);
            }
        }
    }

    void OnMouseDown()
    {
        PlayerScript player = GameObject.Find("PlayerController").GetComponent<PlayerScript>();
        ShipScript tile_ship = GetComponentInChildren<ShipScript>();

        if(player.SelectedShip != null)
        {
            if (tile_ship == null)
                player.SelectedShip.MoveShip(this);
            else
            {
                //Combat code
            }
        }
        else if(tile_ship != null)
        {
            if (player.OwnerId == tile_ship.OwnerId)
                player.SelectedShip = tile_ship;
        }
    }
}