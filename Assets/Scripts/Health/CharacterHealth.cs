using Fusion;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Fusion.NetworkBehaviour;
using UnityEngine.Splines;

public class CharacterHealth : NetworkBehaviour
{
    
    [SerializeField] [NaughtyAttributes.ReadOnly] private float health;

    [Space]
    public float MaxHealth;
    public float MinHealth;

    [Space]
    [SerializeField] private float invinsibleDurationAfterSpawn = 2f;



    [SerializeField] public bool IsInvinclible => invinsibleTimer.ExpiredOrNotRunning(Runner) == false;
    [SerializeField] public bool IsAlive => CurrentHealth > 0;

    [Networked] public float CurrentHealth { get; set; }
    [Networked] private TickTimer invinsibleTimer { get; set; }

    private float lastHealthValue;

    public Action<float> OnHeal;
    public Action<float> OnDamage;
    public Action OnDeath;

    private ChangeDetector changeDetector;

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            CurrentHealth = MaxHealth;

            lastHealthValue = CurrentHealth;

            invinsibleTimer = TickTimer.CreateFromSeconds(Runner, invinsibleDurationAfterSpawn);
        }

        changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }

    public override void FixedUpdateNetwork()
    {
        if(health != CurrentHealth)
            health = CurrentHealth;

        CheckForNetworkPropsChanges();
    }

    private void CheckForNetworkPropsChanges()
    {
        foreach (var change in changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(CurrentHealth):
                    OnCurrentHealthChanged();
                    break;
            }
        }
    }

    private void OnCurrentHealthChanged()
    {
        if (lastHealthValue == CurrentHealth) return;
        if (!Runner.IsForward) return;

        if (lastHealthValue > CurrentHealth )
            InvokeDamageEvents(lastHealthValue - CurrentHealth);

        if (lastHealthValue < CurrentHealth)
            InvokeHealthEvents(CurrentHealth - lastHealthValue);
        
        if (CurrentHealth == 0)
            InvokeDeathEvents();

        lastHealthValue = CurrentHealth;

    }

    public bool ApplyDamage(float damage)
    {
        if (CurrentHealth <= 0f)
            return false;

        if (IsInvinclible)
            return false;
        
        CurrentHealth -= damage;

        return true;
    }

    public bool AddHeal(float health)
    {
        if (CurrentHealth >= 0f)
            return false;
        if (CurrentHealth >= MaxHealth)
            return false;

        CurrentHealth = Mathf.Min(CurrentHealth + health, MaxHealth);

        return true;
    }

    public void ResetHealth()
    {
        CurrentHealth = MaxHealth;
    }


    #region Events

    private void InvokeHealthEvents(float HealAmount)
    {
        if (Runner.IsServer)
            RPC_InvokeHealEvents(HealAmount);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_InvokeHealEvents(float HealAmount)
    {
        OnDamage?.Invoke(HealAmount);
    }

    private void InvokeDamageEvents(float Damage)
    {
        if (Runner.IsServer)
            RPC_InvokeDamageEvents(Damage);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_InvokeDamageEvents(float Damage)
    {
        OnDamage?.Invoke(Damage);
    }

    private void InvokeDeathEvents()
    {
        if (Runner.IsServer)
            RPC_InvokeDeathEvents();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_InvokeDeathEvents()
    {
        OnDeath?.Invoke();
    }

    #endregion









}
