using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public bool isGrounded;
    public bool onSteepSlope;
    public float slopeLimit;
    public bool printToConsole;
    public LayerMask collisionLayers;
    public Vector3 groundContactNormal;

    private void Start()
    {
        isGrounded = false;
    }

    public bool Grounded()
    {
        return isGrounded;
    }

    public bool SteepSlope()
    {
        return onSteepSlope;
    }

    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
        onSteepSlope = false;
        groundContactNormal = Vector3.up;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Environment"))
        {
            groundContactNormal = collision.contacts[0].normal;
            if(Vector3.Angle(groundContactNormal, Vector3.up) < slopeLimit)
            {
                isGrounded = true;
                onSteepSlope = false;
            } else
            {
                isGrounded = false;
                onSteepSlope = true;
            }
            //if (printToConsole) print("OH SHIT");
        }
    }
}
