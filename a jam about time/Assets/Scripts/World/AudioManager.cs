using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script that manages all audio sources and clips on an object
public class AudioManager : MonoBehaviour
{
    // TODO Set up system where other scripts can request to play audio 
    public AudioSource source;
    public AudioClip[] audioClips;
    
    public void PlayAudio(int clip){
        if (!source.isPlaying){
            source.clip = audioClips[clip];
            source.Play();
        }
    }
}
