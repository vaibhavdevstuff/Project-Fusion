using Fusion;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISessionInfoItem : MonoBehaviour
{
    public TextMeshProUGUI PlayerNumberText;
    public TextMeshProUGUI RoomNameText;
    public Button JoinButton;

    SessionInfo sessionInfo;

    public Action<SessionInfo> OnJoinSession;

    private void OnEnable()
    {
        if (JoinButton)
            JoinButton.onClick.AddListener(OnJoinButtonPress);
        else
            Debug.LogError("Join Button Reference Missing");
    }

    private void OnDisable()
    {
        if (JoinButton)
            JoinButton.onClick.RemoveAllListeners();
        else
            Debug.LogError("Join Button Reference Missing");
    }

    private void OnJoinButtonPress()
    {
        OnJoinSession?.Invoke(sessionInfo);
    }

    public void SetInfo(SessionInfo info)
    {
        sessionInfo = info;

        PlayerNumberText.text = $"{sessionInfo.PlayerCount}/{sessionInfo.MaxPlayers}";
        RoomNameText.text = sessionInfo.Name;

        bool isJoinButtonActive = true;

        if(sessionInfo.PlayerCount >= sessionInfo.MaxPlayers)
        {
            isJoinButtonActive = false;
        }

        JoinButton.interactable = isJoinButtonActive;

    }









}
