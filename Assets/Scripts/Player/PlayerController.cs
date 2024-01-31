using Fusion;
using Fusion.Addons.SimpleKCC;
using System.Collections.Generic;
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
    float finalAimValue;

    Vector2 moveAnimBlendSpeed;

    SimpleKCC simpleKCC;
    PlayerAnimationHandler anim;
    WeaponHandler weaponHandler;
    CharacterHealth health;
    PlayerAudioHandler audioHandler;
    CameraController cameraController;
    UIPlayer playerUI;
    HurtEffect hurtEffect;

    NetworkInputData networkInput;

    [Networked]
    private Vector3 moveVelocity { get; set; }

    private bool IsGrounded { get { return isGrounded; } }

    void Awake()
    {
        simpleKCC = GetComponent<SimpleKCC>();
        anim = GetComponent<PlayerAnimationHandler>();
        weaponHandler = GetComponent<WeaponHandler>();
        health = GetComponent<CharacterHealth>();
        audioHandler = GetComponent<PlayerAudioHandler>();
        cameraController = GetComponent<CameraController>();
        playerUI = GetComponentInChildren<UIPlayer>();
        hurtEffect = FindObjectOfType<HurtEffect>();
    }

    public override void Spawned()
    {
        if (!health)
            health = GetComponent<CharacterHealth>();

        if (health)
        {
            health.OnDamage += OnDamage;
            health.OnDeath += OnDeath;
        }

        SetLocalPlayer();
    }

    private void Start()
    {
        if (CursorManager.Instance == null)
            Debug.LogError("Cursor Manager not found");

        CursorManager.Instance?.HideCursor();

    }

    private void SetLocalPlayer()
    {
        if (Object.HasInputAuthority)
            GameManager.Instance.LocalPlayer = this.gameObject;
    }

    public override void FixedUpdateNetwork()
    {
        if(GetInput(out NetworkInputData networkInput))
        {
            this.networkInput = networkInput;
        }

        if (!health.IsAlive) return;

        GroundedCheck();
        MoveCharacter();
        SetVerticleAim();

        AnimationUpdate();
    }

    /// <summary>
    /// Updates character animations based on player input and state.
    /// </summary>
    private void AnimationUpdate()
    {
        //Movement
        moveAnimBlendSpeed.x = Mathf.MoveTowards(anim.GetFloat(anim.AnimIDMoveX), networkInput.MoveDirection.x, animBlendSpeed * Runner.DeltaTime);
        moveAnimBlendSpeed.y = Mathf.MoveTowards(anim.GetFloat(anim.AnimIDMoveZ), networkInput.MoveDirection.y, animBlendSpeed * Runner.DeltaTime);

        if (moveAnimBlendSpeed.magnitude > 0f)
        {
            anim.SetFloat(anim.AnimIDMoveX, moveAnimBlendSpeed.x);
            anim.SetFloat(anim.AnimIDMoveZ, moveAnimBlendSpeed.y);
        }
        else
        {
            anim.SetFloat(anim.AnimIDMoveX, 0);
            anim.SetFloat(anim.AnimIDMoveZ, 0);
        }

        //Vertical Aim
        anim.SetFloat(anim.AnimIDVerticalAim, finalAimValue);

        //Grounded
        bool animatorGround = anim.GetBool(anim.AnimIDGrounded);
        if (isGrounded != animatorGround)
        {
            anim.SetBool(anim.AnimIDGrounded, isGrounded);
        }

        //Firing
        if(networkInput.Fire != anim.GetBool(anim.AnimIDFiring))
            anim.SetBool(anim.AnimIDFiring, networkInput.Fire);

        //Reload
        if (weaponHandler.IsReloading != anim.GetBool(anim.AnimIDReload))
            anim.SetBool(anim.AnimIDReload, weaponHandler.IsReloading);

    }

    /// <summary>
    /// Checks if the player is grounded and updates the corresponding variable.
    /// </summary>
    private void GroundedCheck()
    {
        isGrounded = simpleKCC.IsGrounded;        
    }

    /// <summary>
    /// Sets the vertical aiming value based on player input.
    /// </summary>
    private void SetVerticleAim()
    {
        Vector2 pitchRotation = simpleKCC.GetLookRotation(true, false);

        finalAimValue = pitchRotation.x / 60f;
    }

    /// <summary>
    /// Moves the character based on input and updates animations accordingly.
    /// </summary>
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

        moveVelocity = Vector3.Lerp(moveVelocity, desiredMoveVelocity, acceleration * Runner.DeltaTime);

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

        simpleKCC.Move(moveVelocity, _jumpImpulse);

    }

    /// <summary>
    /// Actions to be performed when the player takes damage.
    /// </summary>
    private void OnDamage(float damage)
    {
        ShowHurtEffect();
    }

    /// <summary>
    /// Actions to be performed when the player dies.
    /// </summary>
    private void OnDeath()
    {
        audioHandler.PlayDeathSound();
        anim.PlayDeathAnimation();
        cameraController.SwitchToDeathCamera();
    }

    /// <summary>
    /// Displays a hurt effect when the player takes damage.
    /// </summary>
    private void ShowHurtEffect()
    {
        if (Object.HasInputAuthority)
            hurtEffect.GotHurt();
    }

    /// <summary>
    /// Perform necessary Steps to Respawns the player
    /// </summary>
    public void RespawnPlayer()
    {
        RPC_RespawnPlayerInput();

        cameraController.SwitchToTPSCamera();
        playerUI.UpdateHealthUI();
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_RespawnPlayerInput()
    {
        RPC_RespawnPlayer();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_RespawnPlayer()
    {
        anim.ResetAnimation();


        health.ResetHealth();
    }

}
