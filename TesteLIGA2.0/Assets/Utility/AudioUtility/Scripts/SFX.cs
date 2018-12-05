using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SFX
{
    public string tag;
    public AudioClip audio;
    public SFX(string tag, AudioClip audio)
    {
        this.tag = tag;
        this.audio = audio;
    }
}