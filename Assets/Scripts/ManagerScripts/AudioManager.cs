using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    // Declaração das variáveis públicas para os áudios
    public AudioClip audioDeath1;
    public AudioClip audioDeath2;
    public AudioClip audioCoin;
    public AudioClip audioSkill;
    public AudioClip audioHit;
    public AudioClip audioMenu;
    public AudioClip audioMenu2;

    private AudioSource audioSource;
    [SerializeField] Slider volumeSlider;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if(!PlayerPrefs.HasKey("musicVolume"))
        {
            PlayerPrefs.SetFloat("musicVolume", 1);
            Load();
        }
        else{
            Load();
        }
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

    public void PlayAudio5()
    {
        audioSource.clip = audioHit;
        audioSource.Play();
    }

    public void PlayMenuAudio()
    {
        audioSource.clip = audioMenu;
        audioSource.Play();
    }

    public void PlayMenuAudio2()
    {
        Debug.Log("AudioSource: " + (audioSource == null ? "NULL" : "OK"));
        Debug.Log("AudioClip: " + (audioMenu2 == null ? "NULL" : "OK"));
        if (audioSource == null || audioMenu2 == null) return;
        audioSource.clip = audioMenu2;
        audioSource.Play();
    }
    public void ChangeVolume(){
        AudioListener.volume = volumeSlider.value;
        Save();
    }

    public void Load(){
        volumeSlider.value = PlayerPrefs.GetFloat("musicVolume");
    }

    private void Save(){
        PlayerPrefs.SetFloat("musicVolume", volumeSlider.value);
    }
}

