using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class WeaponHandler : NetworkBehaviour
{
    public WeaponData weaponData;

    [Space]
    public Transform Gunpoint;


    private WeaponData currentWeaponData;

    private float lastFireTime;
    private float currentAmmo;

    [Networked] public bool IsFiring {  get; set; }
    [Networked] public bool IsReloading {  get; set; }

    private ParticleSystem muzzleFlashParticle;
    private ChangeDetector changeDetector;
    private NetworkInputData networkInput;
    private PlayerAnimationHandler anim;

    public override void Spawned()
    {
        changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);

        anim = GetComponent<PlayerAnimationHandler>();


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

        if (IsFiring)
            OnFiring();
            

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

    private void Fire(Vector3 ForwardVector)
    {
        if (IsReloading) 
            return;
        if (Time.time - lastFireTime < currentWeaponData.RateOfFire) 
            return;

        StartCoroutine(CR_Firing());

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
    }

    private void Reload()
    {
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
