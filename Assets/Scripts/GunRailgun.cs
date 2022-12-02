using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunRailgun : Gun
{
    public LayerMask enemyLayer;
    public GameObject slugTrail;
    public Transform railgunInfinity;
    public float selfKnockback;
    public AudioClip skewer;
    public Slider displaySlider;
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        displaySlider.value = Time.time;
    }

    protected override void Shoot()
    {
        animator.Play("railgun_shoot");
        fireSound.Play();
        displaySlider.minValue = Time.time;
        displaySlider.maxValue = nextFireAvailableTime;


        GameObject player = FindObjectOfType<PlayerMovePhys>().gameObject;
        player.GetComponent<Rigidbody>().AddForce(-playerView.transform.forward * selfKnockback);

        RaycastHit environmentHit = new RaycastHit();
        environmentHit.distance = float.PositiveInfinity;
        if (Physics.Raycast(playerView.transform.position, playerView.transform.forward, out environmentHit, float.PositiveInfinity, impactLayers))
        {
            GameObject ig = Instantiate(impactEffect, environmentHit.point, Quaternion.LookRotation(environmentHit.normal));
            TempSoundManager.PlaySound(skewer, 1.0f, environmentHit.point, 5.0f, 50.0f);
            Destroy(ig, 3f);
        }
        else
        {
            environmentHit.distance = float.PositiveInfinity;
        }

        LineRenderer st = Instantiate(slugTrail).GetComponent<LineRenderer>();
        st.SetPosition(0, rocketSpawnPoint.position);
        if (float.IsPositiveInfinity(environmentHit.distance))
        {
            st.SetPosition(1, playerView.transform.forward * 5000.0f);
        }
        else
        {
            st.SetPosition(1, environmentHit.point);
        }

        RaycastHit[] enemiesHit = Physics.RaycastAll(playerView.transform.position, playerView.transform.forward, environmentHit.distance, enemyLayer);
        bool hitmark = false;
        for (int i = 0; i < enemiesHit.Length; i++)
        {
            enemiesHit[i].transform.gameObject.GetComponent<Enemy>().TakeDamage((int)(baseDamage * Randomizer.PlayerDamageModifier), true);
            if (hitmark == false)
            {
                TempSoundManager.PlaySound(GameManager.GM.hitMarker);
                UIManager.UI.ShowHitmarker();
                hitmark = true;
            }
        }
    }
}
