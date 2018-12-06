using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableBomb : MonoBehaviour
{
    [SerializeField] private SFXDictionary _sfx;
    [SerializeField] private Animator _animator;
    [SerializeField] private Rigidbody2D _rigidBody;
    [SerializeField] private int _bombsQtt;

    public int BombsQtt { get { return (_bombsQtt <= 0) ? 1 : _bombsQtt; } }

    public void OnTriggerEnter2D(Collider2D coll)
    {
        Character player = coll.GetComponent<Character>();
        if (player)
        {
            Debug.Log("asdasf");
            if (player.PickupBomb(BombsQtt))
            {
                if (_App.SoundManager)
                    _App.SoundManager.Play(_sfx.Get("Collected").audio);
//                _rigidBody.Sleep();
                _animator.SetTrigger("Destroy");
                Destroy(gameObject, .3f);
            }
        }
    }
}