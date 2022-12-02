using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileEnemyBullet : MonoBehaviour
{
    public int damage;
    public float speed;
    public float aliveTime;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(this, aliveTime);
    }

    private void Update()
    {
        rb.velocity = transform.forward * speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);
        }

        Destroy(this.gameObject);
    }
}
