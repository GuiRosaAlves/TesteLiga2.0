using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDFollow : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private float _minXPos, _maxXPos;
    [SerializeField] private float _smoothX;
    private float _velocity;
    
    private void Update()
    {
        if (!_player)
            return;
        
        var posX = Mathf.SmoothDamp(transform.position.x, _player.position.x, ref _velocity, _smoothX);
        transform.position = Vector3.right * posX + Vector3.up * transform.position.y + Vector3.forward * transform.position.z;
    }

    private void LateUpdate()
    {
        var posX = Mathf.Clamp(transform.position.x, _minXPos, _maxXPos);
        transform.position = Vector3.right * posX + Vector3.up * transform.position.y + Vector3.forward * transform.position.z;
    }
}