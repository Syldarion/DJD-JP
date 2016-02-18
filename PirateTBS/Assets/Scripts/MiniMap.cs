using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MiniMap : MonoBehaviour
{
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
