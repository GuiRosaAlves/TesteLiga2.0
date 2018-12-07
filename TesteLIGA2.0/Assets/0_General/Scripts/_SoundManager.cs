using UnityEngine;

public class _SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource _aS;


    private void Awake()
    {
        if (!PlayerPrefs.HasKey("sound"))
            IsSoundEnabled = true;
    }

    public bool IsSoundEnabled
    {
        get
        {
            if (PlayerPrefs.HasKey("sound"))
                return (PlayerPrefs.GetInt("sound") > 0);

            return true;
        }
        private set
        {
            if (_aS)
                _aS.enabled = value;

            PlayerPrefs.SetInt("sound", value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public void Play(AudioClip clip)
    {
        if (IsSoundEnabled)
        {
            if (_aS)
            {
                _aS.PlayOneShot(clip);
            }
        }
    }

    public void ChangeSoundState()
    {
        IsSoundEnabled = !IsSoundEnabled;
    }
}