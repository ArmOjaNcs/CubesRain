using UnityEngine;

public class MaterialHolder : MonoBehaviour
{
    [SerializeField] private Material[] _materials;
    [SerializeField] private Material _defaultMaterial;

    public Material DefaultMaterial => _defaultMaterial;

    public Material GetMaterial()
    {
        return _materials[Random.Range(0, _materials.Length)];
    }
}
