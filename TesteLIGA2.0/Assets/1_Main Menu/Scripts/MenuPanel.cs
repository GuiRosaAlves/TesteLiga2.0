using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPanel : MonoBehaviour
{
    [Header("Menu Panel")]
    [SerializeField] private Animator[] _btnAnims;
//    private MenuController _controller;
    
    public Animator _anim { get; private set; }

    private void Awake()
    {
//        _controller = FindObjectOfType<MenuController>();
        _anim = GetComponent<Animator>();
    }

    public void ResetAnimState() //Pequeno Arranjo Técnico, #UnityBugs
    {
        for (int i = 0; i < _btnAnims.Length; i++)
        {
            _btnAnims[i].CrossFade("Normal", 0f);
            _btnAnims[i].Update(0f);
            _btnAnims[i].Update(0f);
            _btnAnims[i].Update(0f);
        }
    }
}