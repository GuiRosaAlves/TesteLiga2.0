using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _cameraBounds;

    private Vector2 _size;
    private Vector2 _offset;

    private float _left;
    private float _right;
    private float _top;
    private float _bottom;

    [SerializeField] private Transform target;
    [SerializeField] private float _smoothTimeY;
    [SerializeField] private float _smoothTimeX;
    private Vector2 velocity;

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
            float posX = Mathf.SmoothDamp(transform.position.x, target.position.x, ref velocity.x, _smoothTimeX);
            float posY = Mathf.SmoothDamp(transform.position.y, target.position.y, ref velocity.y, _smoothTimeY);
            
            transform.position = (Vector3.right * posX) + (Vector3.up * posY) + (Vector3.forward * transform.position.z);
        }
    }

    protected void LateUpdate()
    {
        Vector3 v3 = transform.position;
        v3.x = Mathf.Clamp(v3.x, _left, _right);
        v3.y = Mathf.Clamp(v3.y, _bottom, _top);
        transform.position = v3;
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