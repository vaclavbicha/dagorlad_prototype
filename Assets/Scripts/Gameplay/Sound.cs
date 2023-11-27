using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [NonSerialized]
    public AudioSource source;
    public AudioMixerGroup OutputMixer;

    [Range(0, 1)]
    public float volume;
    [Range(0.1f, 3)]
    public float pitch;
    public bool loop;
}
