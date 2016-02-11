using UnityEngine;
using System.Collections;
using BeardedManStudios.Network;

public class WaterHex : HexTile
{
    float hover_timer = 0.5f;
    float double_click_start = 0;

	void Start()
    {
        InitializeTile();
    }

	void Update()
    {

    }

    public override void InitializeTile()
    {
        MeshRenderer = GetComponent<SkinnedMeshRenderer>();
        base_color = Color.cyan;
        MeshRenderer.material.color = base_color;

        IsWater = true;
    }

    void OnMouseOver()
    {
        float lerp = Mathf.PingPong(Time.time, hover_timer) / hover_timer;
        MeshRenderer.material.color = Color.Lerp(base_color, Color.yellow, lerp);
    }

    void OnMouseExit()
    {
        MeshRenderer.material.color = base_color;
    }

    void OnMouseDown()
    {
        PlayerScript player = GameObject.Find(Networking.PrimarySocket.Me.Name + "Controller").GetComponent<PlayerScript>();
        Fleet tile_fleet = GetComponentInChildren<Fleet>();

        if (Input.GetMouseButtonDown(0))
        {
            if (player.ActiveFleet != null)
            {
                if (tile_fleet == null)
                {
                    if (!player.ActiveFleet.MoveFleet(this))
                        player.ActiveFleet = null;
                }
                else
                {
                    if (tile_fleet != player.ActiveFleet && tile_fleet.OwnerId == player.ActiveFleet.OwnerId)
                    {
                        if (HexGrid.MovementHex(player.ActiveFleet.GetComponentInParent<HexTile>(), player.ActiveFleet.FleetSpeed).Contains(this))
                        {
                            FleetManager manager = GameObject.Find("FleetManagementPanel").GetComponent<FleetManager>();
                            manager.PopulateFleetManager(player.ActiveFleet, tile_fleet);

                            player.ActiveFleet = null;
                        }
                    }
                    else
                    {
                        //Combat UI
                    }
                }
            }
            else if (tile_fleet != null)
            {
                if (player.OwnerId == tile_fleet.OwnerId)
                    player.ActiveFleet = tile_fleet;
            }
        }
    }

    void OnMouseUp()
    {
        if (Time.time - double_click_start < 0.3f)
        {
            this.OnDoubleClick();
            double_click_start = -1;
        }
        else
            double_click_start = Time.time;
    }

    void OnDoubleClick()
    {
        Camera.main.GetComponent<PanCamera>().StartCoroutine("MoveToPosition", this.transform.position);
    }
}