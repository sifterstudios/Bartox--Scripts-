using System;
using DG.Tweening;
using UnityEngine;

namespace Bartox.Interactables
{
    public class PizzPlateInteractionHandler : MonoBehaviour
    {
        bool _hasDoneActivation;
        bool _hasDoneDeactivation;
        Vector3 _originalPosition;

        [SerializeField] PizzPlate _pizzPlate;

        void Start()
        {
            _originalPosition = transform.position;
        }

        void Update()
        {
            if (_pizzPlate.IsEnabled)
            {
                if (_hasDoneActivation) return;
                transform.DOMove(transform.position + (Vector3.up * 3), .7f);
                _hasDoneActivation = true;
                _hasDoneDeactivation = false;

                print("Doing Activation");
            }

            else if (_pizzPlate.IsDisabled && _hasDoneActivation)
            {
                if (!_hasDoneDeactivation) return;
                transform.DOMove(_originalPosition, .7f);
                _hasDoneDeactivation = true;
                _hasDoneActivation = false;
                print("Doing Deactivation");
            }
        }
    }
}