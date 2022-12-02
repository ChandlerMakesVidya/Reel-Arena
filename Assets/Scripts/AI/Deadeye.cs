using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deadeye : Enemy
{
    [Header("Deadeye Settings")]
    public float aimTimer;
    private float aimTimerReset;
    public Vector2 attackTimer;
    private float _attackTimer;
    public float movementTimer;
    private float movementTimerReset;
    public float shotTimer;
    private bool firing;
    //public GameObject head;
    public Vector3 aimDir;
    public LineRenderer laser;
    public GameObject shotTrail;
    public LayerMask shotLayers;
    public AudioSource shotSound;

    public enum Behavior
    {
        Moving,
        Attacking
    }

    public Behavior behavior;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        movementTimerReset = movementTimer;
        aimTimerReset = aimTimer;
        _attackTimer = Random.Range(attackTimer.x, attackTimer.y);
    }

    protected override void Update()
    {
        base.Update();

        if(_attackTimer <= 0.0f && CanSeePlayer())
        {
            behavior = Behavior.Attacking;
        }
        else
        {
            behavior = Behavior.Moving;
        }

        RaycastHit hit;
        float distance;
        if (Physics.Raycast(transform.position, aimDir, out hit))
        {
            distance = hit.distance;
        }
        else
        {
            distance = 1000.0f;
        }

        switch (behavior)
        {
            case Behavior.Moving:
                navAgent.isStopped = false;
                HideLaser();
                movementTimer -= Time.deltaTime * Randomizer.EnemySpeedModifier;
                _attackTimer -= Time.deltaTime * Randomizer.EnemySpeedModifier;

                if (movementTimer <= 0.0f)
                {
                    MoveRandomPoint();
                    movementTimer = movementTimerReset;
                }
                break;
            case Behavior.Attacking:
                navAgent.isStopped = true;
                if (!firing)
                {
                    //head.transform.LookAt(player.transform);
                    //transform.rotation = Quaternion.LookRotation((new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z) - transform.position), Vector3.up);

                    Vector3 lead = player.transform.position + player.Velocity * shotTimer;
                    aimDir = (lead - transform.position).normalized;

                    laser.SetPosition(0, transform.position);
                    laser.SetPosition(1, transform.position + aimDir * distance);
                    aimTimer -= Time.deltaTime * Randomizer.EnemySpeedModifier;

                    if(aimTimer <= 0.0f)
                    {
                        firing = true;
                        StartCoroutine(Fire());
                    }
                }
                break;
        }
    }

    IEnumerator Fire()
    {
        yield return new WaitForSeconds(shotTimer);
        LineRenderer st = Instantiate(shotTrail).GetComponent<LineRenderer>();
        st.SetPosition(0, transform.position);
        st.SetPosition(1, laser.GetPosition(1));
        //laser.enabled = false;
        HideLaser();
        shotSound.Play();
        RaycastHit hit;
        if(Physics.Raycast(st.GetPosition(0), st.GetPosition(1) - st.GetPosition(0), out hit))
        {
            if(hit.collider.gameObject.tag == "Player")
            {
                hit.collider.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);
            }
        }
        yield return new WaitForSeconds(0.5f);
        _attackTimer = Random.Range(attackTimer.x, attackTimer.y);
        aimTimer = aimTimerReset;
        firing = false;
    }

    void HideLaser()
    {
        //laser.enabled = false doesn't work so here we are
        laser.SetPosition(0, Vector3.zero);
        laser.SetPosition(1, Vector3.zero);
    }

    void MoveRandomPoint()
    {
        Vector3 point;
        do
        {
            point = NavRandomPoint2(transform.position, 20.0f);
        } while (Physics.Raycast(transform.position, point - transform.position, 5.0f));
        navAgent.SetDestination(point);
    }
}
