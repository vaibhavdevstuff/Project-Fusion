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

            this.gameObject.name = $"Player {Object.Id} [InputAuthority]";
        }
        else
        {
            //Disable Camera
            GameObject OtherPlayerVirtualCamera = GetComponent<CameraController>().CinemachineVirtualCamera.gameObject;
            OtherPlayerVirtualCamera.name = "Other Player " + OtherPlayerVirtualCamera.name;
            OtherPlayerVirtualCamera.SetActive(false);

            this.gameObject.name = $"Player {Object.Id}";
        }

    }

    public void PlayerLeft(PlayerRef player)
    {
        if (player == Object.InputAuthority)
            Runner.Despawn(Object);
    }






}
