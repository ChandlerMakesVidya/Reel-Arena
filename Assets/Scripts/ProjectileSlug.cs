using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSlug : MonoBehaviour
{
    public float speed;
    public LayerMask stopLayers;
    public TrailRenderer trail;
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = transform.up * speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        DetachTrail();
        Destroy(gameObject);
    }

    private void DetachTrail()
    {
        trail.transform.parent = null;
        trail.emitting = false;
        trail.autodestruct = true;
    }
}
