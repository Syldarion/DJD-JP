using UnityEngine;
using System.Collections;

public class ControlPoint : MonoBehaviour
{
    public GameObject WaterHex;
    public GameObject LandHex;

    public bool IsWaterControl;

	void Start()
	{

	}
	
	void Update()
	{

	}

    void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<HexTile>())
        {
            Debug.Log("Changing Tile");

            Transform new_hex = null;

            if (IsWaterControl && other.GetComponent<LandHex>())
            {
                new_hex = Instantiate(WaterHex, other.transform.position, other.transform.rotation) as Transform;
                new_hex.GetComponent<WaterHex>()._TileType = HexTile.TileType.Water;
            }
            else if (!IsWaterControl && other.GetComponent<WaterHex>())
            {
                new_hex = Instantiate(LandHex, other.transform.position, other.transform.rotation) as Transform;
                new_hex.GetComponent<LandHex>()._TileType = HexTile.TileType.Land;
            }
            else
                return;

            new_hex.SetParent(other.transform.parent);
            new_hex.localPosition = other.transform.localPosition;

            new_hex.GetComponent<HexTile>().CopyTile(other.GetComponent<HexTile>());

            new_hex.name = other.name;

            

            Destroy(other.gameObject);
        }
    }
}
