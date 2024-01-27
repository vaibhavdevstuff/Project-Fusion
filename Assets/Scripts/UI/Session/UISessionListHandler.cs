using Fusion;
using System;
using TMPro;
using UnityEngine;

public class UISessionListHandler : MonoBehaviour
{
    public GameObject SessionListItemPrefab;
    public GameObject SessionListContainer;

    public GameObject SessionStatusObject;
    public TextMeshProUGUI SessionStatusText;

    private void Awake()
    {
        //Clear Default Debug List UI
        ClearSessionList();

        WaitForSession();
    }

    public void ClearSessionList()
    {
        foreach(Transform child in SessionListContainer.transform)
        {
            Destroy(child.gameObject);
        }

        SessionStatusObject.SetActive(false);
    }

    public void AddSessionItemToList(SessionInfo sessionInfo)
    {
        UISessionInfoItem sessionItem = Instantiate(SessionListItemPrefab, SessionListContainer.transform).GetComponent<UISessionInfoItem>();

        sessionItem.SetInfo(sessionInfo);

        sessionItem.OnJoinSession += OnJoinSession;
    }

    private void OnJoinSession(SessionInfo info)
    {
        NetworkRunnerHandler networkRunnerHandler = NetworkRunnerHandler.Instance;
            
        if (networkRunnerHandler != null)
        {
            networkRunnerHandler.JoinSession(info);
        }
    }

    public void NoSessionFound()
    {
        ClearSessionList() ;

        SessionStatusObject.SetActive(true);
        SessionStatusText.text = "No Game Session Found";
    }

    public void WaitForSession()
    {
        ClearSessionList();

        SessionStatusObject.SetActive(true);
        SessionStatusText.text = "Searching for Game Session...";
    }





}
