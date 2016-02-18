using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

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

    //make movement manager
    //if you have an active fleet and click your own fleet, open fleet manager
    //no active fleet and your fleet, set active fleet
    //active fleet and click on enemy fleet, open combat
    //active fleet and click on tile, move fleet

    void OnMouseDown()
    {
        PlayerScript player = GameObject.Find("Controller").GetComponent<PlayerScript>();
        Fleet tile_fleet = GetComponentInChildren<Fleet>();

        if (Input.GetMouseButtonDown(0))
        {
            if (player.ActiveFleet != null)
            {
                if (tile_fleet == null)
                {

                }
                else
                {
                    if (tile_fleet != player.ActiveFleet)
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