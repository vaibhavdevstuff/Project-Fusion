using Fusion;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayer : NetworkBehaviour
{

    [Header("Health")]
    public TextMeshProUGUI HealthText;
    public Image HealthSlider;

    [Header("Health")]
    public GameObject HitCrosshair;

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

        HitCrosshair.SetActive(false);
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

    public void UpdateHealthUI(float value = 0f)
    {
        currentHealth = health.CurrentHealth;

        HealthText.text = currentHealth.ToString();

        HealthSlider.fillAmount = currentHealth / maxHealth;
    }

    public void ShowHitEffect()
    {
        if (Object.HasInputAuthority)
            StartCoroutine(CR_ShowHitEffect());
    }

    IEnumerator CR_ShowHitEffect()
    {
        HitCrosshair.SetActive(true);

        yield return new WaitForSeconds(0.2f);

        HitCrosshair.SetActive(false);
    }








}
