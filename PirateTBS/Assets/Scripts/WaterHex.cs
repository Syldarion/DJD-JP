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

    void OnMouseEnter()
    {
        if (PlayerScript.MyPlayer.OpenUI)
            return;

        if (Input.GetMouseButton(0))
        {
            MovementManager.Instance.MovementQueue.Add(this);
            GetComponent<MeshRenderer>().sharedMaterial = HighlightMaterial;
        }
        else
        {
            MovementManager.Instance.ClearQueue();
        }
    }

    void OnMouseDown()
    {
        MovementManager.Instance.MovementQueue.Add(this);
        GetComponent<MeshRenderer>().sharedMaterial = HighlightMaterial;
    }

    void OnMouseUp()
    {
        if (PlayerScript.MyPlayer.OpenUI)
            return;

        StartCoroutine(MovementManager.Instance.MoveFleet());

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
        Camera.main.GetComponent<PanCamera>().CenterOnTarget(transform);
    }
}