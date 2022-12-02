using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Basic Settings")]
    public int health;
    public int pointsToAward;
    public int damage;
    public float speed;
    public float minMissileDistance;
    public float minWarpDistance;
    public float minWarpPlayerDistance;
    public float maxWarpPlayerDistance;
    public LayerMask sightLayers;
    //TorpedoMove tm;
    public AudioSource painSound;
    public float painChance;
    [Space]
    //unused until i figure out how to make exploding enemies not destory memory
    [Header("Explosion Settings")]
    public float explosiveRadius;
    public LayerMask explosiveLayer;
    public float explosiveForce;
    public int explosiveDamage;
    public AnimationCurve explosiveDamageFalloff;
    public GameObject explosionDebris;
    public GameObject explosionSmoke;
    protected bool kill = false;

    protected PlayerMovePhys player;
    protected NavMeshAgent navAgent;

    protected virtual void Awake()
    {
        player = FindObjectOfType<PlayerMovePhys>();
        navAgent = GetComponent<NavMeshAgent>();
        //tm = GetComponent<TorpedoMove>();
    }

    protected virtual void Update()
    {
        if(navAgent != null)
        {
            navAgent.speed = speed * Randomizer.EnemySpeedModifier;
        }
    }

    protected bool CanSeePlayer()
    {
        RaycastHit los;
        Vector3 dir = player.transform.position - transform.position;
        if (Physics.Raycast(transform.position, dir, out los, Mathf.Infinity, sightLayers))
        {
            if (los.collider.gameObject == player.gameObject)
            {
                return true;
            }
        }
        return false;
    }

    public void TakeDamage(int amount)
    {
        //tm.shot = true;
        //tm.audioSource.Stop();
        if(painSound != null && painSound.isPlaying == false) painSound.Play();
        health -= amount;

        if (health <= 0f)
        {
            GameManager.gameScore += pointsToAward;
            if (kill == false)
            {
                TempSoundManager.PlaySound(GameManager.GM.killMarker);
                kill = true;
            }
            Die();
        }
    }

    public void TakeDamage(int amount, bool railgunDamage)
    {
        //tm.shot = true;
        //tm.audioSource.Stop();
        if (painSound != null && painSound.isPlaying == false) painSound.Play();
        health -= amount;

        if (health <= 0f)
        {
            if (railgunDamage)
            {
                GameManager.gameScore += pointsToAward;
                if (kill == false)
                {
                    TempSoundManager.PlaySound(GameManager.GM.killMarker);
                    kill = true;
                }
                DieRailgun();
            }
            else
            {
                GameManager.gameScore += pointsToAward;
                if (kill == false)
                {
                    TempSoundManager.PlaySound(GameManager.GM.killMarker);
                    kill = true;
                }
                Die();
            }
        }
    }

    protected void Die()
    {
        /*if (Randomizer.EnemiesExplodeUponDeath)
        {
            Collider[] objects = Physics.OverlapSphere(transform.position, explosiveRadius, explosiveLayer);
            foreach (Collider obj in objects)
            {
                Rigidbody objrb = obj.GetComponent<Rigidbody>();
                if (objrb != null)
                {
                    objrb.AddExplosionForce(explosiveForce, transform.position, explosiveRadius);
                }

                float damage = explosiveDamage * explosiveDamageFalloff.Evaluate(Vector3.Distance(transform.position, obj.transform.position) / explosiveRadius);

                if (obj.GetComponent<Enemy>() != null)
                {
                    obj.GetComponent<Enemy>().TakeDamage(damage);
                }

                if(obj.GetComponent<PlayerHealth>() != null)
                {
                    obj.GetComponent<PlayerHealth>().TakeDamage(Mathf.RoundToInt(damage));
                }
            }
            Instantiate(explosionDebris, transform.position, Quaternion.identity);
            Instantiate(explosionSmoke, transform.position, Quaternion.identity);
        }*/
        Destroy(gameObject);
    }

    protected void DieRailgun()
    {
        Collider[] objects = Physics.OverlapSphere(transform.position, explosiveRadius, explosiveLayer);
        foreach (Collider obj in objects)
        {
            Rigidbody objrb = obj.GetComponent<Rigidbody>();
            if (objrb != null)
            {
                objrb.AddExplosionForce(explosiveForce, transform.position, explosiveRadius);
            }

            int damage = Mathf.RoundToInt(explosiveDamage * explosiveDamageFalloff.Evaluate(Vector3.Distance(transform.position, obj.transform.position) / explosiveRadius));

            if (obj.GetComponent<Enemy>() != null)
            {
                obj.GetComponent<Enemy>().TakeDamage(damage);
            }

            if (obj.GetComponent<PlayerHealth>() != null)
            {
                obj.GetComponent<PlayerHealth>().TakeDamage(damage);
            }
        }
        Instantiate(explosionDebris, transform.position, Quaternion.identity);
        //Instantiate(explosionSmoke, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    protected bool CheckPath(Vector3 target)
    {
        NavMeshPath path = new NavMeshPath();
        navAgent.CalculatePath(target, path);
        if (path.status == NavMeshPathStatus.PathPartial || path.status == NavMeshPathStatus.PathInvalid)
        {
            return false;
        }
        return true;
    }

    public Vector3 NavRandomPoint(Vector3 origin, float distance)
    {
        Vector3 randomDirection = Random.insideUnitSphere * distance;
        randomDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randomDirection, out navHit, distance, -1);

        return navHit.position;
    }

    public Vector3 NavRandomPoint2(Vector3 origin, float distance)
    {
        Vector2 rand = Random.insideUnitCircle * distance;
        Vector3 randomDirection = new Vector3(rand.x, 0.0f, rand.y);
        randomDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randomDirection, out navHit, distance, -1);

        return navHit.position;
    }

    public bool Warp(Vector3 origin, float minDistance, float maxDistance)
    {
        float finalDistance = Random.Range(minDistance, maxDistance);
        int repeat = 5; //what if the player is flying far away from the NavMesh?

        Vector3 warpPoint = Vector3.zero;
        
        //make sure the distance is within bounds
        while(repeat > 0)
        {
            warpPoint = NavRandomPoint(origin, finalDistance);
            if(Vector3.Distance(origin, warpPoint) < minDistance)
            {
                repeat--;
                if(repeat <= 0)
                {
                    return false;
                }
                continue;
            }
            repeat = 0;
        }
        //repeat prevents the function from running infinitely by aborting after five failed attempts.
        //NavMesh.SamplePosition in NavRandomPoint() touts to prevent this from occurring, but I say better safe than sorry.

        //do a vision check
        bool visionCheck = Physics.Raycast(
            origin,
            warpPoint - origin,
            Vector3.Distance(origin, warpPoint) * 0.999f,
            LayerMask.NameToLayer("Environment"));
        if (visionCheck)
        {
            return false;
        }

        navAgent.Warp(warpPoint);
        return true;
    }
}
