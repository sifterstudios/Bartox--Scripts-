using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;

namespace Bartox.Violin
{
    public class PizzWaveSpawner : MonoBehaviour
    {
        public static PizzWaveSpawner Singleton;
        [SerializeField] PizzWave _pizzWavePrefab;
        [SerializeField] Transform _pwOriginPoint;
        readonly float _bulletHitMissDistance = 100f;
        Camera _camera;
        ObjectPool<PizzWave> _pool;
        Transform _spawnPoint;

        void Awake()
        {
            if (Singleton == null)
                Singleton = this;
            else if (Singleton != this) Destroy(this);
        }

        void Start()
        {
            _camera = Camera.main;
        }


        public void Spawn()
        {
            var pw = Instantiate(_pizzWavePrefab, _pwOriginPoint.position, quaternion.identity, gameObject.transform);
            pw.gameObject.SetActive(false);

            if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out var hit))
            {
                pw.Target = hit.point;
                pw.Hit = true;
            }
            else
            {
                pw.Target = _camera.transform.position + _camera.transform.forward * _bulletHitMissDistance;
                pw.Hit = false;
            }

            pw.gameObject.transform.position = _pwOriginPoint.position;
            pw.gameObject.SetActive(true);
        }


        public void Stash(PizzWave pizzWave)
        {
            // _pool.Release(pizzWave);
            print("HAHA, WE DONT HAVE ANY POOL, DESTROYING NOW");
            Destroy(pizzWave.gameObject);
        }
    }
}