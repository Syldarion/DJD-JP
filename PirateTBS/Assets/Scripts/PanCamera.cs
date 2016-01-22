using UnityEngine;
using System.Collections;

[AddComponentMenu("Camera Controls/Pan Camera")]
public class PanCamera : MonoBehaviour
{
    float drag_speed;
    float zoom_speed;

    Vector3 translation;
    bool flag;

    void Start()
    {
        drag_speed = 200f;
        zoom_speed = 200f;
        translation = Vector3.zero;
        flag = false;
	}
	
	void Update()
    {
        if (Input.GetMouseButton(0))
        {
            translation -= new Vector3(Input.GetAxis("Mouse X") * drag_speed * Time.deltaTime, 0, Input.GetAxis("Mouse Y") * drag_speed * Time.deltaTime);
        }
        flag = Input.GetMouseButton(0);

        if (flag)
            transform.position += translation;
        translation = Vector3.zero;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        transform.Translate(transform.up * scroll * zoom_speed);
    }
}
