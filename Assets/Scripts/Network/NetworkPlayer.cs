using Fusion;
using Game.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{
    public static NetworkPlayer Local { get; private set; }


    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            Local = this;
            transform.SetRenderLayerInChildren(LayerManager.LocalPlayer);
        }
        else
        {
            //Disable Camera
            GameObject OtherPlayerVirtualCamera = GetComponent<CameraController>().CinemachineVirtualCamera.gameObject;
            OtherPlayerVirtualCamera.name = "Other Player " + OtherPlayerVirtualCamera.name;
            OtherPlayerVirtualCamera.SetActive(false);

        }
            
    }

    public void PlayerLeft(PlayerRef player)
    {
        if (player == Object.InputAuthority)
            Runner.Despawn(Object);
    }






}
