using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//deprecated

public class TorpedoMove : MonoBehaviour
{
    public float acceleration = 50f;
    public float velocityCap = 8f;
    float sqrVelocityCap { get { return velocityCap * velocityCap; } }
    public float rotationDampening = 0.5f;
    public float raycastOffset = 1.5f;
    public float raycastDistance = 10f;
    public LayerMask raycastLayers;
    public float timeToDestroyAfterShot;
    [HideInInspector] public bool shot;
    
    public Transform target;

    Rigidbody rb;
    [HideInInspector] public AudioSource audioSource;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (!shot)
        {
            Move();
            ObstacleAvoidance();
        } else
        {
            rb.angularDrag = 0.0f;
            timeToDestroyAfterShot -= Time.deltaTime;
            if (timeToDestroyAfterShot <= 0f)
            {
                Destroy(gameObject);
            }
        }
    }

    void Move()
    {
        transform.position += transform.forward * velocityCap * Randomizer.EnemySpeedModifier * Time.deltaTime;
        /*if(rb.velocity.magnitude > velocityCap)
        {
            float brakeSpeed = rb.velocity.magnitude - velocityCap;
            Vector3 normalizedVelocity = rb.velocity.normalized;
            Vector3 brakeVelocity = normalizedVelocity * brakeSpeed;

            rb.AddForce(-brakeVelocity);
        }*/
    }

    void Turn()
    {
        Vector3 pos = target.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(pos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationDampening * Time.deltaTime);
    }

    void ObstacleAvoidance()
    {
        RaycastHit hit;
        Vector3 offset = Vector3.zero;
        Vector3 left = transform.position - transform.right * raycastOffset;
        Vector3 right = transform.position + transform.right * raycastOffset;
        Vector3 up = transform.position + transform.up * raycastOffset;
        Vector3 down = transform.position - transform.up * raycastOffset;

        Debug.DrawRay(left, transform.forward * raycastDistance, Color.cyan);
        Debug.DrawRay(right, transform.forward * raycastDistance, Color.cyan);
        Debug.DrawRay(up, transform.forward * raycastDistance, Color.cyan);
        Debug.DrawRay(down, transform.forward * raycastDistance, Color.cyan);

        if(Physics.Raycast(left, transform.forward, out hit, raycastDistance, raycastLayers))
        {
            offset += Vector3.right;
        }
        else if (Physics.Raycast(right, transform.forward, out hit, raycastDistance, raycastLayers))
        {
            offset -= Vector3.right;
        }

        if (Physics.Raycast(up, transform.forward, out hit, raycastDistance, raycastLayers))
        {
            offset -= Vector3.up;
        }
        else if (Physics.Raycast(up, transform.forward, out hit, raycastDistance, raycastLayers))
        {
            offset += Vector3.up;
        }

        if(offset != Vector3.zero)
        {
            transform.Rotate(offset * 5f * Time.deltaTime);
        }
        else
        {
            Turn();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!shot)
        {
            if (collision.gameObject.tag == "Player")
            {
                collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(5 * (int)Randomizer.EnemyDamageModifier);
                Destroy(gameObject);
            }
        }
    }
}
