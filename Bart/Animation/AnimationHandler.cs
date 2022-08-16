using UnityEngine;

namespace Bartox.Bart.Animation
{
    public class AnimationHandler : MonoBehaviour
    {
        public static int IsPushing;
        public static int IsJumping;
        public static int IsFalling;
        public static int IsDead;
        public static int IsCrouching;
        public Animator Anim;
        public bool CanRotate;
        int _horizontal;
        int _vertical;
        [SerializeField] float _deadZoneValue = 0.2f;


        void Awake()
        {
            IsPushing = Animator.StringToHash("isPushing");
            IsJumping = Animator.StringToHash("isJumping");
            IsFalling = Animator.StringToHash("isFalling");
            IsDead = Animator.StringToHash("isDead");
            IsCrouching = Animator.StringToHash("isCrouching");
        }

        public void Initialize()
        {
            Anim = GetComponent<Animator>();
            _vertical = Animator.StringToHash("vertical");
            _horizontal = Animator.StringToHash("horizontal");
            Anim.Play("Locomotion", 0);
        }

        public void UpdateAnimatorMovementValues(float verticalMovement, float horizontalMovement)
        {
            // if (verticalMovement < _deadZoneValue && verticalMovement > -_deadZoneValue) verticalMovement = 0;
            // if (horizontalMovement < _deadZoneValue && horizontalMovement > -_deadZoneValue) horizontalMovement = 0;

            Anim.SetFloat(_vertical, verticalMovement, 0.1f, Time.deltaTime);
            Anim.SetFloat(_horizontal, horizontalMovement, 0.1f, Time.deltaTime);
        }

        public void MakeCanRotate()
        {
            CanRotate = true;
        }

        public void StopRotation()
        {
            CanRotate = false;
        }

        public void PlayGrab()
        {
            if (!Anim.GetCurrentAnimatorStateInfo(1).IsName("Grab")) Anim.Play("Grab", 1);
        }

        public void PlayPushLeverWall()
        {
            if (!Anim.GetCurrentAnimatorStateInfo(1).IsName("PushLeverWall")) Anim.Play("PushLeverWall", 1);
        }

        public void PlayPullLeverWall()
        {
            if (!Anim.GetCurrentAnimatorStateInfo(1).IsName("PullLeverWall")) Anim.Play("PullLeverWall", 1);
        }

        public void PlayPushLeverGround()
        {
            if (!Anim.GetCurrentAnimatorStateInfo(1).IsName("PushLeverGround")) Anim.Play("PushLeverGround", 1);
        }

        public void PlayPullLeverGround()
        {
            if (!Anim.GetCurrentAnimatorStateInfo(1).IsName("PullLeverGround")) Anim.Play("PullLeverGround", 1);
        }

        public void PlayOpenDoorInside()
        {
            if (!Anim.GetCurrentAnimatorStateInfo(1).IsName("OpenDoorInside")) Anim.Play("OpenDoorInside", 1);
        }

        public void PlayOpenDoorOutside()
        {
            if (!Anim.GetCurrentAnimatorStateInfo(1).IsName("OpenDoorOutside")) Anim.Play("OpenDoorOutside", 1);
        }

        public void PlayOpenDoorPush()
        {
            if (!Anim.GetCurrentAnimatorStateInfo(1).IsName("OpenDoorPush")) Anim.Play("OpenDoorPush", 1);
        }

        public void PlayCloseDoorInside()
        {
            if (!Anim.GetCurrentAnimatorStateInfo(1).IsName("CloseDoorInside")) Anim.Play("CloseDoorInside", 1);
        }

        public void PlayCloseDoorOutside()
        {
            if (!Anim.GetCurrentAnimatorStateInfo(1).IsName("CloseDoorOutside")) Anim.Play("CloseDoorOutside", 1);
        }

        public void PlayPressButton()
        {
            if (!Anim.GetCurrentAnimatorStateInfo(1).IsName("PressButton")) Anim.Play("PressButton", 1);
        }

        public void PlayPush()
        {
            if (Anim.GetBool(IsPushing)) return;
            Anim.Play("PushEnter", 1);
            Anim.SetBool(IsPushing, true);
        }

        public void PlayPushExit()
        {
            Anim.SetBool(IsPushing, false);
        }

        public void PlayJump()
        {
            if (Anim.GetBool(IsJumping)) return;
            Anim.Play("Jump", 1);
            Anim.SetBool(IsJumping, true);
            Anim.SetBool(IsFalling, true);


            print("Started Jumping Animation!");
        }

        void Start()
        {
            Initialize();
        }

        public void PlayJumpExit()
        {
            Anim.SetBool(IsJumping, false);
        }

        public void PlayFalling()
        {
            if (Anim.GetCurrentAnimatorStateInfo(1).IsName("Falling")) return;
            Anim.Play("Falling", 1);
            Anim.SetBool(IsFalling, true);
        }

        public void PlayFallingExit()
        {
            Anim.SetBool(IsFalling, false);
        }

        public void PlayHit()
        {
            Anim.Play("Hit", 2);
        }

        public void PlayDeath()
        {
            if (!Anim.GetBool(IsDead))
            {
                Anim.Play("Death", 2);
                Anim.SetBool(IsDead, true);
            }
        }

        public void PlayCrouchEnter()
        {
            if (Anim.GetBool(IsCrouching)) return;
            Anim.Play("Crouching");
            Anim.SetBool(IsCrouching, true);
        }

        public void PlayCrouchExit()
        {
            if (!Anim.GetBool(IsCrouching)) return;

            Anim.Play("Locomotion");
            Anim.SetBool(IsCrouching, false);
        }
    }
}