using UnityEngine;
using System.Collections;

[AddComponentMenu("Camera Controls/Pan Camera")]
public class PanCamera : MonoBehaviour
{
    float drag_speed;
    float zoom_speed;

    Vector3 translation;

    Ray forward_ray;
    RaycastHit hit;

    Transform current_target;
    Vector3 current_offset;

    void Start()
    {
        drag_speed = 200f;
        zoom_speed = 100f;
        translation = Vector3.zero;
	}
	
	void Update()
    {
        if (PlayerScript.MyPlayer.OpenUI)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
            CenterOnTarget(current_target);

        //100-500
        forward_ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(forward_ray, out hit))
        {
            if (hit.distance <= 500.0f && hit.distance >= 100.0f)
                translation += transform.forward * Input.GetAxis("Mouse ScrollWheel") * zoom_speed;
            else if (hit.distance > 500.0f)
                translation += transform.forward;
            else if (hit.distance < 100.0f)
                translation += -transform.forward;

            current_offset = transform.position - hit.transform.position;
        }

        if (Input.GetMouseButton(1))
            translation -= new Vector3(Input.GetAxis("Mouse X") * drag_speed * Time.deltaTime * (hit.distance / 100), 0, Input.GetAxis("Mouse Y") * drag_speed * Time.deltaTime * (hit.distance / 100));
        else
            translation += new Vector3(Input.GetAxis("Horizontal") * drag_speed * Time.deltaTime * (hit.distance / 100), 0, Input.GetAxis("Vertical") * drag_speed * Time.deltaTime * (hit.distance / 100));

        transform.position += translation;
        translation = Vector3.zero;
    }

    public void CenterOnTarget(Transform target)
    {
        if (target)
        {
            current_target = target;
            MoveToPosition(target.position);
        }
    }

    IEnumerator MoveToPosition(Vector3 new_pos)
    {
        Vector3 target = new_pos + current_offset;
        while(Vector3.Distance(transform.position, target) > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, target, 0.25f);
            yield return null;
        }
        transform.position = target;
    }
}
