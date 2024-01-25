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

    [Header("Name Change Data")]
    public Button ChangeNameButton;
    public Button ChangeNameConfirmButton;
    public TextMeshProUGUI PlayerNickNameText;
    public GameObject PlayerNameChangePanel;
    public TMP_InputField PlayerNameInputField;


    private string currentPlayerName;

    private void Start()
    {
        

        SubscribeButtons();
        SetInitialNickName();

    }

    private void SubscribeButtons()
    {
        PlayButton.onClick.AddListener(OnPlayButtonPress);
        ChangeNameButton.onClick.AddListener(OnChangeNameButtonPress);
        ChangeNameConfirmButton.onClick.AddListener(OnChangeNameConfirmButtonPress);
    }

    private void OnPlayButtonPress()
    {
        LoadNextScene();
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
