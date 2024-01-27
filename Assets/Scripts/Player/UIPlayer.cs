using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayer : NetworkBehaviour
{

    [Header("Health")]
    public TextMeshProUGUI HealthText;
    public Image HealthSlider;



    private float maxHealth;
    private float currentHealth;

    private CharacterHealth health;

    void Awake()
    {
        health = GetComponentInParent<CharacterHealth>();
    }

    private void OnEnable()
    {
        if(health == null)
            health = GetComponentInParent<CharacterHealth>();

        if (health)
        {
            maxHealth = health.MaxHealth;

            health.OnDamage += UpdateHealthUI;
            health.OnHeal += UpdateHealthUI;
        }
    }

    private void Start()
    {
        if(!Object.HasInputAuthority)
            this.gameObject.SetActive(false);

        UpdateHealthUI();
    }

    private void OnDisable()
    {
        health.OnDamage -= UpdateHealthUI;
        health.OnHeal -= UpdateHealthUI;
    }

    private void UpdateHealthUI(float value = 0f)
    {
        currentHealth = health.CurrentHealth;

        HealthText.text = currentHealth.ToString();

        HealthSlider.fillAmount = currentHealth / maxHealth;
    }

    void Update()
    {
        
    }










}
