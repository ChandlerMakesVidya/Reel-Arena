using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileRocket : MonoBehaviour
{
    public float baseDamage;
    public AnimationCurve damageFalloff;
    public float speed;
    public float explosiveForce;
    public float explosiveRadius;
    public LayerMask explosiveLayer;
    public TrailRenderer trail;
    public AudioClip explosionSound;
    public GameObject explosionDebris;
    public GameObject explosionSmoke;
    Rigidbody rb;
    private bool detonate;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        //transform.position += transform.up * speed * Time.deltaTime;
        rb.velocity = transform.up * speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!detonate)
        {
            Collider[] objects = Physics.OverlapSphere(transform.position, explosiveRadius, explosiveLayer);
            foreach (Collider obj in objects)
            {
                Rigidbody objrb = obj.GetComponent<Rigidbody>();
                if (objrb != null)
                {
                    objrb.AddExplosionForce(explosiveForce, transform.position, explosiveRadius);
                }

                bool hitmark = false;
                if (obj.GetComponent<Enemy>() != null)
                {
                    int damage = Mathf.RoundToInt(baseDamage * damageFalloff.Evaluate(Vector3.Distance(transform.position, obj.transform.position) / explosiveRadius) * Randomizer.PlayerDamageModifier);
                    obj.GetComponent<Enemy>().TakeDamage(damage);
                    if(hitmark == false)
                    {
                        TempSoundManager.PlaySound(GameManager.GM.hitMarker);
                        UIManager.UI.ShowHitmarker();
                        hitmark = true;
                    }
                }
            }

            DetachTrail();
            TempSoundManager.PlaySound(explosionSound, 0.5f, transform.position, 10.0f, 50.0f);
            Instantiate(explosionDebris, transform.position, Quaternion.LookRotation(collision.contacts[0].normal));
            //Instantiate(explosionSmoke, transform.position, Quaternion.LookRotation(collision.contacts[0].normal));
            detonate = true;
        }

        Destroy(gameObject);
    }

    private void DetachTrail()
    {
        trail.transform.parent = null;
        trail.emitting = false;
        trail.autodestruct = true;
    }
}
