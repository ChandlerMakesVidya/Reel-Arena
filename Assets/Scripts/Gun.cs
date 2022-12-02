using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public int weaponID; //unique id for this weapon, used for ammo checks
    public int baseDamage; //HP damage vs enemies
    public AnimationCurve damageFalloff; //how damage scales based on distance from enemies
    public float damageFalloffRange; //range at which shots will begin doing minimum damage
    public int maxAmmo; //maximum amount of ammo this weapon can hold, set to 0 for infinite ammo
    public AudioSource emptySound;
    public float maxRange; //max range of shots
    public LayerMask impactLayers; //shots can only impact on objects in these layers
    public float fireRate; //fastest speed at which the gun can fire shots
    public float recoilPunch; //amount (in rotation) that the camera will punch up when firing a shot
    public float accuracy; //max spread angle for shots
    protected float nextFireAvailableTime;

    public AudioSource fireSound;
    public ParticleSystem muzzleflash;
    public GameObject impactEffect;
    public Transform rocketSpawnPoint;
    protected Camera playerView;
    protected Animator animator;
    protected WeaponSwitch wpnSwitch;
    protected PlayerLook playerLook;
    protected WeaponSwitch wepSwitch;

    protected virtual void Start()
    {
        playerView = Camera.main;
        animator = GetComponent<Animator>();
        wpnSwitch = FindObjectOfType<WeaponSwitch>();
        nextFireAvailableTime = float.NegativeInfinity;
        playerLook = FindObjectOfType<PlayerLook>();
        wepSwitch = FindObjectOfType<WeaponSwitch>();
        wepSwitch.weaponAmmo[weaponID] = maxAmmo;
    }
    
    protected virtual void Update()
    {
        //print(Input.GetAxis("Mouse ScrollWheel"));
        if (Time.time >= nextFireAvailableTime)
        {
            wpnSwitch.canSwitchWeapon = true;
            if (Input.GetButton("Fire"))
            {
                if (wepSwitch.weaponAmmo[weaponID] != 0 || maxAmmo == 0 || Randomizer.InfiniteAmmo)
                {
                    nextFireAvailableTime = Time.time + 1f / (fireRate * Randomizer.FireSpeedModifier);
                    if(!Randomizer.InfiniteAmmo)
                        wepSwitch.weaponAmmo[weaponID]--;
                    Shoot();
                    playerLook.PunchView(recoilPunch);
                }
            }

            if (Input.GetButtonDown("Fire"))
            {
                if(wepSwitch.weaponAmmo[weaponID] == 0 && maxAmmo != 0 && !Randomizer.InfiniteAmmo)
                {
                    emptySound.Play();
                }
            }
        }
        else wpnSwitch.canSwitchWeapon = false;

        if (wepSwitch.weaponAmmo[weaponID] < 0) wepSwitch.weaponAmmo[weaponID] = 0;

        if (wepSwitch.weaponAmmo[weaponID] > maxAmmo) wepSwitch.weaponAmmo[weaponID] = maxAmmo;

        if (Randomizer.FireSpeedModifier != 1.0f)
            animator.speed = Randomizer.FireSpeedModifier;
        else animator.speed = 1.0f;
    }

    protected virtual void Shoot()
    {
        
    }
}
