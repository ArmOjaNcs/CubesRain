using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MeshRenderer))]
public class DropDownCube : MonoBehaviour
{
    public event Action<DropDownCube> FallenDown;

    private bool _isFirstCollision;
    private MeshRenderer _renderer;

    private void OnEnable()
    {
        _isFirstCollision = true;
    }

    private void Awake()
    { 
        _renderer = GetComponent<MeshRenderer>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Ground" && _isFirstCollision)
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
