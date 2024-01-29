using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIDeathScreen : MonoBehaviour
{
    [Header("Button")]
    public Button RespawnButton;
    public Button QuitButton;

    [Header("Panels")]
    public GameObject DeathUIPanel;

    GameObject localPlayer;
    CharacterHealth health;

    private void OnDisable()
    {
        UnSubscribeButton();
        SubscribeHealthOnDeath(false);
    }

    IEnumerator Start()
    {
        while (GameManager.Instance.LocalPlayer == null)
            yield return null;

        SubscribeButtons();
        SubscribeHealthOnDeath(true);
    }

    private void SubscribeButtons()
    {
        RespawnButton.onClick.AddListener(OnRespawnButtonPress);
        QuitButton.onClick.AddListener(OnQuitButtonPress);
    }

    private void UnSubscribeButton()
    {
        RespawnButton.onClick.RemoveAllListeners();
        QuitButton.onClick.RemoveAllListeners();
    }

    private void SubscribeHealthOnDeath(bool action)
    {
        if (health)
        {
            if (action)
                health.OnDeath += OnDeath;
            else 
                health.OnDeath -= OnDeath;

            return;
        }

        if (!localPlayer)
        {
            GetLocalPlayer();
        }
        if (localPlayer)
        {
            health = localPlayer.GetComponent<CharacterHealth>();

            if (action)
                health.OnDeath += OnDeath;
            else
                health.OnDeath -= OnDeath;
        }
        else
            Debug.LogError("LocalPlayer Not Found");
    }

    private void OnRespawnButtonPress()
    {
        if (localPlayer == null) GetLocalPlayer();

        if (localPlayer)
        {
            PlayerController playerController = localPlayer.GetComponent<PlayerController>();
            playerController.RespawnPlayer();
            DeathUIPanel.SetActive(false);
        }

    }

    private void OnQuitButtonPress()
    {
        
    }

    private void OnDeath()
    {
        Invoke(nameof(ShowDeathPanel), 2f);
    }

    private void ShowDeathPanel()
    {
        DeathUIPanel.SetActive(true);
    }

    private void GetLocalPlayer()
    {
        localPlayer = GameManager.Instance.LocalPlayer;
    }










}
