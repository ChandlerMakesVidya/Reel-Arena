using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPistol : Gun
{
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
        animator.Play("pistol_shoot");
        fireSound.Play();

        RaycastHit hit;
        if(Physics.Raycast(playerView.transform.position, playerView.transform.forward, out hit, maxRange, impactLayers))
        {
            //print(hit.transform.name);

            Enemy enemy = hit.transform.GetComponent<Enemy>();
            if(enemy != null)
            {
                int damage = Mathf.RoundToInt(baseDamage * damageFalloff.Evaluate(hit.distance / damageFalloffRange) * Randomizer.PlayerDamageModifier);
                enemy.TakeDamage(damage);
                UIManager.UI.ShowHitmarker();
                TempSoundManager.PlaySound(GameManager.GM.hitMarker);
                //print("dealed " + damage + " damage to enemy!");

                //hit.rigidbody.AddForce(-hit.normal * 128f);
            }

            GameObject ig = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(ig, 2f);
        }
    }
}
