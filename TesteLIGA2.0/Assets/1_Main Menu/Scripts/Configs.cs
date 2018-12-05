using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Configs : MenuPanel
{
    [Header("Configs")]
    [SerializeField] private Text _soundEnabledTxt;

    void Start()
    {
        GetSoundManagerData();
    }

    public void GetSoundManagerData()
    {
        if (_App.SoundManager)
            _soundEnabledTxt.text = _App.SoundManager.IsSoundEnabled ? "LIGADA" : "DESLIGADA";
    }

    public void TriggerAudioState()
    {
        if (_App.SoundManager)
            _App.SoundManager.ChangeSoundState();
        else
        {
            switch (_soundEnabledTxt.text)
            {
                case "LIGADA":
                    _soundEnabledTxt.text = "DESLIGADA";
                    break;
                case "DESLIGADA":
                    _soundEnabledTxt.text = "LIGADA";
                    break;
                default:
                    _soundEnabledTxt.text = "LIGADA";
                    break;
            }
        }

        GetSoundManagerData();
    }
}