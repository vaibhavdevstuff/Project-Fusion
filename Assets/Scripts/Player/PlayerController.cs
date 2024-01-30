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

    private void GroundedCheck()
    {
        isGrounded = simpleKCC.IsGrounded;        
    }

    private void SetVerticleAim()
    {
        Vector2 pitchRotation = simpleKCC.GetLookRotation(true, false);

        finalAimValue = pitchRotation.x / 60f;
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

    private void OnDamage(float damage)
    {
        ShowHurtEffect();
    }

    private void OnDeath()
    {
        audioHandler.PlayDeathSound();
        anim.PlayDeathAnimation();
        cameraController.SwitchToDeathCamera();
    }

    private void ShowHurtEffect()
    {
        if (Object.HasInputAuthority)
            hurtEffect.GotHurt();
    }

    public void RespawnPlayer()
    {
        RPC_RespawnPlayer();

        cameraController.SwitchToTPSCamera();
        playerUI.UpdateHealthUI();
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_RespawnPlayer()
    {
        anim.SetNetTrigger(anim.AnimIDReset);

        Vector3 respawnPosition = new Vector3(Random.Range(-5f, 5f), 3f, Random.Range(-5f, 5f));
        transform.position = respawnPosition;

        health.ResetHealth();
    }


}
