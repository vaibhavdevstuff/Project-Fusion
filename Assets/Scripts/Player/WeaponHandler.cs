using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class WeaponHandler : NetworkBehaviour
{
    public WeaponData weaponData;
    public Transform Gunpoint;

    [Space]
    public LayerMask weaponHitLayer;

    [Space]

    private int visualCount;

    private float lastFireTime;
    private float currentAmmo;

    [Networked] public bool IsFiring {  get; set; }
    [Networked] public bool IsReloading {  get; set; }
    [Networked] public Vector3 HitPosition{  get; set; }
    [Networked] public int fireCount{  get; set; }

    private WeaponData currentWeaponData;

    private Camera cam;
    private ParticleSystem muzzleFlashParticle;
    private ChangeDetector changeDetector;
    private NetworkInputData networkInput;
    private PlayerAnimationHandler anim;

    public override void Spawned()
    {
        changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);

        anim = GetComponent<PlayerAnimationHandler>();
        cam = Camera.main;

        visualCount = fireCount;

        SetupWeapon(weaponData);
    }

    private void SetupWeapon(WeaponData _weaponData)
    {
        if(_weaponData == null)
        {
            Debug.LogError("Weapon Data is Null", gameObject);
            return;
        }

        currentWeaponData = _weaponData;

        var muzzleFlash = Instantiate(currentWeaponData.MuzzelFlashPrefab, Gunpoint.position, Quaternion.identity);
        muzzleFlash.transform.parent = Gunpoint;
        muzzleFlash.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        muzzleFlashParticle = muzzleFlash.GetComponent<ParticleSystem>();

        currentAmmo = currentWeaponData.MagazineSize;
    }

    private void Start()
    {
        //------------------------------------------------------------------
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData networkInput))
        {
            this.networkInput = networkInput;
        }

        if (currentAmmo <= 0)
            Reload();            

        ProcessInput();
        //CheckForNetworkPropsChanges();
    }

    private void ProcessInput()
    {
        if (networkInput.Fire)
            Fire(networkInput.ForwardViewVector);

        if (networkInput.Reload)
            Reload();
    }

    private void CheckForNetworkPropsChanges()
    {
        foreach (var change in changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(IsFiring):
                    //OnFireChanged();
                    break;
                case nameof(IsReloading):
                    break;
            }
        }
    }

    public override void Render()
    {
        for (int i = visualCount; i < fireCount; i++)
        {
            OnFiring();
            visualCount = fireCount;
        }

    }

    private void Fire(Vector3 ForwardVector)
    {
        if (IsReloading) 
            return;
        if (Time.time - lastFireTime < currentWeaponData.RateOfFire) 
            return;

        StartCoroutine(CR_Firing());
        ProcessHitScan(ForwardVector);

        lastFireTime = Time.time;
        
        currentAmmo--;
    }

    IEnumerator CR_Firing()
    {
        IsFiring = true;

        yield return new WaitForSeconds(0.09f);        

        IsFiring = false;
    }

    private void OnFiring()
    {
        muzzleFlashParticle.Play();

        var projectileObject = Instantiate(currentWeaponData.ProjectilePrefab, Gunpoint.position, Gunpoint.rotation);
        var projectile = projectileObject.GetComponent<Projectile>();
        projectile.SetHit(HitPosition, Vector3.zero, false);
    }

    private void ProcessHitScan(Vector3 ForwardVector)
    {
        var hitOptions = HitOptions.IncludePhysX;

        Runner.LagCompensation.Raycast(
            cam.transform.position,
            ForwardVector,
            currentWeaponData.Range,
            Object.InputAuthority,
            out var hitInfo,
            weaponHitLayer,
            hitOptions);

        bool hitOtherPlayer = false;

        Vector3 _hitPosition = cam.transform.position + ForwardVector * currentWeaponData.Range;

        if(hitInfo.Hitbox != null)
        {
            hitOtherPlayer = true;
            _hitPosition = hitInfo.Point;
        }
        if(hitInfo.Collider != null)
        {
            _hitPosition = hitInfo.Point;
        }

        HitPosition = _hitPosition;

        if (hitOtherPlayer)
        {
            Debug.DrawRay(cam.transform.position, ForwardVector * currentWeaponData.Range, Color.red, 1f);
        }
        else
        {
            Debug.DrawRay(cam.transform.position, ForwardVector * currentWeaponData.Range, Color.green, 1f);
        }

        fireCount++;
    }

    private void Reload()
    {
        if (IsReloading) 
            return;
        if (currentAmmo == currentWeaponData.MagazineSize)
            return;

        IsReloading = true;
        StartCoroutine(CR_Reload());
    }

    IEnumerator CR_Reload()
    {
        // Simulate reloading time
        yield return new WaitForSeconds(0.15f);
        float waitTime = anim.Animator.GetCurrentPlayingAnimationTime(2);
        yield return new WaitForSeconds(waitTime - 0.2f);

        // Refill the magazine
        currentAmmo = currentWeaponData.MagazineSize;

        IsReloading = false;

    }






}
