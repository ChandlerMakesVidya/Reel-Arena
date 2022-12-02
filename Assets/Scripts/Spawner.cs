using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: Chandler Hummingbird
 * Date Created: Oct 30, 2020
 * Date Modified: Oct 30, 2020
 * Description: Simple script that continually spawns a single given enemy prefab. Additional logic
 * also checks to see if there are too many enemies in the game and restricts the amount of enemies
 * that come from one particular spawner.
 */

public class Spawner : MonoBehaviour
{
    public GameObject enemyToSpawn;
    public float spawnInterval;
    [Tooltip("The max number of active enemies originating from this particular spawner at any time. Set to 0 to have no limit.")]
    public int spawnLimit;
    [SerializeField] GameObject[] children;
    /*[Tooltip("If set >0, the spawner will not spawn enemies if there are already enemies within this distance of it.")]
    public float breathingRoom;*/

    private void Start()
    {
        if(spawnLimit > 0)
        {
            children = new GameObject[spawnLimit];
        }
        StartCoroutine(SpawnRoutine());
    }

    bool FamilyIsFull()
    {
        //only care if spawnLimit is more than 0, treat negative numbers as 0
        if (spawnLimit < 1) return false;
        for(int i = 0; i < children.Length; i++)
        {
            if (children[i] == null) return false;
        }
        return true;
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            if(FindObjectsOfType<Enemy>().Length + 1 < GameManager.GM.enemyCap
                && !FamilyIsFull() )
            {
                GameObject child = Instantiate(enemyToSpawn, transform.position, transform.rotation);
                if(spawnLimit > 0)
                {
                    bool placed = false;
                    for(int i = 0; i < children.Length; i++)
                    {
                        if(children[i] == null)
                        {
                            children[i] = child;
                            placed = true;
                        }
                        if (placed) break;
                    }
                }
            }
            else
            {
                if(FindObjectsOfType<Enemy>().Length + 1 >= GameManager.GM.enemyCap)
                {
                    print("enemy cap reached!");
                }
            }
        }
    }
}
