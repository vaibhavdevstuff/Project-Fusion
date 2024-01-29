using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject LocalPlayer;

    private void Awake()
    {
        Instance = this;
    }












}
