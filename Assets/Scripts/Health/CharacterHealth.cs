using Fusion;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Fusion.NetworkBehaviour;
using UnityEngine.Splines;

/// <summary>
/// Manages the health and related events for a character in a networked environment.
/// </summary>
public class CharacterHealth : NetworkBehaviour
{
    [SerializeField][NaughtyAttributes.ReadOnly] private float health;

    [Space]
    public float MaxHealth;
    public float MinHealth;

    [Space]
    [SerializeField] private float invincibleDurationAfterSpawn = 2f;

    [SerializeField] public bool IsInvincible => invincibleTimer.ExpiredOrNotRunning(Runner) == false;
    [SerializeField] public bool IsAlive => CurrentHealth > 0;

    [Networked] public float CurrentHealth { get; set; }
    [Networked] private TickTimer invincibleTimer { get; set; }

    private float lastHealthValue;

    public Action<float> OnHeal;
    public Action<float> OnDamage;
    public Action OnDeath;

    private ChangeDetector changeDetector;

    #region Unity Callbacks

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            CurrentHealth = MaxHealth;
            lastHealthValue = CurrentHealth;
            invincibleTimer = TickTimer.CreateFromSeconds(Runner, invincibleDurationAfterSpawn);
        }

        changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }

    private void OnDisable()
    {
        OnHeal = null;
        OnDamage = null;
        OnDeath = null;
    }

    public override void FixedUpdateNetwork()
    {
        if (health != CurrentHealth)
            health = CurrentHealth;

        CheckForNetworkPropsChanges();
    }

    #endregion

    /// <summary>
    /// Checks for changes in network properties and invokes appropriate events.
    /// </summary>
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

    /// <summary>
    /// Called when the current health property is changed.
    /// </summary>
    private void OnCurrentHealthChanged()
    {
        if (lastHealthValue == CurrentHealth) return;
        if (!Runner.IsForward) return;

        if (lastHealthValue > CurrentHealth)
            InvokeDamageEvents(lastHealthValue - CurrentHealth);

        if (lastHealthValue < CurrentHealth)
            InvokeHealthEvents(CurrentHealth - lastHealthValue);

        if (CurrentHealth == 0)
            InvokeDeathEvents();

        lastHealthValue = CurrentHealth;
    }

    /// <summary>
    /// Applies damage to the character.
    /// </summary>
    public bool ApplyDamage(float damage)
    {
        if (CurrentHealth <= 0f)
            return false;

        if (IsInvincible)
            return false;

        CurrentHealth -= damage;

        return true;
    }

    /// <summary>
    /// Adds healing to the character.
    /// </summary>
    public bool AddHeal(float health)
    {
        if (CurrentHealth >= 0f)
            return false;

        if (CurrentHealth >= MaxHealth)
            return false;

        CurrentHealth = Mathf.Min(CurrentHealth + health, MaxHealth);

        return true;
    }

    /// <summary>
    /// Resets the character's health to the maximum.
    /// </summary>
    public void ResetHealth()
    {
        CurrentHealth = MaxHealth;
    }

    #region Events

    /// <summary>
    /// Invokes events related to healing.
    /// </summary>
    private void InvokeHealthEvents(float healAmount)
    {
        if (Runner.IsServer)
            RPC_InvokeHealEvents(healAmount);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_InvokeHealEvents(float healAmount)
    {
        OnHeal?.Invoke(healAmount);
    }

    /// <summary>
    /// Invokes events related to taking damage.
    /// </summary>
    private void InvokeDamageEvents(float damage)
    {
        if (Runner.IsServer)
            RPC_InvokeDamageEvents(damage);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_InvokeDamageEvents(float damage)
    {
        OnDamage?.Invoke(damage);
    }

    /// <summary>
    /// Invokes events related to character death.
    /// </summary>
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
