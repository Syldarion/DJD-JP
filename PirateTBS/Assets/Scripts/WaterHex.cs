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
        MeshRenderer = GetComponent<MeshRenderer>();
        IsWater = true;

        MeshRenderer.sharedMaterial = CloudMaterial;
    }

    void OnMouseDown()
    {
        if (!PlayerScript.MyPlayer.OpenUI)
            GameObject.Find("MovementManager").GetComponent<MovementManager>().MoveFleet(this);
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
        if (!PlayerScript.MyPlayer.OpenUI)
            Camera.main.GetComponent<PanCamera>().CenterOnTarget(transform);
    }
}