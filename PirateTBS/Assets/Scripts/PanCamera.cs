using UnityEngine;
using System.Collections;

[AddComponentMenu("Camera Controls/Pan Camera")]
public class PanCamera : MonoBehaviour
{
    float cameraSpeed;

	void Start()
    {
        cameraSpeed = 2.0f;
	}
	
	void Update()
    {
        /*
        if (Input.GetMouseButton(2))
        {
            float left_right = Input.GetAxis("Mouse X");
            float up_down = Input.GetAxis("Mouse Y");

            transform.Translate(left_right, up_down, 0.0f, Space.Self);
        }
        else if (Input.GetMouseButton(1))
        {
            float left_right = Input.GetAxis("Mouse X");
            float up_down = -Input.GetAxis("Mouse Y");

            transform.Rotate(up_down, 0.0f, 0.0f, Space.Self);
            transform.Rotate(0.0f, left_right, 0.0f, Space.World);
        }
        */

        transform.Translate((Input.GetAxis("Horizontal") * transform.right + 
            Input.GetAxis("Vertical") * -transform.forward) * Time.deltaTime * cameraSpeed);

        if (Input.GetKey(KeyCode.LeftShift))
            transform.Translate(transform.up * Time.deltaTime * cameraSpeed);
        else if (Input.GetKey(KeyCode.LeftControl))
            transform.Translate(-transform.up * Time.deltaTime * cameraSpeed);
	}
}
