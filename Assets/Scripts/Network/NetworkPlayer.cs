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
            transform.SetRenderLayerInChildren(LayerManager.Player, true);

            this.gameObject.name = $"Player {Object.Id} [InputAuthority]";
        }
        else
        {
            transform.SetRenderLayerInChildren(LayerManager.Player, true);

            //Disable Camera
            CameraController cameraController = GetComponent<CameraController>();
            cameraController.DisableCameras();
            //GameObject OtherPlayerVirtualCamera = cameraController.CinemachineVirtualCamera.gameObject;
            //OtherPlayerVirtualCamera.name = $"{Object.Id} {OtherPlayerVirtualCamera.name}";
            //OtherPlayerVirtualCamera.SetActive(false);

            this.gameObject.name = $"Player {Object.Id}";
        }

    }

    public void PlayerLeft(PlayerRef player)
    {
        if (player == Object.InputAuthority)
            Runner.Despawn(Object);
    }






}
