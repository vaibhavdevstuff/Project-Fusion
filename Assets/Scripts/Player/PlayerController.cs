using Fusion;
using Fusion.Addons.SimpleKCC;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 10.0f;
    [SerializeField] private float speedAcceleration = 25.0f;
    [SerializeField] private float speedDeceleration = 30.0f;
    [SerializeField] private float jumpImpulse = 5.0f;
    [SerializeField] private float gravity = 30.0f;

    private SimpleKCC KCC;
    private PlayerInput input;
    private Camera mainCamera;

    [Networked]
    private Vector3 _moveVelocity { get; set; }

    void Awake()
    {
        KCC = GetComponent<SimpleKCC>();
        input = GetComponent<PlayerInput>();

        mainCamera = Camera.main;
    }

    public override void FixedUpdateNetwork()
    {
        MoveCharacter();
    }

    void MoveCharacter()
    {
        KCC.AddLookRotation(new Vector2(-input.LookDirection.x, input.LookDirection.y));

        Vector3 inputDirection = KCC.TransformRotation * new Vector3(input.MoveDirection.x, 0.0f, input.MoveDirection.y);

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
        if (input.Jump)
        {
            if (KCC.IsGrounded == true)
            {
                _jumpImpulse = Vector3.up * jumpImpulse;
            }

            input.Jump = false;
        }


        KCC.SetGravity(Vector3.down * gravity);

        KCC.Move(_moveVelocity, _jumpImpulse);

    }


    
}
