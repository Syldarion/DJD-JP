using UnityEngine;
using System.Collections;

public class Cannonball : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        Destroy(this.gameObject);  
    }
}
