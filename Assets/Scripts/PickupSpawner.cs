using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupSpawner : MonoBehaviour
{
    public GameObject pickup;
    public float respawnTimer;

    private void Start()
    {
        Instantiate(pickup, transform.position, Quaternion.identity, transform);
    }

    public void RespawnPickup()
    {
        StartCoroutine(Respawn());
    }

    public IEnumerator Respawn()
    {
        //print("RESPAWN");
        yield return new WaitForSeconds(respawnTimer);
        //print("should spawn now");
        Instantiate(pickup, transform.position, Quaternion.identity, transform);
    }
}
