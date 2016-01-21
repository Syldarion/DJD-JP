using UnityEngine;
using System.Collections;
using BeardedManStudios.Network;

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
        PlayerScript player = GameObject.Find(Networking.PrimarySocket.Me.Name + "Controller").GetComponent<PlayerScript>();
        FleetScript tile_fleet = GetComponentInChildren<FleetScript>();

        Debug.Log(player.OwnerId);

        if(player.ActiveFleet != null)
        {
            if (tile_fleet == null)
                player.ActiveFleet.MoveFleet(this);
            else
            {
                if(tile_fleet.OwnerId == player.ActiveFleet.OwnerId)
                {
                    //Fleet merge UI
                }
                else
                {
                    //Combat UI
                }
            }
        }
        else if(tile_fleet != null)
        {
            if (player.OwnerId == tile_fleet.OwnerId)
                player.ActiveFleet = tile_fleet;
        }
    }
}