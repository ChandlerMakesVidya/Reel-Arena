using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SimpleWarp : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Warp());
    }

    public Vector3 NavRandomPoint(Vector3 origin, float distance)
    {
        Vector3 randomDirection = Random.insideUnitSphere * distance;
        randomDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randomDirection, out navHit, distance, -1);

        return navHit.position;
    }

    IEnumerator Warp()
    {
        while (true)
        {
            yield return new WaitForSeconds(2.0f);
            Vector3 random = NavRandomPoint(transform.position, 5.0f);
            GetComponent<NavMeshAgent>().Warp(random);
        }
    }
}
