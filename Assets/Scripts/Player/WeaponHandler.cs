using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class WeaponHandler : NetworkBehaviour
{
    public float FiringRate = 0.15f;


    private float lastFireTime;
    
    [Networked] public bool IsFiring {  get; set; }

    private ChangeDetector changeDetector;
    private NetworkInputData networkInput;

    public override void Spawned()
    {
        changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData networkInput))
        {
            this.networkInput = networkInput;
        }

        
        UpdateInput();
        CheckForNetworkPropsChanges();
    }

    private void UpdateInput()
    {
        if (networkInput.Fire)
            Fire(networkInput.ForwardViewVector);
    }

    private void CheckForNetworkPropsChanges()
    {
        foreach (var change in changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(IsFiring):
                    OnFireChanged();
                    break;
            }
        }
    }

    private void Fire(Vector3 ForwardVector)
    {
        if (Time.time - lastFireTime < FiringRate) return;

        lastFireTime = Time.time;
    }

    private void OnFireChanged()
    {

    }







}
