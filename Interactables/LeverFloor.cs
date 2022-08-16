using Bartox.Audio;
using UnityEngine;

namespace Bartox
{
    public class LeverFloor : MonoBehaviour, IAmInteractable
    {
        [SerializeField] bool _activated;

        Color _color;
        bool _isinProximity;
        Light _light;

        void Awake()
        {
            _light = GetComponentInChildren<Light>();
        }

        void Start()
        {
            _light.color = _activated ? Color.green : Color.red;
        }

        public void Interact()
        {

        }

        public void PlaySound(FMODEvent fEvent)
        {

        }
    }
}