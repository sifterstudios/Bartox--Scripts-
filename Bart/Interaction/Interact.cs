using UnityEngine;
using UnityEngine.InputSystem;
using PlayerInput = Bartox.Bart.Input.PlayerInput;

namespace Bartox.Player.Interaction
{
    public class Interact : MonoBehaviour
    {
        Bart_Input _bartInput;
        bool _insideInteractionTrigger;

        void Start()
        {
            _bartInput = FindObjectOfType<PlayerInput>().GetComponent<Bart_Input>();
            _bartInput.Player.Interact.started += OnInteractRequest;
        }

        void OnDisable()
        {
            _bartInput.Player.Interact.started -= OnInteractRequest;
        }

        void OnTriggerEnter(Collider other)
        {
            _insideInteractionTrigger = true;
        }

        void OnTriggerExit(Collider other)
        {
            _insideInteractionTrigger = false;
        }

        public virtual void OnInteractRequest(InputAction.CallbackContext obj)
        {
            if (_insideInteractionTrigger)
            {
                // DO SOMETHING
            }
        }
    }
}