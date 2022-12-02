using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempSoundManager : MonoBehaviour
    //optimized from the same TempSound system used in Ichikketsu
{
    static GameObject listener;

    //clip
    //clip + volume
    //clip + volume + position
    //clip + volume + position + mindistance + maxdistance

    public static void PlaySound(AudioClip sound)
    {
        if(listener == null) listener = FindObjectOfType<AudioListener>().gameObject;
        GameObject gameObject = new GameObject("tempSound");
        gameObject.transform.SetParent(listener.transform);
        gameObject.transform.localPosition = Vector3.zero;
        AudioSource tempSound = gameObject.AddComponent<AudioSource>();
        tempSound.playOnAwake = false;
        PlaySound(gameObject, sound, 1f, 1f, 2f);
    }

    public static void PlaySound(AudioClip sound, float volume)
    {
        if (listener == null) listener = FindObjectOfType<AudioListener>().gameObject;
        GameObject gameObject = new GameObject("tempSound");
        gameObject.transform.SetParent(listener.transform);
        gameObject.transform.localPosition = Vector3.zero;
        AudioSource tempSound = gameObject.AddComponent<AudioSource>();
        tempSound.playOnAwake = false;
        PlaySound(gameObject, sound, volume, 1f, 2f);
    }

    public static void PlaySound(AudioClip sound, float volume, Vector3 position)
    {
        GameObject gameObject = new GameObject("tempSound");
        gameObject.transform.position = position;
        AudioSource tempSound = gameObject.AddComponent<AudioSource>();
        tempSound.playOnAwake = false;
        PlaySound(gameObject, sound, volume, 1f, 2f);
    }

    public static void PlaySound(AudioClip sound, float volume, Vector3 position, float minDistance, float maxDistance)
    {
        GameObject gameObject = new GameObject("tempSound");
        gameObject.transform.position = position;
        AudioSource tempSound = gameObject.AddComponent<AudioSource>();
        tempSound.playOnAwake = false;
        PlaySound(gameObject, sound, volume, minDistance, maxDistance);
    }

    private static void PlaySound(GameObject prefab, AudioClip a, float volume, float minDistance, float maxDistance)
    {
        AudioSource sA = prefab.GetComponent<AudioSource>();
        sA.clip = a;
        sA.volume = volume;
        sA.minDistance = minDistance;
        sA.maxDistance = maxDistance;
        sA.rolloffMode = AudioRolloffMode.Linear;
        sA.spatialBlend = 1.0f;
        sA.Play();
        Destroy(prefab, a.length + 1.0f);
    }
}
