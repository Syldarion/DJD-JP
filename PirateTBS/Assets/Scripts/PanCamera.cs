using UnityEngine;
using System.Collections;

[AddComponentMenu("Camera Controls/Pan Camera")]
public class PanCamera : MonoBehaviour
{
    float drag_speed;                   //How many units the camera will move per frame while dragging
    float zoom_speed;                   //How many units the camera will zoom per frame

    Vector3 translation;                //How much to move this frame

    Ray forward_ray;                    //Ray to maintain zoom level
    RaycastHit hit;                     //Raycast to maintain zoom level

    Transform current_target;           //Current target of the camera
    Vector3 current_offset;             //Current offset from map

    void Start()
    {
        drag_speed = 20f;
        zoom_speed = 10f;
        translation = Vector3.zero;
	}
	
	void Update()
    {
        if (PlayerScript.MyPlayer.OpenUI && name == "MainCamera")
            return;

        if (Input.GetKeyDown(KeyCode.Space))
            CenterOnTarget(current_target);

        //100-500
        forward_ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(forward_ray, out hit))
        {
            if (hit.distance <= 50.0f && hit.distance >= 10.0f)
                translation += transform.forward * Input.GetAxis("Mouse ScrollWheel") * zoom_speed;
            else if (hit.distance > 50.0f)
                translation += transform.forward;
            else if (hit.distance < 10.0f)
                translation += -transform.forward;

            current_offset = transform.position - hit.transform.position;
        }

        if (Input.GetMouseButton(1))
            translation -= new Vector3(Input.GetAxis("Mouse X") * drag_speed * Time.deltaTime, 0, Input.GetAxis("Mouse Y") * drag_speed * Time.deltaTime);
        else
            translation += new Vector3(Input.GetAxis("Horizontal") * drag_speed * Time.deltaTime, 0, Input.GetAxis("Vertical") * drag_speed * Time.deltaTime);

        transform.position += translation;
        translation = Vector3.zero;
    }

    /// <summary>
    /// Sets camera target and starts to move towards that target
    /// </summary>
    /// <param name="target">Target to center camera on</param>
    public void CenterOnTarget(Transform target)
    {
        if (target)
        {
            current_target = target;
            StartCoroutine(MoveToPosition(target.position));
        }
    }

    /// <summary>
    /// Smoothly move camera to new position
    /// </summary>
    /// <param name="new_pos">Position to move camera to</param>
    /// <returns></returns>
    IEnumerator MoveToPosition(Vector3 new_pos)
    {
        Vector3 target = new_pos + current_offset;
        while(Vector3.Distance(transform.position, target) > 1.0f)
        {
            transform.position = Vector3.Lerp(transform.position, target, 0.25f);
            yield return null;
        }
        transform.position = target;
    }
}
