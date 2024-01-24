using Fusion;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHealth : NetworkBehaviour
{
    
    [SerializeField] [NaughtyAttributes.ReadOnly] private float health;

    [Space]
    [SerializeField] private float MaxHealth;
    [SerializeField] private float MinHealth;

    [Space]
    [SerializeField] private float invinsibleDurationAfterSpawn = 2f;



    [SerializeField] public bool IsInvinclible => invinsibleTimer.ExpiredOrNotRunning(Runner) == false;
    [SerializeField] public bool IsAlive => CurrentHealth > 0;

    [Networked] private float CurrentHealth { get; set; }
    [Networked] private TickTimer invinsibleTimer { get; set; }


    public override void Spawned()
    {
        Debug.Log(Object.Id + " Spawned");
        if (HasStateAuthority)
        {
        Debug.Log(Object.Id + " SetHealth");
            CurrentHealth = MaxHealth;

            invinsibleTimer = TickTimer.CreateFromSeconds(Runner, invinsibleDurationAfterSpawn);
        }
    }

    public override void FixedUpdateNetwork()
    {
        if(health != CurrentHealth)
            health = CurrentHealth;
    }

    public bool ApplyDamage(float damage)
    {
        if (CurrentHealth <= 0f)
            return false;

        if (IsInvinclible)
            return false;
        Debug.Log(Object.Id + " Apply Damage " +  damage);
        CurrentHealth -= damage;

        return true;
    }

    public bool AddHeal(float health)
    {
        if (CurrentHealth <= 0f)
            return false;
        if (CurrentHealth >= MaxHealth)
            return false;

        CurrentHealth = Mathf.Min(CurrentHealth + health, MaxHealth);

        return true;
    }













}
