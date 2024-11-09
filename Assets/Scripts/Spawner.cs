using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class Spawner : MonoBehaviour
{
    [SerializeField] private DropDownCube _prefab;
    [SerializeField] private Transform _ground;
    [SerializeField] private float _repeatRate;
    [SerializeField] private int _poolCapacity;
    [SerializeField] private int _poolMaxSize;
    [SerializeField, Range(1, 5)] private float _offset;
    [SerializeField] private MaterialHolder _materialHolder;

    private readonly float _height = 60;
    private readonly int _minCubeLifeTime = 2;
    private readonly int _maxCubeLifeTime = 5;

    private ObjectPool<DropDownCube> _pool;

    private float MinXPosition => _ground.position.x - _ground.localScale.x * 5 + _offset;
    private float MaxXPosition => MinXPosition + _ground.localScale.x * 10 - _offset * 2;
    private float YPosition => _ground.position.y + _height;
    private float MinZPosition => _ground.position.z - _ground.localScale.z * 5 + _offset;
    private float MaxZPosition => MinZPosition + _ground.localScale.z * 10 - _offset * 2;

    private void Awake()
    {
        _pool = new ObjectPool<DropDownCube>(
        createFunc: () => CreateCube(),
        actionOnGet: (cube) => SetStartParameters(cube),
        actionOnRelease: (cube) => cube.gameObject.SetActive(false),
        actionOnDestroy: (cube) => DestroyObjectInPool(cube),
        collectionCheck: true,
        defaultCapacity: _poolCapacity,
        maxSize: _poolMaxSize);
    }

    private void Start()
    {
        StartCoroutine(BeginingCubesRain(_repeatRate));
    }

    private void GetCube()
    {
        _pool.Get();
    }

    private DropDownCube CreateCube()
    {
        DropDownCube createdCube = Instantiate(_prefab);
        createdCube.FallenDown += OnFallenDown;
        createdCube.gameObject.SetActive(false);
        return createdCube;
    }

    private void OnFallenDown(DropDownCube cube)
    {
        cube.SetMaterial(_materialHolder.GetMaterial());
        StartCoroutine(BeginCubeLifeTime(cube));
    }

    private void SetStartParameters(DropDownCube cube)
    {
        cube.transform.position = GetSpawnPosition();
        cube.SetMaterial(_materialHolder.DefaultMaterial);
        cube.Rigidbody.velocity = Vector3.zero;
        cube.gameObject.SetActive(true);
    }

    private void DestroyObjectInPool(DropDownCube cube)
    {
        cube.FallenDown -= OnFallenDown;
        Destroy(cube.gameObject);
    }

    private Vector3 GetSpawnPosition()
    {
        float xPosition = Random.Range(MinXPosition, MaxXPosition);
        float zPosition = Random.Range(MinZPosition, MaxZPosition);

        return new Vector3(xPosition, YPosition, zPosition);
    }

    private IEnumerator BeginCubeLifeTime(DropDownCube cube)
    {
        float timeToWait = Random.Range(_minCubeLifeTime, _maxCubeLifeTime + 1);
        yield return new WaitForSeconds(timeToWait);
        _pool.Release(cube);
    }

    private IEnumerator BeginingCubesRain(float repeatRate)
    {
        while(true)
        {
            GetCube();
            yield return new WaitForSeconds(repeatRate);
        }
    }
}
