using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILobby : UIManager
{
    [Header("Buttons")]
    public Button BackButton;
    public Button CreateRoomButton;
    public Button CreateRoomConfirmButton;


    [Header("Back Button Data")]
    public GameObject VCam_Menu;
    public GameObject MainMenuUIPanel;

    [Header("Create Room Data")]
    public GameObject CreateRoomPanel;
    public TMP_InputField SessionNameInputField;


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
        CreateRoomConfirmButton.onClick.AddListener(OnCreateRoomConfirmButtonPress);
    }

    private void UnSubscribeButtons()
    {
        BackButton.onClick.RemoveAllListeners();
        CreateRoomButton.onClick.RemoveAllListeners();
        CreateRoomConfirmButton.onClick.RemoveAllListeners();
    }

    private void OnBackButtonPress()
    {
        VCam_Menu.SetActive(true);
        MainMenuUIPanel.SetActive(true);
        UIFader.FadeOut();
    }

    private void OnCreateRoomButtonPress()
    {
        CreateRoomPanel.SetActive(true);
    }

    private void OnCreateRoomConfirmButtonPress()
    {
        string sessionName = SessionNameInputField.text;

        if (string.IsNullOrEmpty(sessionName)) return;

        NetworkRunnerHandler networkRunnerHandler = NetworkRunnerHandler.Instance;

        if (networkRunnerHandler)
        {
            networkRunnerHandler.CreateSession(sessionName, 1);
        }
    }
















}
