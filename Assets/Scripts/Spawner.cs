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

    private float _minXPos => _ground.position.x - _ground.localScale.x * 5 + _offset;
    private float _maxXPos => _minXPos + _ground.localScale.x * 10 - _offset * 2;
    private float _yPos => _ground.position.y + _height;
    private float _minZPos => _ground.position.z - _ground.localScale.z * 5 + _offset;
    private float _maxZPos => _minZPos + _ground.localScale.z * 10 - _offset * 2;

    private void Awake()
    {
        _pool = new ObjectPool<DropDownCube>(
        createFunc: () => CreateCube(),
        actionOnGet: (cube) => ActionOnGet(cube),
        actionOnRelease: (cube) => cube.gameObject.SetActive(false),
        actionOnDestroy: (cube) => ActionOnDestroy(cube),
        collectionCheck: true,
        defaultCapacity: _poolCapacity,
        maxSize: _poolMaxSize);
    }

    private void Start()
    {
        InvokeRepeating(nameof(GetCube), 0, _repeatRate);
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
        StartCoroutine(CubeLifeTime(cube));
    }

    private void ActionOnGet(DropDownCube cube)
    {
        cube.transform.position = GetSpawnPosition();
        cube.SetMaterial(_materialHolder.DefaultMaterial);
        cube.GetComponent<Rigidbody>().velocity = Vector3.zero;
        cube.gameObject.SetActive(true);
    }

    private void ActionOnDestroy(DropDownCube cube)
    {
        cube.FallenDown -= OnFallenDown;
        Destroy(cube.gameObject);
    }

    private Vector3 GetSpawnPosition()
    {
        float xPos = Random.Range(_minXPos, _maxXPos);
        float zPos = Random.Range(_minZPos, _maxZPos);

        return new Vector3(xPos, _yPos, zPos);
    }

    private IEnumerator CubeLifeTime(DropDownCube cube)
    {
        float timeToWait = Random.Range(_minCubeLifeTime, _maxCubeLifeTime + 1);
        yield return new WaitForSeconds(timeToWait);
        _pool.Release(cube);
    }
}
