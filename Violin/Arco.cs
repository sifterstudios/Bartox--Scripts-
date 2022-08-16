using Bartox.Audio;
using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace Bartox.Violin
{
    public class Arco : MonoBehaviour
    {
        [SerializeField] ForceFieldController _arcoForceField;
        [SerializeField] float _forceFieldActivationTime = 0.6f;
        [SerializeField] float _forceFieldDeactivationTime = 1f;
        PARAMETER_ID _arcoToggleParameterID;
        public bool IsActive;

        void Start()
        {
            _arcoToggleParameterID = SFXManager.GetGlobalParameterID(FMODParamConstants.ArcoToggle);
        }


        public void ActivateArco()
        {
            if (IsActive) return;
            IsActive = true;
            RuntimeManager.StudioSystem.setParameterByID(_arcoToggleParameterID, 1);
            DOTween.To(() => _arcoForceField.openCloseProgress, x => _arcoForceField.openCloseProgress = x, 2,
                _forceFieldActivationTime).onComplete();
        }

        public void DeactivateArco()
        {
            IsActive = false;
            RuntimeManager.StudioSystem.setParameterByID(_arcoToggleParameterID, 0);
            DOTween.To(() => _arcoForceField.openCloseProgress, x => _arcoForceField.openCloseProgress = x, -2,
                _forceFieldDeactivationTime);
        }

        void Update()
        {
            print("Current ForceField Progress: " + _arcoForceField.openCloseProgress);
        }
    }
}