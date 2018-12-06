using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerHelper : MonoBehaviour
{
    public AudioSource AS { get; private set; }
    
    public void Awake()
    {
        AS = GetComponent<AudioSource>();
        
        if (_App.SoundManager && AS)
            AS.enabled = _App.SoundManager.IsSoundEnabled;
    }
}