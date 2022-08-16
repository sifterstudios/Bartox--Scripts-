using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;

namespace Bartox.Violin
{
    public class SparksSpawner : MonoBehaviour
    {
        public static SparksSpawner Singleton;
        [SerializeField] Sparks _sparksPrefab;

        Camera _camera;

        // ObjectPool<Sparks> _pool;
        Vector3 _spawnTransform;

        void Awake()
        {
            if (Singleton == null) Singleton = this;
            else if (Singleton != this) Destroy(this);
        }

        void Start()
        {
            _camera = Camera.main;
            // _pool = new ObjectPool<Sparks>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool,
            // OnDestroyPoolObject, true, 400);
        }

        void OnDestroyPoolObject(Sparks sparks)
        {
            // Destroy(sparks.gameObject);
        }

        void OnReturnedToPool(Sparks sparks)
        {
            // sparks.gameObject.SetActive(false);
        }

        void OnTakeFromPool(Sparks sparks)
        {
            // sparks.gameObject.transform.position = _spawnTransform;
            // sparks.gameObject.transform.LookAt(_camera.transform, Vector3.up);
            // sparks.gameObject.transform.parent = transform;
            // sparks.gameObject.SetActive(true);
        }

        // Sparks CreatePooledItem()
        // {
        //     // var ps = Instantiate(_sparksPrefab, _spawnTransform, quaternion.identity, transform);
        //     // ps.gameObject.SetActive(false);
        //     // return ps;
        // }

        public void Spawn(Vector3 position)
        {
            // _spawnTransform = position;
            // _pool.Get();
            var ps = Instantiate(_sparksPrefab, _spawnTransform, quaternion.identity, transform);
        }

        public void Stash(Sparks sparks)
        {
            // _pool.Release(sparks);
            Destroy(sparks.gameObject);
        }
    }
}