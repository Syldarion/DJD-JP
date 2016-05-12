﻿using UnityEngine;
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

        if (Input.GetMouseButton(0))
        {
            if (!CombatSceneManager.Instance)
                MovementManager.Instance.MovementQueue.Add(this);
            else
                CombatMovementManager.Instance.MovementQueue.Add(this);
            GetComponent<MeshRenderer>().sharedMaterial = HighlightMaterial;
        }
        else
        {
            if (!CombatSceneManager.Instance)
                MovementManager.Instance.ClearQueue();
            else
                CombatMovementManager.Instance.ClearQueue();
        }
    }

    void OnMouseDown()
    {
        if (!CombatSceneManager.Instance)
            MovementManager.Instance.MovementQueue.Add(this);
        else
            CombatMovementManager.Instance.MovementQueue.Add(this);
        GetComponent<MeshRenderer>().sharedMaterial = HighlightMaterial;
    }

    void OnMouseUp()
    {
        if (PlayerScript.MyPlayer.OpenUI)
            return;

        if (!CombatSceneManager.Instance)
            MovementManager.Instance.MoveFleet();
        else
            CombatMovementManager.Instance.MoveShip();

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