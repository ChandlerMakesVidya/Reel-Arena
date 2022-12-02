using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunShotgun : Gun
{
    public int pelletsPerShot; //how many shots are going to be fired by this weapon per fire?

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void Shoot()
    {
        muzzleflash.Play();
        animator.Play("dblshotgun_shoot");
        fireSound.Play();
        bool hitmark = false;

        for(int i = 0; i < pelletsPerShot; i++)
        {
            Vector3 dir = playerView.transform.forward;
            //print(transform.forward);
            dir.x += Random.Range(-accuracy, accuracy);
            dir.y += Random.Range(-accuracy, accuracy);
            dir.z += Random.Range(-accuracy, accuracy);

            RaycastHit hit;
            if(Physics.Raycast(playerView.transform.position, dir, out hit, maxRange, impactLayers))
            {
                Enemy enemy = hit.transform.GetComponent<Enemy>();
                if (enemy != null)
                {
                    int damage = Mathf.RoundToInt(baseDamage * damageFalloff.Evaluate(hit.distance / damageFalloffRange) * Randomizer.PlayerDamageModifier);
                    enemy.TakeDamage(damage);
                    if(hitmark == false)
                    {
                        TempSoundManager.PlaySound(GameManager.GM.hitMarker);
                        UIManager.UI.ShowHitmarker();
                        hitmark = true;
                    }
                    //print("dealed " + damage + " damage to enemy!");

                    //hit.rigidbody.AddForce(-hit.normal * 128f);
                }

                GameObject ig = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(ig, 2f);
            }
        }
    }
}
