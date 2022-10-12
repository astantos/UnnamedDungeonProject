using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMovement : MonoBehaviour
{
    public float MaxSpeed;

    protected Rigidbody rbody;

    protected void Awake()
    {
        rbody = GetComponent<Rigidbody>();
    }

    public void SetSpeed(Vector3 direction)
    {
        Vector3 newVel = direction.normalized * MaxSpeed;
        newVel.y = rbody.velocity.y;
        rbody.velocity = transform.TransformDirection(newVel);
    }
}
