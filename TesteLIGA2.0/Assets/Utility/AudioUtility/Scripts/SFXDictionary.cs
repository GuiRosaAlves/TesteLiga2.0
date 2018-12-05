using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SFXDictionary
{
    [SerializeField] protected List<SFX> library;
    private AudioSource targetSource;
    public int Count { get { return library.Count; } }

    public SFXDictionary()
    {
        library = new List<SFX>();
        targetSource = null;
    }
    public SFXDictionary(AudioSource source)
    {
        library = new List<SFX>();
        targetSource = source;
    }

    public SFX Add()
    {
        SFX returnValue = new SFX();
        library.Add(returnValue);
        return returnValue;
    }

    public SFX Add(string tag, AudioClip audio)
    {
        SFX returnValue = new SFX(tag, audio);
        library.Add(returnValue);
        return returnValue;
    }

    public SFX Get(string tag)
    {
        foreach (SFX sfx in library)
        {
            if (sfx.tag == tag)
            {
                return sfx;
            }
        }
        Debug.Log("Sound Effect not found in the library!");
        return default(SFX);
    }

    public SFX Get(int index)
    {
        if (library.Count != 0 && index < library.Count)
        {
            return library[index];
        }
        else
        {
            Debug.Log("Sound Effect not found in the library!");
        }
        return default(SFX);
    }

    public void Remove(string tag)
    {
        for (int i = 0; i < library.Count; i++)
        {
            if (library[i].tag == tag)
            {
                library.RemoveAt(i);
                return;
            }
        }
        Debug.Log("Sound Effect not found in the library!");
    }

    public void Remove(int index)
    {
        if (library.Count != 0 && index < library.Count)
        {
            library.RemoveAt(index);
        }
        else
        {
            Debug.Log("Sound Effect not found in the library!");
        }
    }

    public bool Play(string tag, ulong delay)
    {
        foreach (SFX sfx in library)
        {
            if (sfx.tag == tag)
            {
                targetSource.clip = sfx.audio;
                targetSource.Play(delay);
                return true;
            }
        }
        Debug.Log("Sound Effect not found in the library!");
        return false;
    }

    public bool Play(AudioSource source, string tag, ulong delay)
    {
        foreach (SFX sfx in library)
        {
            if (sfx.tag == tag)
            {
                source.clip = sfx.audio;
                source.Play(delay);
                return true;
            }
        }
        Debug.Log("Sound Effect not found in the library!");
        return false;
    }

    public bool PlayOneShot(string tag)
    {
        foreach (SFX sfx in library)
        {
            if (sfx.tag == tag)
            {
                targetSource.PlayOneShot(sfx.audio);
                return true;
            }
        }
        Debug.Log("Sound Effect not found in the library!");
        return false;
    }

    public bool PlayOneShot(AudioSource source, string tag)
    {
        foreach (SFX sfx in library)
        {
            if (sfx.tag == tag)
            {
                source.PlayOneShot(sfx.audio);
                return true;
            }
        }
        Debug.Log("Sound Effect not found in the library!");
        return false;
    }

    public void Clear()
    {
        library.Clear();
    }
}