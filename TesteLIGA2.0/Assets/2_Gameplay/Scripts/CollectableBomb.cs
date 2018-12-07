using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableBomb : MonoBehaviour
{
    [SerializeField] private SFXDictionary _sfx;
    [SerializeField] private Animator _animator;
    [SerializeField] private Rigidbody2D _rigidBody;
    [SerializeField] private int _bombsQtt;
    private Character _player;
    private bool _pickupTrigger = true;

    public int BombsQtt { get { return (_bombsQtt <= 0) ? 1 : _bombsQtt; } }

    public void OnTriggerStay2D(Collider2D coll)
    {
        if (_pickupTrigger && _player)
        {
            if (_player.PickupBomb(BombsQtt))
            {
                if (_App.SoundManager)
                    _App.SoundManager.Play(_sfx.Get("Collected").audio);
                _animator.SetTrigger("Destroy");
                Destroy(gameObject, .3f);
            }

            _pickupTrigger = false;
            _player = null;
        }
        else
        {
            _player = coll.GetComponent<Character>();
        }
    }
}