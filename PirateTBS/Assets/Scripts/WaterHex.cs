using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class WaterHex : HexTile
{
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

        //Movement related
        if (!PlayerScript.MyPlayer.ActiveShip)
            return;
        if (Input.GetMouseButton(0) && 
            PlayerScript.MyPlayer.ActiveShip.MovementQueue.Count < PlayerScript.MyPlayer.ActiveShip.Speed && 
            !PlayerScript.MyPlayer.ActiveShip.MoveActionTaken)
        {
            PlayerScript.MyPlayer.ActiveShip.CmdQueueMove(this.HexCoord.Q, this.HexCoord.R);
            GetComponent<MeshRenderer>().sharedMaterial = HighlightMaterial;
        }
    }

    void OnMouseDown()
    {
        if (PlayerScript.MyPlayer.ActiveShip && 
            PlayerScript.MyPlayer.ActiveShip.MovementQueue.Count < PlayerScript.MyPlayer.ActiveShip.Speed && 
            !PlayerScript.MyPlayer.ActiveShip.MoveActionTaken)
        {
            PlayerScript.MyPlayer.ActiveShip.CmdQueueMove(this.HexCoord.Q, this.HexCoord.R);
            GetComponent<MeshRenderer>().sharedMaterial = HighlightMaterial;
        }
    }

    void OnMouseUp()
    {
        if (PlayerScript.MyPlayer.OpenUI)
            return;

        if (PlayerScript.MyPlayer.ActiveShip)
        {
            if (PlayerScript.MyPlayer.ActiveShip.MovementQueue.Count > 0)
                PlayerScript.MyPlayer.ActiveShip.CmdMoveShip();
        }

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