using System.Collections.Generic;
using Bartox.Bart.Animation;
using Bartox.UI;
using Bartox.Violin;
using KinematicCharacterController;
using UnityEngine;
using Bartox.Bart.State;

// TODO: Implement push mechanic and call "Push_Enter" in animation controller
// TODO: Add an "Interacting" character state
// TODO: Add the ability to grab an Item
// TODO: Add lever functionality
// TODO: Add ability to open and close doors
// TODO: Add possibility to press button on wall
// TODO: Add HitCollision Mechanic
// TODO: Add Health Component
namespace Bartox.Bart.Controller
{
    public enum OrientationMethod
    {
        TowardsCamera,
        TowardsMovement
    }

    public struct PlayerCharacterInputs
    {
        public float MoveVertical;
        public float MoveHorizontal;
        public bool JumpDown;
        public bool CrouchDown;
        public bool PizzDown;
        public bool PizzUp;
        public bool AimDown;
        public bool AimUp;
        public bool ArcoDown;
        public bool InteractDown;
    }
    

    public enum BonusOrientationMethod
    {
        None,
        InvertGravity,
        TowardsGroundSlopeAndGravity
    }

    public class BartCharacterController : MonoBehaviour, ICharacterController
    {
        public KinematicCharacterMotor Motor;

        [Header("Stable Movement")] public float _maxStableMoveSpeed = 10f;

        [SerializeField] float _stableMovementSharpness = 15f;
        [SerializeField] float _orientationSharpness = 10f;
        [SerializeField] OrientationMethod _orientationMethod = OrientationMethod.TowardsCamera;

        [Header("Air Movement")] public float _maxAirMoveSpeed = 15f;

        [SerializeField] float _airAccelerationSpeed = 15f;
        [SerializeField] float _drag = 0.1f;

        [Header("Jumping")] public bool _allowJumpingWhenSliding;

        [SerializeField] float _jumpUpSpeed = 10f;
        [SerializeField] float _jumpScalableForwardSpeed = 10f;
        [SerializeField] float _jumpPreGroundingGraceTime;
        [SerializeField] float _jumpPostGroundingGraceTime;

        [Header("Misc")] public List<Collider> _ignoredColliders = new();

        [SerializeField] float _bonusOrientationSharpness = 10f;
        [SerializeField] Vector3 _gravity = new(0, -30f, 0);
        [SerializeField] Transform _meshRoot;
        [SerializeField] float _crouchedCapsuleHeight = 1f;
        [HideInInspector] public AnimationHandler AnimationHandler;
        [SerializeField] PizzWaveSpawner _pizzWaveSpawner;
        [SerializeField] float _checkForGroundDistance = 2;

        readonly Collider[] _probedColliders = new Collider[8];
        Camera _camera;
        Vector3 _internalVelocityAdd = Vector3.zero;
        bool _isCrouching;
        bool _isPizzing;
        bool _jumpConsumed;
        bool _jumpedThisFrame;
        bool _jumpRequested;


        Vector3 _lookInputVector;

        Vector3 _moveInputVectorPlusCamera;


        bool _shouldBeCrouching;

        bool _shouldBePizzing;

        float _timeSinceJumpRequested = Mathf.Infinity;

        float _timeSinceLastAbleToJump;

        Quaternion _tmpTransientRot;
        Vector3 _cameraPlanarDirection;
        Quaternion _cameraPlanarRotation;
        Arco _arco;


        void Awake()
        {
            // Handle initial state
            StateManager.Singleton.TransitionToState(States.Locomotion);

            // Assign the characterController to the motor
            Motor.CharacterController = this;
        }

        void Start()
        {
            _camera = Camera.main;
            AnimationHandler = _meshRoot.GetComponent<AnimationHandler>();
            _arco = GetComponentInChildren<Arco>();
        }

        #region Kinematic Character Controller (Engine Flow)

        public void BeforeCharacterUpdate(float deltaTime)
        {
        }


        public void AfterCharacterUpdate(float deltaTime)
        {
            switch (StateManager.Singleton.Current)
            {
                case States.Locomotion:
                {
                    // Handle jump-related values
                    {
                        // Handle jumping pre-ground grace period
                        if (_jumpRequested && _timeSinceJumpRequested > _jumpPreGroundingGraceTime)
                            _jumpRequested = false;

                        if (_allowJumpingWhenSliding
                                ? Motor.GroundingStatus.FoundAnyGround
                                : Motor.GroundingStatus.IsStableOnGround)
                        {
                            // If we're on a ground surface, reset jumping values
                            if (!_jumpedThisFrame) _jumpConsumed = false;
                            _timeSinceLastAbleToJump = 0f;
                        }
                        else
                        {
                            // Keep track of time since we were last able to jump (for grace period)
                            _timeSinceLastAbleToJump += deltaTime;
                        }
                    }

                    // Handle uncrouching
                    if (_isCrouching && !_shouldBeCrouching) HandleUncrouching();

                    break;
                }
            }
        }


        public void PostGroundingUpdate(float deltaTime)
        {
            // Handle landing and leaving ground
            if (Motor.GroundingStatus.IsStableOnGround && !Motor.LastGroundingStatus.IsStableOnGround)
                OnLanded();
            else if (!Motor.GroundingStatus.IsStableOnGround && Motor.LastGroundingStatus.IsStableOnGround)
                OnLeaveStableGround();
        }

        #endregion

        #region Kinematic Character Controller (Rotation)

        public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            switch (StateManager.Singleton.Current)
            {
                case States.Locomotion:
                {
                    if (IsLookInputMoving())
                    {
                        var smoothedLookInputDirection = SlerpToNewLookDirection(deltaTime);

                        currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, Motor.CharacterUp);
                    }

                    var currentUp = currentRotation * Vector3.up;

                    HandleGravity(ref currentRotation, deltaTime, currentUp);

                    break;
                }
            }
        }


        void HandleGravity(ref Quaternion currentRotation, float deltaTime, Vector3 currentUp)
        {
            var smoothedGravityDir = Vector3.Slerp(currentUp, Vector3.up,
                1 - Mathf.Exp(-_bonusOrientationSharpness * deltaTime));
            currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) *
                              currentRotation;
        }

        Vector3 SlerpToNewLookDirection(float deltaTime)
        {
            var smoothedLookInputDirection = Vector3.Slerp(Motor.CharacterForward,
                _cameraPlanarDirection,
                1 - Mathf.Exp(-_orientationSharpness * deltaTime)).normalized;
            return smoothedLookInputDirection;
        }

        #endregion

        #region Kinematic Character Controller (Velocity)

        public void AddVelocity(Vector3 velocity)
        {
            switch (StateManager.Singleton.Current)
            {
                case States.Locomotion:
                {
                    _internalVelocityAdd += velocity;
                    break;
                }
            }
        }

        void HandleAirMovement(ref Vector3 currentVelocity, float deltaTime)
        {
            var addedVelocity = _moveInputVectorPlusCamera * (_airAccelerationSpeed * deltaTime);

            var currentVelocityOnInputsPlane =
                Vector3.ProjectOnPlane(currentVelocity, Motor.CharacterUp);

            // Limit air velocity from inputs
            if (currentVelocityOnInputsPlane.magnitude < _maxAirMoveSpeed)
            {
                // clamp addedVel to make total vel not exceed max vel on inputs plane
                var newTotal = Vector3.ClampMagnitude(currentVelocityOnInputsPlane + addedVelocity,
                    _maxAirMoveSpeed);
                addedVelocity = newTotal - currentVelocityOnInputsPlane;
            }
            else
            {
                // Make sure added vel doesn't go in the direction of the already-exceeding velocity
                if (Vector3.Dot(currentVelocityOnInputsPlane, addedVelocity) > 0f)
                    addedVelocity = Vector3.ProjectOnPlane(addedVelocity,
                        currentVelocityOnInputsPlane.normalized);
            }

            // Prevent air-climbing sloped walls
            if (Motor.GroundingStatus.FoundAnyGround)
                if (Vector3.Dot(currentVelocity + addedVelocity, addedVelocity) > 0f)
                {
                    var perpendicularObstructionNormal = Vector3
                        .Cross(Vector3.Cross(Motor.CharacterUp, Motor.GroundingStatus.GroundNormal),
                            Motor.CharacterUp).normalized;
                    addedVelocity =
                        Vector3.ProjectOnPlane(addedVelocity, perpendicularObstructionNormal);
                }

            // Apply added velocity
            currentVelocity += addedVelocity;
        }

        bool AllowedToJump()
        {
            // You're allowed to jump if you haven't jumped this frame.
            // And if slide jumping is allowed, you can jump then.
            // OR if the time since we last jumped is higher than the threshold we've decided on.
            return !_jumpConsumed &&
                   ((_allowJumpingWhenSliding
                        ? Motor.GroundingStatus.FoundAnyGround
                        : Motor.GroundingStatus.IsStableOnGround) ||
                    _timeSinceLastAbleToJump <= _jumpPostGroundingGraceTime);
        }

        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            switch (StateManager.Singleton.Current)
            {
                case States.Locomotion:
                {
                    // Ground movement
                    if (Motor.GroundingStatus.IsStableOnGround)
                    {
                        var currentVelocityMagnitude = currentVelocity.magnitude;

                        var effectiveGroundNormal = Motor.GroundingStatus.GroundNormal;

                        // Reorient velocity on slope
                        currentVelocity = Motor.GetDirectionTangentToSurface(currentVelocity, effectiveGroundNormal) *
                                          currentVelocityMagnitude;

                        // _moveInputVectorPlusCamera.z = 0;
                        // Calculate target velocity
                        var inputRight =
                            Vector3.Cross(_moveInputVectorPlusCamera,
                                Motor.CharacterUp); // Cross Product from moveinput and character up
                        var reorientedInput = Vector3.Cross(effectiveGroundNormal, inputRight).normalized *
                                              _moveInputVectorPlusCamera
                                                  .magnitude; // Cross Product from the ground and moveinput
                        var targetMovementVelocity = reorientedInput * _maxStableMoveSpeed; // Direction * speed

                        // Smooth movement Velocity
                        currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity,
                            1f - Mathf.Exp(-_stableMovementSharpness * deltaTime)); // Accelerate
                    }
                    // Air movement
                    else
                    {
                        if (_moveInputVectorPlusCamera.sqrMagnitude > 0f)
                            HandleAirMovement(ref currentVelocity, deltaTime);

                        // Gravity
                        currentVelocity += _gravity * deltaTime;

                        // Drag
                        currentVelocity *= 1f / (1f + _drag * deltaTime);

                        // Check for landing
                        if (AreWeLanding())
                        {
                            AnimationHandler.Anim.SetBool(AnimationHandler.IsFalling, false);
                        }
                    }

                    // Handle jumping
                    HandleJumping(ref currentVelocity, deltaTime);

                    // Take into account additive velocity
                    if (_internalVelocityAdd.sqrMagnitude > 0f)
                    {
                        currentVelocity += _internalVelocityAdd;
                        _internalVelocityAdd = Vector3.zero;
                    }

                    break;
                }
            }
        }

        void HandleJumping(ref Vector3 currentVelocity, float deltaTime)
        {
            _jumpedThisFrame = false;
            _timeSinceJumpRequested += deltaTime;
            if (_jumpRequested)
                // See if we actually are allowed to jump
                if (AllowedToJump())
                    ExecuteJump(ref currentVelocity);
        }

        bool AreWeLanding()
        {
            if (Motor.Velocity.y >= 0)
            {
                return false;
            }

            return Physics.Raycast(transform.position, -Motor.CharacterUp,
                _checkForGroundDistance, 3);
        }

        void ExecuteJump(ref Vector3 currentVelocity)
        {
            // Calculate jump direction before ungrounding
            var jumpDirection = Motor.CharacterUp;
            if (Motor.GroundingStatus.FoundAnyGround && !Motor.GroundingStatus.IsStableOnGround)
                jumpDirection = Motor.GroundingStatus.GroundNormal;

            // Makes the character skip ground probing/snapping on its next update. 
            // If this line weren't here, the character would remain snapped to the ground when trying to jump. Try commenting this line out and see.
            Motor.ForceUnground();

            // Add to the return velocity and reset jump state
            currentVelocity += jumpDirection * _jumpUpSpeed -
                               Vector3.Project(currentVelocity, Motor.CharacterUp);
            currentVelocity += _moveInputVectorPlusCamera * _jumpScalableForwardSpeed;
            AnimationHandler.PlayJump();
            _jumpRequested = false;
            _jumpConsumed = true;
            _jumpedThisFrame = true;
        }

        #endregion
        

        #region Input

        void HandleCrouchingInput(PlayerCharacterInputs inputs)
        {
            if (inputs.CrouchDown)
            {
                _shouldBeCrouching = true;

                if (!_isCrouching) HandleCrouching();
            }
            else
            {
                _shouldBeCrouching = false;
            }
        }

        void HandleJumpingInput(PlayerCharacterInputs inputs)
        {
            if (inputs.JumpDown)
            {
                _timeSinceJumpRequested = 0f;
                _jumpRequested = true;
            }
        }



        bool IsLookInputMoving()
        {
            return _lookInputVector.sqrMagnitude > 0f && _orientationSharpness > 0f;
        }

        public void SetInputs(ref PlayerCharacterInputs inputs)
        {
            var clampedMoveInputVector = ClampMoveInput(inputs);

            // Calculate camera direction and rotation on the character plane
            _cameraPlanarDirection = CalculateCameraPlanarDirection(_camera.transform.rotation);
            _cameraPlanarRotation = CalculateCameraPlanarRotation(_cameraPlanarDirection);

            switch (StateManager.Singleton.Current)
            {
                case States.Locomotion:
                {
                    HandleMoveInput(_cameraPlanarRotation, clampedMoveInputVector);
                    HandleLookInput(_cameraPlanarDirection);
                    HandleJumpingInput(inputs);
                    HandleCrouchingInput(inputs);
                    HandlePizzInput(inputs);
                    HandleAimInput(inputs);
                    HandleArcoInput(inputs);
                    break;
                    // TODO: Add in other states here and refactor the above to match!
                }
            }

            if (Motor.BaseVelocity.magnitude > 0)
            {
                AnimationHandler.UpdateAnimatorMovementValues(inputs.MoveVertical, inputs.MoveHorizontal);
            }
        }

        void HandleArcoInput(PlayerCharacterInputs inputs)
        {
            if (inputs.ArcoDown && !_arco.IsActive)
            {
                _arco.ActivateArco();
            }
            else if (!inputs.ArcoDown && _arco.IsActive)
            {
                _arco.DeactivateArco();
            }
        }

        void HandleAimInput(PlayerCharacterInputs inputs)
        {
            if (inputs.AimDown)
            {
                _camera.GetComponent<BartCamera>().ActivateAimCam();
                UIManager.Singleton.ActivateAim();
            }

            if (inputs.AimUp)
            {
                _camera.GetComponent<BartCamera>().ActivateMainFreeLookCam();
                UIManager.Singleton.ActivateNormal();
            }
        }

        Quaternion CalculateCameraPlanarRotation(Vector3 cameraPlanarDirection)
        {
            var cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, Motor.CharacterUp);
            return cameraPlanarRotation;
        }

        Vector3 CalculateCameraPlanarDirection(Quaternion cameraRotation)
        {
            var cameraPlanarDirection =
                Vector3.ProjectOnPlane(_camera.transform.rotation * Vector3.forward, Motor.CharacterUp).normalized;
            if (cameraPlanarDirection.sqrMagnitude == 0f)
                cameraPlanarDirection = Vector3.ProjectOnPlane(cameraRotation * Vector3.up, Motor.CharacterUp)
                    .normalized;
            return cameraPlanarDirection;
        }

        static Vector3 ClampMoveInput(PlayerCharacterInputs inputs)
        {
            var moveInputVector =
                Vector3.ClampMagnitude(new Vector3(inputs.MoveHorizontal, 0f, inputs.MoveVertical), 1f);
            moveInputVector.z = Mathf.Clamp(moveInputVector.z, -0.5f, 1f);
            return moveInputVector;
        }

        void HandleLookInput(Vector3 cameraPlanarDirection)
        {
            _lookInputVector = _orientationMethod switch
            {
                OrientationMethod.TowardsCamera => cameraPlanarDirection,
                OrientationMethod.TowardsMovement => _moveInputVectorPlusCamera.normalized,
                _ => _lookInputVector
            };
        }

        void HandleMoveInput(Quaternion cameraPlanarRotation, Vector3 moveInputVector)
        {
            _moveInputVectorPlusCamera = cameraPlanarRotation * moveInputVector;
        }

        void HandlePizzInput(PlayerCharacterInputs inputs)
        {
            if (inputs.PizzDown) Pizz();

            if (inputs.PizzUp) UnPizz();
        }

        #endregion

        #region Behaviour

        void HandleCrouching()
        {
            _isCrouching = true;
            Motor.SetCapsuleDimensions(0.5f, _crouchedCapsuleHeight, _crouchedCapsuleHeight * 0.5f);
            AnimationHandler.Anim.SetBool(AnimationHandler.IsCrouching, true);
        }

        protected void OnLanded()
        {
            AnimationHandler.Anim.SetBool(AnimationHandler.IsFalling, false);
            AnimationHandler.Anim.SetBool(AnimationHandler.IsJumping, false);
        }

        protected void OnLeaveStableGround()
        {
        }

        void Pizz()
        {
            if (_isPizzing) return;
            _isPizzing = true;
            _pizzWaveSpawner.Spawn();
        }


        void UnPizz()
        {
            _isPizzing = false;
        }

        void HandleUncrouching()
        {
            // Do an overlap test with the character's standing height to see if there are any obstructions
            Motor.SetCapsuleDimensions(0.5f, 2f, 1f);
            if (Motor.CharacterOverlap(
                    Motor.TransientPosition,
                    Motor.TransientRotation,
                    _probedColliders,
                    Motor.CollidableLayers,
                    QueryTriggerInteraction.Ignore) > 0)
            {
                // If obstructions, just stick to crouching dimensions
                Motor.SetCapsuleDimensions(0.5f, _crouchedCapsuleHeight, _crouchedCapsuleHeight * 0.5f);
            }
            else
            {
                // If no obstructions, uncrouch
                AnimationHandler.Anim.SetBool(AnimationHandler.IsCrouching, false);
                _isCrouching = false;
            }
        }

        #endregion

        #region Kinematic Character Controller (Engine, Not Used Yet)

        public bool IsColliderValidForCollisions(Collider coll)
        {
            if (_ignoredColliders.Count == 0) return true;

            if (_ignoredColliders.Contains(coll)) return false;

            return true;
        }


        public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint,
            ref HitStabilityReport hitStabilityReport)
        {
        }

        public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint,
            ref HitStabilityReport hitStabilityReport)
        {
        }

        public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint,
            Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
        {
        }

        public void OnDiscreteCollisionDetected(Collider hitCollider)
        {
        }

        #endregion
    }
}