using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundAudio : MonoBehaviour
{
    public AudioClip backgroundMusic;
    private AudioSource audioSource;
    private bool isMusicPlaying;
    public GameObject isPlayingState;
    
    public float volume = 0.5f;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (backgroundMusic != null)
        {
            audioSource.clip = backgroundMusic;
            audioSource.loop = true; 
            audioSource.volume = volume;
            audioSource.Play();

            isMusicPlaying = true;
        }
    }

    public void ControlAudio()
    {
        if (isMusicPlaying)
        {
            audioSource.Stop();
            isPlayingState.SetActive(true);
            isMusicPlaying = false;
        }
        else
        {
            audioSource.Play();
            isPlayingState.SetActive(false);
            isMusicPlaying = true;
        }
    }
}
