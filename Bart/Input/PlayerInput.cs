using Bartox.Bart.Controller;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Bartox.Bart.Input
{
    public class PlayerInput : MonoBehaviour
    {
        const string MouseXInput = "Mouse X";
        const string MouseYInput = "Mouse Y";
        const string MouseScrollInput = "Mouse ScrollWheel";
        const string HorizontalInput = "Horizontal";
        const string VerticalInput = "Vertical";
        [SerializeField] BartCamera _bartCamera;
        public BartCharacterController Character;
        float _currentZoomSetting;
        bool _inputAim;
        bool _inputCrouch;
        bool _inputJump;
        bool _inputPizz;
        bool _inputArco;
        InputAction _jump;
        InputAction _look;
        Vector2 _lookInputVector;
        InputAction _move;
        Vector2 _moveInputVector;
        InputAction _pizz;

        Bart_Input BartInput; // Csharp event file

        void Awake()
        {
            BartInput = new Bart_Input();
            BartInput.Enable();
        }

        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            BartInput.Enable();

            BartInput.Player.Move.performed += i => _moveInputVector = i.ReadValue<Vector2>();
            BartInput.Player.Move.canceled += _ => _moveInputVector = Vector2.zero;

            BartInput.Player.Pizz.started += _ => _inputPizz = true;
            BartInput.Player.Pizz.canceled += _ => _inputPizz = false;

            BartInput.Player.Aim.started += _ => _inputAim = true;
            BartInput.Player.Aim.canceled += _ => _inputAim = false;

            BartInput.Player.Look.performed += i => _lookInputVector = i.ReadValue<Vector2>();
            BartInput.Player.Look.canceled += _ => _lookInputVector = Vector2.zero;
        }

        void Update()
        {
            HandleCharacterInput();
        }

        void LateUpdate()
        {
            HandleCameraInput();
        }

        void OnEnable()
        {
            BartInput.Player.Enable();
        }

        void OnDisable()
        {
            BartInput.Player.Disable();
        }

        void HandleCameraInput()
        {
        }

        void HandleCharacterInput()
        {
            var characterInputs = new PlayerCharacterInputs
            {
                // Build the CharacterInputs struct
                MoveVertical = _moveInputVector.y,
                MoveHorizontal = _moveInputVector.x,
                JumpDown = BartInput.Player.Jump.WasPressedThisFrame(),
                CrouchDown = BartInput.Player.Crouch.IsPressed(),
                PizzDown = BartInput.Player.Pizz.WasPressedThisFrame(),
                PizzUp = BartInput.Player.Pizz.WasReleasedThisFrame(),
                AimDown = BartInput.Player.Aim.WasPressedThisFrame(),
                AimUp = BartInput.Player.Aim.WasReleasedThisFrame(),
                InteractDown = BartInput.Player.Interact.WasPressedThisFrame(),
                ArcoDown = BartInput.Player.Arco.IsPressed(),
            };

            // Apply inputs to character
            Character.SetInputs(ref characterInputs);
        }

        void OnDestroy()
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}