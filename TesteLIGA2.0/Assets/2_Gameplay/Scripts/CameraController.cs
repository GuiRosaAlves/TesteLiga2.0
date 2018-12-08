using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraController : MonoBehaviour
{
    public static CameraController instance { get; private set; }
    
    [SerializeField] private Transform _cameraBounds;
    [SerializeField] private float _shakeIntensity = 0.24f;
    [SerializeField] private float _screenShakeDuration = 4.41f;
    
    private Vector2 _size;
    private Vector2 _offset;

    private float _left;
    private float _right;
    private float _top;
    private float _bottom;

    [SerializeField] private Transform target;
    [SerializeField] private float _smoothTimeY;
    [SerializeField] private float _smoothTimeX;
    private Vector2 _velocity;
    private float _shakeAmt;

    protected void Awake()
    {
        instance = this;
    }
    
    protected void Start()
    {
        Sprite sprite = _cameraBounds.transform.GetComponent<SpriteRenderer>().sprite;

        float pixelPerUnits = sprite.rect.width / sprite.bounds.size.x;

        CalculateSize(sprite, pixelPerUnits);
        RefreshBounds();
    }

    protected void FixedUpdate()
    {
        if (target != null)
        {
            float posX = Mathf.SmoothDamp(transform.position.x, target.position.x, ref _velocity.x, _smoothTimeX);
            float posY = Mathf.SmoothDamp(transform.position.y, target.position.y, ref _velocity.y, _smoothTimeY);
            
            transform.position = (Vector3.right * posX) + (Vector3.up * posY) + (Vector3.forward * transform.position.z);
        }
    }

    private void Update()
    {
        if(_shakeAmt > 0) 
        {
            float quakeAmt = Random.value*_shakeAmt*2 - _shakeAmt;
            Vector3 pp = transform.position;
            pp.x += quakeAmt;
            pp.y += quakeAmt;
            transform.position = pp;
            _shakeAmt -= Time.deltaTime * _screenShakeDuration;
        }
    }

    protected void LateUpdate()
    {
        Vector3 pos = transform.position;
        
        pos.x = Mathf.Clamp(pos.x, _left, _right);
        pos.y = Mathf.Clamp(pos.y, _bottom, _top);
        transform.position = pos;
    }
    
    public void ScreenShake()
    {
        _shakeAmt = _shakeIntensity;
    }

    protected void CalculateSize(Sprite sprite, float pixelPerUnits)
    {
        _size = new Vector2(_cameraBounds.transform.parent.localScale.x * sprite.texture.width / pixelPerUnits,
                            _cameraBounds.transform.parent.localScale.y * sprite.texture.height / pixelPerUnits);

        _offset = _cameraBounds.transform.position;
    }

    protected void RefreshBounds()
    {
        var vertExtent = Camera.main.orthographicSize;
        var horzExtent = vertExtent * Screen.width / Screen.height;

        _left = horzExtent - _size.x / 2.0f + _offset.x;
        _right = _size.x / 2.0f - horzExtent + _offset.x;
        _bottom = vertExtent - _size.y / 2.0f + _offset.y;
        _top = _size.y / 2.0f - vertExtent + _offset.y;
    }
}