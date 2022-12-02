using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grunt : Enemy
{
    [Header("Grunt Settings")]
    public float minEngagementDistance;
    public Transform projectileOrigin;
    public GameObject projectile;
    private bool inRange;
    private bool firing;
    public float fireRate;
    public float movementTimer;
    private float movementTimerReset;
    public Vector2 attackTimer;
    private float _attackTimer;
    bool canReachPlayer;
    
    public enum Behavior
    {
        Following,  //get into engagement range of the player
        Attacking,  //move around and attack the player
        Wandering   //move around, if cannot reach player
    }

    public Behavior behavior;

    private void Start()
    {
        movementTimerReset = movementTimer;
        InvokeRepeating("CheckPlayerPath", 0.0f, 0.5f);
    }

    void CheckPlayerPath()
    {
        canReachPlayer = CheckPath(player.transform.position);
    }

    protected override void Update()
    {
        base.Update();

        if(!firing && (Vector3.Distance(player.transform.position, transform.position) > minEngagementDistance || CanSeePlayer() == false))
        {
            if (canReachPlayer)
            {
                behavior = Behavior.Following;
            }
            else
            {
                behavior = Behavior.Wandering;
            }
        }
        else
        {
            behavior = Behavior.Attacking;
        }

        switch (behavior)
        {
            case Behavior.Following:
                inRange = false;
                if (!firing)
                {
                    navAgent.isStopped = false;
                    //navAgent.SetDestination(player.transform.position);
                    //check if player can be reached
                }
                break;
            case Behavior.Wandering:
                //move in random directions
                movementTimer -= Time.deltaTime * Randomizer.EnemySpeedModifier;
                if (!firing)
                {
                    navAgent.isStopped = false;
                    if (movementTimer <= 0.0f)
                    {
                        MoveRandomPoint();
                        movementTimer = movementTimerReset;
                    }
                }
                break;
            case Behavior.Attacking:
                //move in random directions and fire projectiles
                movementTimer -= Time.deltaTime * Randomizer.EnemySpeedModifier;

                if (!inRange)
                {
                    _attackTimer = Random.Range(attackTimer.x, attackTimer.y);
                    inRange = true;
                    break;
                }

                if (!firing)
                {
                    navAgent.isStopped = false;
                    _attackTimer -= Time.deltaTime * Randomizer.EnemySpeedModifier;
                    if(movementTimer <= 0.0f)
                    {
                        MoveRandomPoint();
                        movementTimer = movementTimerReset;
                    }
                }

                if (_attackTimer <= 0.0f && CanSeePlayer())
                {
                    firing = true;
                    navAgent.isStopped = true;
                    _attackTimer = Random.Range(attackTimer.x, attackTimer.y);
                    StartCoroutine(Fire());
                }
                break;
        }
    }

    IEnumerator Fire()
    {
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(fireRate / Randomizer.EnemySpeedModifier);
            transform.rotation = Quaternion.LookRotation(
            -(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z) - transform.position),
            Vector3.up);

            Instantiate(projectile, projectileOrigin.position, Quaternion.LookRotation(
                player.transform.position - projectileOrigin.transform.position, Vector3.up));
        }
        yield return new WaitForSeconds(fireRate / Randomizer.EnemySpeedModifier);
        firing = false;
    }

    void MoveRandomPoint()
    {
        int attempt = 3;
        Vector3 point;
        do
        {
            point = NavRandomPoint2(transform.position, 20.0f);
            attempt--;
        } while (Physics.Raycast(transform.position, point - transform.position, 5.0f) || attempt > 1);
        navAgent.SetDestination(point);
    }
}
