using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioSource _audioSource;
    public bool SoundState
    {
        get
        {
            if (PlayerPrefs.HasKey("sound"))
            {
                return (PlayerPrefs.GetInt("sound") == 0) ? false : true;
            }
            return true;
        }
        private set
        {
            PlayerPrefs.SetInt("sound", value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    protected void Awake()
    {
        instance = this;
    }

    protected void Update()
    {
        _audioSource.enabled = SoundState;
    }

    public void Play(AudioClip clip)
    {
        if (SoundState)
        {
            if (_audioSource)
            {
                _audioSource.PlayOneShot(clip);
            }
        }
    }

    public void ChangeSoundState()
    {
        SoundState = !SoundState;
    }
}