using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    public GameObject LocalPlayer;

    private void Awake()
    {
        Instance = this;
    }

    public void DisconnectFromServer()
    {
        if(Runner.IsServer)
        {
            NetworkSpawner spawner = FindObjectOfType<NetworkSpawner>();

            Destroy(spawner.gameObject);
            
            SceneManager.LoadScene(0);

        }
        else
        {
            RPC_DisconnectFromServer(Runner.LocalPlayer);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_DisconnectFromServer(PlayerRef playerRef)
    {
        if (Runner.IsServer)
        {
            Runner.Disconnect(playerRef);
        }
    }



















}
