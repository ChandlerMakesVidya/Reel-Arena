using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public int giveHealth;
    public int ammoType;
    public int giveAmmo;
    public AudioClip pickupSound;
    Vector3 posOffset = new Vector3();
    Vector3 tempPos = new Vector3();

    // Start is called before the first frame update
    void Start()
    {
        posOffset = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * 50.0f * Time.deltaTime);

        tempPos = posOffset;
        tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * 1.0f) * 0.5f;
        transform.position = tempPos;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            GetComponentInParent<PickupSpawner>().RespawnPickup();
            other.GetComponent<PlayerHealth>().Heal(giveHealth);
            FindObjectOfType<WeaponSwitch>().GetComponent<WeaponSwitch>().weaponAmmo[ammoType] += giveAmmo;
            TempSoundManager.PlaySound(pickupSound);
            Destroy(gameObject);
        }
    }
}
