using Fusion;
using Fusion.Sockets;
using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkRunnerHandler : MonoBehaviour
{
    public NetworkRunner NetworkRunnerPrefab;
    private NetworkRunner networkRunner;

    private void Start()
    {
        networkRunner = Instantiate(NetworkRunnerPrefab);
        networkRunner.name = "Network Runner";

        var client = InitializeNetworkRunner(networkRunner, GameMode.AutoHostOrClient, NetAddress.Any(),
            SceneRef.FromIndex(1), null);

        Debug.Log("Server is Started"); 
    
    }


    protected virtual Task InitializeNetworkRunner(NetworkRunner runner, GameMode gameMode, NetAddress netAddress, 
        SceneRef sceneRef, Action<NetworkRunner> initialized)
    {
        var sceneManager = runner.GetComponents(typeof(MonoBehaviour)).OfType<INetworkSceneManager>().FirstOrDefault();
        sceneManager ??= runner.gameObject.AddComponent<NetworkSceneManagerDefault>();

        runner.ProvideInput = true;
        runner.AddCallbacks(networkRunner.GetComponent<NetworkSpawner>());
        
        var sceneInfo = new NetworkSceneInfo();
        if (sceneRef.IsValid)
        {
            sceneInfo.AddSceneRef(sceneRef, LoadSceneMode.Additive);
        }

        return runner.StartGame(new StartGameArgs
        {
            GameMode = gameMode,
            Address = netAddress,
            Scene = sceneInfo,
            SessionName = "ProjectFusionRoom",
            OnGameStarted = initialized,
            SceneManager = sceneManager,
            
        });


    }


}
