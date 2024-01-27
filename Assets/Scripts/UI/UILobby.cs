using System;
using UnityEngine;
using UnityEngine.UI;

public class UILobby : UIManager
{
    [Header("Buttons")]
    public Button BackButton;
    public Button CreateRoomButton;


    [Header("Back Button Data")]
    public GameObject VCam_Menu;
    public GameObject MainMenuUIPanel;


    private void Start()
    {
        
    }

    private void OnEnable()
    {
        SubscribeButtons();
    }

    private void OnDisable()
    {
        UnSubscribeButtons();
    }


    private void SubscribeButtons()
    {
        BackButton.onClick.AddListener(OnBackButtonPress);
        CreateRoomButton.onClick.AddListener(OnCreateRoomButtonPress);
    }

    private void UnSubscribeButtons()
    {
        BackButton.onClick.RemoveAllListeners();
        CreateRoomButton.onClick.RemoveAllListeners();
    }

    private void OnBackButtonPress()
    {
        VCam_Menu.SetActive(true);
        MainMenuUIPanel.SetActive(true);
        UIFader.FadeOut();
    }

    private void OnCreateRoomButtonPress()
    {
        LoadNextScene();
    }


















}
