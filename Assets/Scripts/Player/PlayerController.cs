using Fusion;
using Fusion.Addons.SimpleKCC;
using UnityEngine;
using Player.Input;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 10.0f;
    [SerializeField] private float speedAcceleration = 25.0f;
    [SerializeField] private float speedDeceleration = 30.0f;
    [SerializeField] private float jumpImpulse = 5.0f;
    [SerializeField] private float gravity = 30.0f;

    private SimpleKCC KCC;
    private Camera mainCamera;

    NetworkInputData networkInput;

    [Networked]
    private Vector3 _moveVelocity { get; set; }

    void Awake()
    {
        KCC = GetComponent<SimpleKCC>();

        mainCamera = Camera.main;
    }

    public override void FixedUpdateNetwork()
    {
        if(GetInput(out NetworkInputData networkInput))
        {
            this.networkInput = networkInput;
        }

        MoveCharacter();
    }

    void MoveCharacter()
    {
        KCC.AddLookRotation(new Vector2(-networkInput.LookDirection.y, networkInput.LookDirection.x));

        Vector3 inputDirection = KCC.TransformRotation * new Vector3(networkInput.MoveDirection.x, 0.0f, networkInput.MoveDirection.y);

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
            if (KCC.IsGrounded == true)
            {
                _jumpImpulse = Vector3.up * jumpImpulse;
            }
        }


        KCC.SetGravity(Vector3.down * gravity);

        KCC.Move(_moveVelocity, _jumpImpulse);

    }


    
}
