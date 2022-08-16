using UnityEngine;

namespace Bartox.Interactables
{
    public class PizzPlate : MonoBehaviour
    {
        [SerializeField] GameObject _greenParticleSystem;
        [SerializeField] GameObject _redParticleSystem;
        [SerializeField] ParticleSystem _impactParticleSystem;
        [Range(0, 15)] [SerializeField] float _deactivationTime = 3f;
        [SerializeField] bool _usesTimer;

        float _currentDeactivationTime;


        public bool IsEnabled;
        public bool IsDisabled;


        void Start()
        {
            _greenParticleSystem.SetActive(false);
            _redParticleSystem.SetActive(true);
        }


        void OnCollisionEnter(Collision collision)
        {
            if (!collision.gameObject.CompareTag("PizzWave")) return;
            ActivateSparks(collision);
            if (IsDisabled) Activate();


            else Deactivate();
        }

        void ActivateSparks(Collision collision)
        {
            _impactParticleSystem.gameObject.SetActive(false);
            _impactParticleSystem.gameObject.transform.LookAt(collision.gameObject.transform);
            _impactParticleSystem.gameObject.SetActive(true);
            _impactParticleSystem.Clear(true);
            _impactParticleSystem.Play(true);
        }

        void Activate()
        {
            _greenParticleSystem.SetActive(true);
            _redParticleSystem.SetActive(false);
            _currentDeactivationTime = _deactivationTime;
            // TODO: Add activation sound
            IsEnabled = true;
            IsDisabled = false;
        }

        void Deactivate()
        {
            _greenParticleSystem.SetActive(false);
            _redParticleSystem.SetActive(true);
            // TODO: Add deactivation sound
            IsEnabled = false;
            IsDisabled = true;
        }

        void Update()
        {
            if (!_usesTimer) return;
            _currentDeactivationTime -= Time.deltaTime;
            if (_currentDeactivationTime <= 0) Deactivate();
        }
    }
}