using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Configs : MenuPanel
{
    [Header("Configs")]
    [SerializeField] private Text _soundEnabledTxt;
    [SerializeField] private AudioSource _bgAs;

    void Start()
    {
        GetSoundManagerData();
    }

    public void GetSoundManagerData()
    {
        if (_App.SoundManager)
        {
            _soundEnabledTxt.text = _App.SoundManager.IsSoundEnabled ? "LIGADA" : "DESLIGADA";
            _bgAs.enabled = _App.SoundManager.IsSoundEnabled;
        }
    }

    public void TriggerAudioState()
    {
        if (_App.SoundManager)
        {
            _App.SoundManager.ChangeSoundState();
            GetSoundManagerData();
        }
        else
        {
            switch (_soundEnabledTxt.text)
            {
                case "LIGADA":
                    _soundEnabledTxt.text = "DESLIGADA";
                    _bgAs.enabled = true;
                    break;
                case "DESLIGADA":
                    _soundEnabledTxt.text = "LIGADA";
                    _bgAs.enabled = false;
                    break;
                default:
                    _soundEnabledTxt.text = "LIGADA";
                    _bgAs.enabled = true;
                    break;
            }
        }
    }
}