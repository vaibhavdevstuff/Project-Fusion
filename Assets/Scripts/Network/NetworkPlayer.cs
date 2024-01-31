using Fusion;
using Game.Utils;

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{
    public static NetworkPlayer Local { get; private set; }


    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            // Set the local player instance.
            Local = this;

            // Set rendering layer for local player.
            transform.SetRenderLayerInChildren(LayerManager.LocalPlayer, true);

            // Set the object name with input authority tag.
            this.gameObject.name = $"Player {Object.Id} [InputAuthority]";
        }
        else
        {
            // Set rendering layer for remote player.
            transform.SetRenderLayerInChildren(LayerManager.RemotePlayer, true);

            // Disable Camera for remote players.
            CameraController cameraController = GetComponent<CameraController>();
            cameraController.DisableCameras();

            // Set the object name for remote players.
            this.gameObject.name = $"Player {Object.Id}";
        }

    }

    public void PlayerLeft(PlayerRef player)
    {
        if (player == Object.InputAuthority)
            Runner.Despawn(Object);
    }






}
