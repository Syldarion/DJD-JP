using UnityEngine;
using System.Collections;

public class Cannonball : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 5.0f);
    }

    void Update()
    {

    }

    public void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}