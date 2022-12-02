using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
    PlayerMove playerMove;
    public AudioSource stepAudio;

    // Start is called before the first frame update
    void Start()
    {
        playerMove = GetComponent<PlayerMove>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerMove.Grounded() && playerMove.GetVelocity() > playerMove.movementSpeed / 2 && !stepAudio.isPlaying)
        {
            stepAudio.volume = Random.Range(0.8f, 1f);
            stepAudio.pitch = Random.Range(0.8f, 1.1f);
            stepAudio.Play();
        }
    }
}
