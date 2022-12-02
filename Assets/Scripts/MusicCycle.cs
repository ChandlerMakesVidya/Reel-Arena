using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicCycle : MonoBehaviour
{
    public AudioClip[] musicTracks;
    public string[] trackNames;
    private Text currentTrackText;
    AudioSource aud;

    private void Start()
    {
        currentTrackText = UIManager.UI.currentTrack;
        aud = GetComponent<AudioSource>();
        StartCoroutine(PlayMusic());
    }

    IEnumerator PlayMusic()
    {
        while (true)
        {
            int trackIndex = Random.Range(0, musicTracks.Length);
            aud.clip = musicTracks[trackIndex];
            currentTrackText.text = trackNames[trackIndex];
            aud.Play();
            yield return new WaitForSecondsRealtime(aud.clip.length);
        }
    }
}
