using Bartox.Bart.Controller;
using FMOD.Studio;
using FMODUnity;
using KinematicCharacterController;
using UnityEngine;

namespace Bartox.Audio
{
    public class BartLocomotionSFX : MonoBehaviour
    {
        [SerializeField] EventReference _locomotionEventReference;
        [SerializeField] EventReference _jumpEventReference;
        [SerializeField] EventReference _landEventReference;
        PARAMETER_ID _surfaceCollisionParameterID;
        PARAMETER_ID _verticalVelocityParameterID;
        PARAMETER_ID _movementSpeedParameterID;
        [SerializeField] KinematicCharacterMotor _motor;
        BartCharacterController _characterController;
        string _currentSurfaceCollision;
        [SerializeField] float _timeSinceLastFootstep;

        void Start()
        {
            _surfaceCollisionParameterID =
                SFXManager.GetLocalParameterID(_locomotionEventReference.Guid, FMODParamConstants.SurfaceCollision);
            _verticalVelocityParameterID =
                SFXManager.GetGlobalParameterID(FMODParamConstants.VerticalVelocity);
            _characterController = _motor.gameObject.GetComponent<BartCharacterController>();
        }


        // Called from animation events!!!
        void OnLandSFX()
        {
            SFXManager.PlaySoundWithLabeledParameter(_landEventReference, _surfaceCollisionParameterID,
                _currentSurfaceCollision, transform.position);
            print("Called OnLandSFX!");
        }

        // Called from animation events!!!
        void OnFootstepSFX()
        {
            if (_timeSinceLastFootstep <= .3f) return;


            if (!_motor.GroundingStatus.FoundAnyGround || _motor.BaseVelocity.magnitude < 0.1f) return;

            SFXManager.PlaySoundWithLabeledParameter(_locomotionEventReference, _surfaceCollisionParameterID,
                _currentSurfaceCollision, transform.position);
            _timeSinceLastFootstep = 0;
        }

        // Called from animation events!!!
        void OnJumpSFX()
        {
            SFXManager.PlaySoundWithLabeledParameter(_jumpEventReference, _surfaceCollisionParameterID,
                _currentSurfaceCollision, transform.position);
            print("Called OnJumpSFX");
        }


        void Update()
        {
            _timeSinceLastFootstep += Time.deltaTime;
            if (_motor.GroundingStatus.GroundCollider == null) return;

            RuntimeManager.StudioSystem.setParameterByID(_verticalVelocityParameterID,
                (_motor.BaseVelocity.y / _characterController._maxAirMoveSpeed), true);
            _currentSurfaceCollision = _motor.GroundingStatus.GroundCollider.tag;
        }
    }
}