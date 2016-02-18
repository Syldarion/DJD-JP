using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class WaterHex : HexTile
{
    public Material DefaultMaterial;
    public Material FogMaterial;

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
        GetComponent<MeshRenderer>().sharedMaterial = FogMaterial;

        IsWater = true;
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