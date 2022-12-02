using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSMG : Gun
{
    /// <summary>
    /// As the smg outputs sustained fire, the accuracy of the weapon becomes worse per shot (inaccuracyPerShot)
    /// up to a certain point (maxInaccuracy). The accuracy can only recover when the weapon is not firing (accuracyRecoveryRate)
    /// startInaccuracy can be set to a negative float if you want more than just the first shot to be completely accurate
    /// as inaccuracy is only applied when it is a positive float.
    /// </summary>
    
    public float startInaccuracy;
    private float inaccuracy;
    public float inaccuracyPerShot;
    public float maxInaccuracy;
    public float accuracyRecoveryRate;

    protected override void Start()
    {
        base.Start();
        inaccuracy = startInaccuracy;
    }

    protected override void Update()
    {
        if (Time.time >= nextFireAvailableTime && inaccuracy > startInaccuracy)
        {
            inaccuracy -= accuracyRecoveryRate * Time.deltaTime;
        }

        if (inaccuracy < startInaccuracy)
        {
            inaccuracy = startInaccuracy;
        }
        else if (inaccuracy > maxInaccuracy)
        {
            inaccuracy = maxInaccuracy;
        }

        if (inaccuracy > 0f)
            accuracy = inaccuracy;
        else accuracy = 0f;

        float _rotate180 = accuracy * - 180f;
        Quaternion recoilTilt = Quaternion.Euler(_rotate180, 0f, 0f);

        transform.localRotation = recoilTilt;

        //print(inaccuracy);
        base.Update();
    }

    protected override void Shoot()
    {
        muzzleflash.Play();
        animator.Play("smg_shoot");
        fireSound.Play();
        inaccuracy += inaccuracyPerShot;

        Vector3 dir = playerView.transform.forward;
        //print(transform.forward);
        dir.x += Random.Range(-accuracy, accuracy);
        dir.y += Random.Range(-accuracy, accuracy);
        dir.z += Random.Range(-accuracy, accuracy);

        RaycastHit hit;
        if (Physics.Raycast(playerView.transform.position, dir, out hit, maxRange, impactLayers))
        {
            Enemy enemy = hit.transform.GetComponent<Enemy>();
            if (enemy != null)
            {
                int damage = Mathf.RoundToInt(baseDamage * damageFalloff.Evaluate(hit.distance / damageFalloffRange) * Randomizer.PlayerDamageModifier);
                enemy.TakeDamage(damage);
                TempSoundManager.PlaySound(GameManager.GM.hitMarker);
                UIManager.UI.ShowHitmarker();
                //print("dealed " + damage + " damage to enemy!");

                //hit.rigidbody.AddForce(-hit.normal * 128f);
            }

            GameObject ig = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(ig, 2f);
        }
    }
}
