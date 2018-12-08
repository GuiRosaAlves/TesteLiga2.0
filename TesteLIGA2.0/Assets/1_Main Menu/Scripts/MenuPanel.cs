using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPanel : MonoBehaviour
{
    [Header("Menu Panel")]
    [SerializeField] private Animator[] _btnAnims;
    
    public Animator Anim { get; private set; }

    private void Awake()
    {
        Anim = GetComponent<Animator>();
    }

    public void ResetAnimState() //Pequeno Arranjo Técnico, #UnityBugs
    {
        for (int i = 0; i < _btnAnims.Length; i++)
        {
            _btnAnims[i].Rebind();
            _btnAnims[i].Play("Normal");
        }
    }
}