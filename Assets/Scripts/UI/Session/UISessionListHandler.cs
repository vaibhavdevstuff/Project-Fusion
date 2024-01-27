using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UISessionListHandler : MonoBehaviour
{
    public GameObject SessionListItemPrefab;
    public GameObject SessionListContainer;

    public GameObject SessionStatusObject;
    public TextMeshProUGUI SessionStatusText;

    public void ClearDefaultList()
    {
        foreach(Transform child in SessionListContainer.transform)
        {
            Destroy(child);
        }

        SessionStatusObject.SetActive(false);
    }

    public void AddSessionItemToList(SessionInfo sessionInfo)
    {
        UISessionInfoItem sessionItem = Instantiate(SessionListItemPrefab, SessionListContainer.transform).GetComponent<UISessionInfoItem>();

        sessionItem.SetInfo(sessionInfo);


    }

    private void NoSessionFound()
    {
        SessionStatusObject.SetActive(true);
        SessionStatusText.text = "No Game Session Found";
    }

    private void WaitForSession()
    {
        SessionStatusObject.SetActive(true);
        SessionStatusText.text = "Searching for Game Session...";
    }





}
