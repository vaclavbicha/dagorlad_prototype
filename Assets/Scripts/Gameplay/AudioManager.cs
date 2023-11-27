using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Sound[] sounds;

    public void Awake()
    {
        foreach (Sound s in sounds)
        {
            s.source = transform.GetChild(1).gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.outputAudioMixerGroup = s.OutputMixer;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s != null)
        {
            s.source.Play();
        }
        else
        {
            Debug.LogError($"Sound {name} not found");
        }
    }
    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s != null)
        {
            s.source.Stop();
        }
        else
        {
            Debug.LogError($"Sound {name} not found");
        }
    }
    public void SetMusicVolume(float volume)
    {
        volume -= 80;
        audioMixer.SetFloat("Music", volume);
    }
    public void SetSFXVolume(float volume)
    {
        volume -= 80;
        audioMixer.SetFloat("SoundEffects", volume);
    }
    public void SetMasterVolume(float volume)
    {
        volume -= 45;
        audioMixer.SetFloat("Master", volume);
    }
}
//GameMasterScript.instance.GetComponent<AudioManager>().Play(GameMasterScript.instance.CurrentSettings.Music);
//public AudioSource[] AudioSources;
//public OnCollision[] BodyParts;

//public float maxVolume = 0f;

//public GameObject ImpactAnimation;
//// Start is called before the first frame update
//void Start()
//{
//    AudioSources = GetComponents<AudioSource>();
//    maxVolume = AudioSources[0].volume;
//    foreach (OnCollision obj in BodyParts)
//    {
//        //obj.LoggEvents = true;
//        //obj.AddEvent("Enter", "CameraWall", oncoldebug);
//        obj.AddEvent("Enter", "CameraWall", OnCollisionSound);
//    }
//}
//public void PlaySound(float volume)
//{
//    for (int i = 0; i < AudioSources.Length; i++)
//    {
//        if (!AudioSources[i].isPlaying)
//        {
//            AudioSources[i].volume = volume;
//            AudioSources[i].Play();
//            i = AudioSources.Length;
//        }
//    }
//}