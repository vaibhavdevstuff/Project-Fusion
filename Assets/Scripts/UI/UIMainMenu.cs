using System;
using TMPro;
using Unity.Tutorials.Core.Editor;
using UnityEngine;
using UnityEngine.UI;

public class UIMainMenu : UIManager
{
    [Header("Buttons")]
    public Button PlayButton;
    public Button SettingsButton;
    public Button QuitButton;

    [Header("Play Button Data")]
    public GameObject VCam_MainMenu;
    public GameObject LobbyUIPanel;

    [Header("Name Change Data")]
    public Button ChangeNameButton;
    public Button ChangeNameConfirmButton;
    public TextMeshProUGUI PlayerNickNameText;
    public GameObject PlayerNameChangePanel;
    public TMP_InputField PlayerNameInputField;


    private string currentPlayerName;

    private void Start()
    {
        SetInitialNickName();
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
        PlayButton.onClick.AddListener(OnPlayButtonPress);
        ChangeNameButton.onClick.AddListener(OnChangeNameButtonPress);
        ChangeNameConfirmButton.onClick.AddListener(OnChangeNameConfirmButtonPress);
    }

    private void UnSubscribeButtons()
    {
        PlayButton.onClick.RemoveAllListeners();
        ChangeNameButton.onClick.RemoveAllListeners();
        ChangeNameConfirmButton.onClick.RemoveAllListeners();
    }

    private void OnPlayButtonPress()
    {
        //LoadNextScene();
        VCam_MainMenu.SetActive(false);
        LobbyUIPanel.SetActive(true);
        UIFader.FadeOut();

    }

    private void SetInitialNickName()
    {
        if(PlayerPrefs.HasKey(GamePrefs.PlayerNickName))
        {
            currentPlayerName = PlayerPrefs.GetString(GamePrefs.PlayerNickName);
            PlayerNickNameText.text = currentPlayerName; 
        }
    }

    private void OnChangeNameButtonPress()
    {
        PlayerNameChangePanel.SetActive(true);

        PlayerNameInputField.text = currentPlayerName;
    }

    private void OnChangeNameConfirmButtonPress()
    {
        var InputText = PlayerNameInputField.text;

        if (string.IsNullOrEmpty(InputText)) return;

        currentPlayerName = InputText;

        PlayerNickNameText.text = currentPlayerName;

        PlayerPrefs.SetString(GamePrefs.PlayerNickName, currentPlayerName);

        PlayerNameChangePanel.SetActive(false);
    }

















}
