using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torpedo : Enemy
{
    [Header("Torpedo Settings")]
    public float minSpeed = 4.0f;
    public float maxSpeed = 8.0f;
    public float maxSteerForce = 3.0f;

    public LayerMask obstacleMask;
    public float boundsRadius = 0.27f;
    public float collisionAvoidDist = 5.0f;

    public float targetWeight = 1.0f;
    public float avoidCollisionWeight = 10.0f;
    [Space]
    public GameObject burstEffect;
    public AudioClip burstSound;
    public float burstRadius;
    public AnimationCurve burstFalloff;
    
    Vector3 position;
    Vector3 forward;
    Vector3 velocity;
    Vector3 acceleration;
    Transform cachedTransform;

    protected override void Awake()
    {
        base.Awake();
        cachedTransform = transform;
        position = cachedTransform.position;
        forward = cachedTransform.forward;
        float startSpeed = (minSpeed + maxSpeed) / 2;
        velocity = transform.forward * startSpeed;
    }

    protected override void Update()
    {
        Vector3 acceleration = Vector3.zero;
        /*
        Vector3 offsetToTarget = (player.transform.position - position);
        acceleration = SteerTowards(offsetToTarget) * targetWeight;
        
        if (IsHeadingForCollision())
        {
            Vector3 collisionAvoidDir = ObstacleRays();
            Vector3 collisionAvoidForce = SteerTowards(collisionAvoidDir) * avoidCollisionWeight;
            acceleration += collisionAvoidForce;
        }
        */
        Vector3 offsetToTarget = (player.transform.position - position);

        if (CanSeePlayer())
        {
            acceleration = SteerTowards(offsetToTarget) * targetWeight;
        }
        else
        {
            acceleration = SteerTowards(offsetToTarget);

            if (IsHeadingForCollision())
            {
                Vector3 collisionAvoidDir = ObstacleRays();
                Vector3 collisionAvoidForce = SteerTowards(collisionAvoidDir) * avoidCollisionWeight;
                acceleration += collisionAvoidForce;
            }
        }
        velocity += acceleration * Time.deltaTime;
        float speed = velocity.magnitude;
        Vector3 dir = velocity / speed;
        speed = Mathf.Clamp(speed, minSpeed, maxSpeed);
        velocity = dir * speed;

        cachedTransform.position += velocity * Time.deltaTime;
        cachedTransform.forward = dir;
        position = cachedTransform.position;
        forward = dir;
    }

    bool IsHeadingForCollision()
    {
        RaycastHit hit;
        if(Physics.SphereCast(position, boundsRadius, forward, out hit, collisionAvoidDist, obstacleMask))
        {
            return true;
        } else { }
        return false;
    }

    Vector3 ObstacleRays()
    {
        Vector3[] rayDirections = BoidHelper.directions;

        for(int i = 0; i < rayDirections.Length; i++)
        {
            Vector3 dir = cachedTransform.TransformDirection(rayDirections[i]);
            Ray ray = new Ray(position, dir);
            if (!Physics.SphereCast(ray, boundsRadius, collisionAvoidDist, obstacleMask))
            {
                return dir;
            }
        }
        return forward;
    }

    Vector3 SteerTowards(Vector3 vector)
    {
        Vector3 v = vector.normalized * maxSpeed - velocity;
        return Vector3.ClampMagnitude(v, maxSteerForce);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer != 12)
        {
            Instantiate(burstEffect, transform.position, Quaternion.identity);
            TempSoundManager.PlaySound(burstSound, 1.0f, transform.position, 1.0f, 25.0f);
            if(collision.gameObject.layer == 9)
            {
                collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);
            }
            Die();
        }
    }
}
