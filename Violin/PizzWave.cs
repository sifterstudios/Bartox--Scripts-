using Bartox.Audio;
using Cinemachine;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace Bartox.Violin
{
    public class PizzWave : MonoBehaviour
    {
        [SerializeField] float _speed = 4.5f;
        [SerializeField] int _maxReflections = 3;
        [SerializeField] float _minimumTimeBetweenSound = .2f;

        Camera _camera;
        int _currentSpentReflections;
        StudioEventEmitter _ePizzWave;
        CinemachineImpulseSource _cinemachineImpulseSource;

        Rigidbody _rb;
        float _timeSinceLastSound;
        PARAMETER_ID _pizzWaveVelocityParameterID;
        PARAMETER_ID _pizzWaveComplexityParameterID;
        [SerializeField] float _addDragForEveryCollision = .4f;
        [SerializeField] int _pizzWaveComplexitySetting;


        public Vector3 Target { get; set; }
        public bool Hit { get; set; }

        void Start()
        {
            _pizzWaveVelocityParameterID =
                SFXManager.GetGlobalParameterID(FMODParamConstants.PizzWaveVelocity);
            _pizzWaveComplexityParameterID =
                SFXManager.GetGlobalParameterID(FMODParamConstants.PizzWaveComplexity);
            _cinemachineImpulseSource = GetComponent<CinemachineImpulseSource>();
            _ePizzWave = GetComponent<StudioEventEmitter>();
            _camera = Camera.main;
            transform.LookAt(Target);
            _rb = GetComponent<Rigidbody>();
            _rb.velocity = transform.forward * _speed;
            _ePizzWave.Play();
            _cinemachineImpulseSource.GenerateImpulse(_rb.velocity);
            _ePizzWave.SetParameter(_pizzWaveComplexityParameterID, (float) _pizzWaveComplexitySetting);
        }

        void Update()
        {
            _timeSinceLastSound += Time.deltaTime;
            _ePizzWave.SetParameter(_pizzWaveVelocityParameterID, _rb.velocity.magnitude);
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                print("I hit the player!");
                return; // FOR NOW!
            }

            if (_timeSinceLastSound < _minimumTimeBetweenSound) return;

            if (_currentSpentReflections == _maxReflections) PizzWaveSpawner.Singleton.Stash(this);


            _ePizzWave.Play();
            _timeSinceLastSound = 0;
            _currentSpentReflections++;
            _rb.drag += _addDragForEveryCollision;
            SparksSpawner.Singleton.Spawn(transform.position);
        }
    }
}