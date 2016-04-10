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
    }

    void Update()
    {

    }

    public void CopyHexGridToMap()
    {
        LoadingScreenManager.Instance.SetMessage("Creating minimap...");
        LoadingScreenManager.Instance.SetProgress(40.0f);

        float camera_height = (HexGrid.Instance.GridHeight * HexGrid.Instance.HexWidth) * 0.5f / Mathf.Tan(Mathf.Deg2Rad * (GetComponent<Camera>().fieldOfView / 2.0f));
        GetComponent<Camera>().farClipPlane = camera_height + 100.0f;
        float swap;

        foreach (LandHex lh in HexGrid.Instance.LandTiles)
        {
            Vector3 current_local_pos = lh.transform.localPosition;
            swap = current_local_pos.y;
            current_local_pos = new Vector3(current_local_pos.x, current_local_pos.z, swap);

            LandHex new_hex = Instantiate(lh);
            new_hex.name = lh.name;
            new_hex.transform.SetParent(transform);
            new_hex.transform.localPosition = new Vector3(current_local_pos.x, current_local_pos.y, camera_height);
        }

        foreach (WaterHex wh in HexGrid.Instance.WaterTiles)
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
