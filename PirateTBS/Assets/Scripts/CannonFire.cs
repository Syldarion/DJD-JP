using UnityEngine;

public class CannonFire : MonoBehaviour
{
    public GameObject cannonBall;
    public float rateOfFire = 0.5f;
    private float fireDelay;
    public float speed = 1000;
    public float randomness = 50.0f;
    public Transform Target;

    void Update()
    {   
        if (Input.GetKeyDown(KeyCode.F) && Time.time > fireDelay)
        {
            float temp = speed;
            fireDelay = Time.time + rateOfFire;
            Debug.Log("F key pressed");
            
            Vector3 RightDir = transform.position + new Vector3(10, 0, 0);
            Vector3 LeftDir = transform.position - new Vector3(10, 0, 0);
            //Vector3 UpDir = transform.position + new Vector3(0, 0, 10);
            //Vector3 DownDir = transform.position - new Vector3(0, 0, 10);

            speed += Random.Range(-randomness, randomness);
            GameObject clone = Instantiate(cannonBall, transform.position + new Vector3 (0, 30), Quaternion.identity) as GameObject;
            if (Vector3.Distance(Target.position, LeftDir) > Vector3.Distance(Target.position, RightDir))
            {  
                clone.GetComponent<Rigidbody>().AddForce((transform.right * speed) + (transform.up * speed), ForceMode.Impulse);
            }
            else if (Vector3.Distance(Target.position, LeftDir) < Vector3.Distance(Target.position, RightDir))
            {
                clone.GetComponent<Rigidbody>().AddForce((-transform.right * speed) + (transform.up * speed), ForceMode.Impulse);
            }
            //if(Vector3.Distance(Target.position, UpDir) > Vector3.Distance(Target.position, DownDir))
            //{
            //    clone.GetComponent<Rigidbody>().AddForce((-transform.forward * speed), ForceMode.Impulse);
            //}
            //else if (Vector3.Distance(Target.position, UpDir) < Vector3.Distance(Target.position, DownDir))
            //{
            //    clone.GetComponent<Rigidbody>().AddForce((transform.forward * speed), ForceMode.Impulse);
            //}
            speed = temp;  
        }
    }    
}
