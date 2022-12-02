using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunRocketLauncher : Gun
{
    public GameObject rocket;
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
        animator.Play("rocketlauncher_shoot");
        fireSound.Play();
        Instantiate(rocket, rocketSpawnPoint.position, rocketSpawnPoint.rotation);
    }
}
