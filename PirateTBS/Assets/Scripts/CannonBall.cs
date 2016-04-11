using UnityEngine;
using System.Collections;

public class CannonBall : MonoBehaviour 
{
    public Transform target;
    public float speed = 1000;
    void Update()
    {
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);
    }
    public void SetTarget(Transform Target, float Speed)
    {
        target = Target;
        speed = Speed;
    }
}
