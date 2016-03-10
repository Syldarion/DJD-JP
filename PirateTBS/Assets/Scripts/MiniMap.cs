using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MiniMap : MonoBehaviour
{
    [HideInInspector]
    public static MiniMap Instance;

    public WaterHex WaterHexPrefab;
    public LandHex LandHexPrefab;

    void Start()
    {
        Instance = this;

        //16:9 - 0.2:0.25
        //5:4
        //4:3
        //3:2
        //16:10

        //Rect modify_rect = GetComponent<Camera>().rect;

        //modify_rect.width = Camera.main.pixelWidth / (1080.0f / 0.2f);
        //modify_rect.height = Camera.main.pixelHeight / (1920.0f / 0.25f);

        //GetComponent<Camera>().rect = modify_rect;
    }

    void Update()
    {

    }

    public void CopyHexGridToMap()
    {
        LoadingScreenManager.Instance.SetMessage("Creating Minimap...");
        LoadingScreenManager.Instance.SetProgress(75.0f);

        HexGrid grid = HexGrid.Instance;

        float camera_height = (grid.GridHeight * grid.HexWidth) * 0.5f / Mathf.Tan(Mathf.Deg2Rad * (GetComponent<Camera>().fieldOfView / 2.0f));
        GetComponent<Camera>().farClipPlane = camera_height + 100.0f;
        float swap;

        foreach (LandHex lh in grid.LandTiles)
        {
            Vector3 current_local_pos = lh.transform.localPosition;
            swap = current_local_pos.y;
            current_local_pos = new Vector3(current_local_pos.x, current_local_pos.z, swap);

            LandHex new_hex = Instantiate(lh);
            new_hex.name = lh.name;
            new_hex.transform.SetParent(transform);
            new_hex.transform.localPosition = new Vector3(current_local_pos.x, current_local_pos.y, camera_height);
        }

        foreach (WaterHex wh in grid.WaterTiles)
        {
            Vector3 current_local_pos = wh.transform.localPosition;
            swap = current_local_pos.y;
            current_local_pos = new Vector3(current_local_pos.x, current_local_pos.z, swap);

            WaterHex new_hex = Instantiate(wh);
            new_hex.name = wh.name;
            new_hex.transform.SetParent(transform);
            new_hex.transform.localPosition = new Vector3(current_local_pos.x, current_local_pos.y, camera_height);
        }
    }
}
