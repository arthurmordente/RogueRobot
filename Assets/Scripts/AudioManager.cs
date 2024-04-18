using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Declaração das variáveis públicas para os áudios
    public AudioClip audioDeath1;
    public AudioClip audioDeath2;
    public AudioClip audioCoin;
    public AudioClip audioSkill;

    private AudioSource audioSource;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void PlayAudio1()
    {
        audioSource.clip = audioCoin;
        audioSource.Play();
    }

    public void PlayAudio2()
    {
        audioSource.clip = audioDeath1;
        audioSource.Play();
    }

    public void PlayAudio3()
    {
        audioSource.clip = audioDeath2;
        audioSource.Play();
    }

    public void PlayAudio4()
    {
        audioSource.clip = audioSkill;
        audioSource.Play();
    }
}
