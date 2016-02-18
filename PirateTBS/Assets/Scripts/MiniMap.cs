using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MiniMap : MonoBehaviour
{
    public Camera MinimapCamera;
    public WaterHex WaterHexPrefab;
    public LandHex LandHexPrefab;



    void Start()
    {
    }

    void Update()
    {

    }

    public void CopyHexGridToMap()
    {
        HexGrid grid = GameObject.Find("Grid").GetComponent<HexGrid>();

        foreach(LandHex lh in grid.LandTiles)
        {
            Vector3 current_local_pos = lh.transform.localPosition;

            float camera_height = (grid.GridWidth * grid.HexWidth) / Mathf.Tan(Mathf.Deg2Rad * 30.0f);

            LandHex new_hex = Instantiate(lh);
            new_hex.transform.SetParent(MinimapCamera.transform);
            new_hex.transform.localPosition = current_local_pos + new Vector3(0.0f, camera_height, 0.0f);
        }

        foreach (WaterHex wh in grid.WaterTiles)
        {
            Vector3 current_local_pos = wh.transform.localPosition;

            float camera_height = (grid.GridWidth * grid.HexWidth) / Mathf.Tan(Mathf.Deg2Rad * 30.0f);

            WaterHex new_hex = Instantiate(wh);
            new_hex.transform.SetParent(MinimapCamera.transform);
            new_hex.transform.localPosition = current_local_pos + new Vector3(0.0f, camera_height, 0.0f);
        }
    }
}
