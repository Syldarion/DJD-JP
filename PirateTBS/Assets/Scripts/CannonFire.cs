using UnityEngine;
using System.Collections;

public class CannonFire : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
    public Rigidbody projectile;
    public float speed = 1000;
    public Transform Target;

    void Update()
    {
        Rigidbody clone;
        // Put this in your update function
        if (Input.GetKeyDown(KeyCode.F))
        {

            // Instantiate the projectile at the position and rotation of this transform
            clone = Instantiate(projectile, transform.position, transform.rotation) as Rigidbody;
            clone.GetComponentInChildren<CannonBall>().SetTarget(Target,speed);

           
           // clone.velocity.Set();
            Destroy(clone.gameObject, 1);
        }
    }    
}
