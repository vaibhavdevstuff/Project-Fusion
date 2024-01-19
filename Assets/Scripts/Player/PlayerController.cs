using Fusion;
using Fusion.Addons.SimpleKCC;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 10.0f;
    [SerializeField] private float speedAcceleration = 25.0f;
    [SerializeField] private float speedDeceleration = 30.0f;
    [SerializeField] private float jumpImpulse = 5.0f;
    [SerializeField] private float gravity = 30.0f;

    [Header("Animation")]
    [SerializeField] private float animBlendSpeed = 5f;

    bool isGrounded;

    Vector2 moveAnimBlendSpeed;

    SimpleKCC simpleKCC;
    PlayerAnimationHandler anim;

    NetworkInputData networkInput;

    [Networked]
    private Vector3 _moveVelocity { get; set; }

    private bool IsGrounded { get { return isGrounded; } }

    void Awake()
    {
        simpleKCC = GetComponent<SimpleKCC>();
        anim = GetComponent<PlayerAnimationHandler>();
    }

    public override void FixedUpdateNetwork()
    {
        if(GetInput(out NetworkInputData networkInput))
        {
            this.networkInput = networkInput;
        }

        GroundedCheck();
        MoveCharacter();
        SetVerticleAim();
    }

    private void GroundedCheck()
    {
        isGrounded = simpleKCC.IsGrounded;

        //update animator if using character
        bool animatorGround = anim.GetBool(anim.AnimIDGrounded);

        if (isGrounded != animatorGround)
        {
            anim.SetBool(anim.AnimIDGrounded, isGrounded);
        }
    }

    private void SetVerticleAim()
    {
        Vector2 pitchRotation = simpleKCC.GetLookRotation(true, false);

        float finalAimValue = pitchRotation.x / 60f;

        anim.SetFloat(anim.AnimIDVerticalAim, finalAimValue);
    }

    private void MoveCharacter()
    {
        simpleKCC.AddLookRotation(-networkInput.LookDirection.y, networkInput.LookDirection.x, -30f, 60f);
        
        Vector3 inputDirection = simpleKCC.TransformRotation * new Vector3(networkInput.MoveDirection.x, 0.0f, networkInput.MoveDirection.y);

        Vector3 desiredMoveVelocity = inputDirection * moveSpeed;

        float acceleration;
        if (desiredMoveVelocity == Vector3.zero)
        {
            acceleration = speedDeceleration;
        }
        else
        {
            acceleration = speedAcceleration;
        }

        _moveVelocity = Vector3.Lerp(_moveVelocity, desiredMoveVelocity, acceleration * Runner.DeltaTime);

        Vector3 _jumpImpulse = Vector3.zero;
        if (networkInput.Jump)
        {
            if (isGrounded)
            {
                _jumpImpulse = Vector3.up * jumpImpulse;
                anim.SetBool(anim.AnimIDJump, true);
            }
        }
        if(!isGrounded)
        {
            if (anim.GetBool(anim.AnimIDJump))
                anim.SetBool(anim.AnimIDJump, false);
        }


        simpleKCC.SetGravity(Vector3.down * gravity);

        simpleKCC.Move(_moveVelocity, _jumpImpulse);

        moveAnimBlendSpeed.x = Mathf.Lerp(anim.GetFloat(anim.AnimIDMoveX), networkInput.MoveDirection.x, animBlendSpeed * Runner.DeltaTime);
        moveAnimBlendSpeed.y = Mathf.Lerp(anim.GetFloat(anim.AnimIDMoveZ), networkInput.MoveDirection.y, animBlendSpeed * Runner.DeltaTime);

        if(moveAnimBlendSpeed.magnitude > 0f)
        {
            anim.SetFloat(anim.AnimIDMoveX, moveAnimBlendSpeed.x);
            anim.SetFloat(anim.AnimIDMoveZ, moveAnimBlendSpeed.y);
        }

    }


    
}
