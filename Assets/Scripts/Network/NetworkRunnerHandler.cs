using Fusion;
using Fusion.Sockets;
using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkRunnerHandler : MonoBehaviour
{
    public static NetworkRunnerHandler Instance;

    public NetworkRunner NetworkRunnerPrefab;
    private NetworkRunner networkRunner;


    private const string PublicLobbyKey = "PublicLobbyID";

    private int PlaygroundSceneIndex = 1; 

    private void Awake()
    {
        Instance = this;

        CheckForNetworkRunner();
    }

    private void CheckForNetworkRunner()
    {
        NetworkRunner networkRunner = FindObjectOfType<NetworkRunner>();

        if (networkRunner != null)
            this.networkRunner = networkRunner;
    }

    private void Start()
    {
        if (networkRunner == null)
        {
            networkRunner = Instantiate(NetworkRunnerPrefab);
            networkRunner.name = "Network Runner";

        }


        //var client = InitializeNetworkRunner(networkRunner, GameMode.AutoHostOrClient, NetAddress.Any(),
        //    SceneRef.FromIndex(PlaygroundSceneIndex), PublicLobbyKey, "ProjectFusionRoom", null);

        Debug.Log("Server is Started"); 
    
    }

    protected virtual Task InitializeNetworkRunner(NetworkRunner runner, GameMode gameMode, NetAddress netAddress, 
        SceneRef sceneRef, string sessionName, string lobbyID, Action<NetworkRunner> initialized)
    {
        var sceneManager = runner.GetComponents(typeof(MonoBehaviour)).OfType<INetworkSceneManager>().FirstOrDefault();
        sceneManager ??= runner.gameObject.AddComponent<NetworkSceneManagerDefault>();

        runner.ProvideInput = true;
        runner.AddCallbacks(networkRunner.GetComponent<NetworkSpawner>());
        
        var sceneInfo = new NetworkSceneInfo();
        if (sceneRef.IsValid)
        {
            sceneInfo.AddSceneRef(sceneRef, LoadSceneMode.Single);
        }

        return runner.StartGame(new StartGameArgs
        {
            GameMode = gameMode,
            Address = netAddress,
            Scene = sceneInfo,
            SessionName = sessionName,
            CustomLobbyName = lobbyID,
            OnGameStarted = initialized,
            SceneManager = sceneManager,
            
        });


    }

    public void OnJoinLobby()
    {
        var task = JoinLobby();
    }

    private async Task JoinLobby()
    {
        Debug.Log("Joining Lobby in Progress");

        var result = await networkRunner.JoinSessionLobby(SessionLobby.Custom, PublicLobbyKey);

        if(result.Ok)
        {
            Debug.Log("Joining Lobby Successful");
        }
        else
        {
            Debug.Log("Unable to Join Lobby");
        }


    }


    public void CreateSession(string sessionName, int sceneIndex)
    {
        Debug.Log($"Creating Session: {sessionName}");

        SceneRef sceneRef = SceneRef.FromIndex(sceneIndex);

        var client = InitializeNetworkRunner(networkRunner, GameMode.Host, NetAddress.Any(),
            sceneRef, sessionName, PublicLobbyKey, null);

    }

    public void JoinSession(SessionInfo sessionInfo)
    {
        Debug.Log($"Joining Session: {sessionInfo.Name}");

        SceneRef sceneRef = SceneRef.FromIndex(PlaygroundSceneIndex);

        var client = InitializeNetworkRunner(networkRunner, GameMode.Client, NetAddress.Any(),
            sceneRef, sessionInfo.Name, PublicLobbyKey, null);

    }














}//class
