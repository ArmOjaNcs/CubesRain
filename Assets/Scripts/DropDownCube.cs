using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MeshRenderer))]
public class DropDownCube : MonoBehaviour
{
    private bool _isFirstCollision;
    private MeshRenderer _renderer;
    private Rigidbody _rigidbody;

    public event Action<DropDownCube> FallenDown;

    public Rigidbody Rigidbody => _rigidbody;

    private void OnEnable()
    {
        _isFirstCollision = true;
    }

    private void Awake()
    { 
        _renderer = GetComponent<MeshRenderer>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.TryGetComponent<Platform>(out Platform _) && _isFirstCollision)
        {
            _isFirstCollision = false;
            FallenDown?.Invoke(this);
        }
    }

    public void SetMaterial(Material material)
    {
        _renderer.material = material;
    }
}
