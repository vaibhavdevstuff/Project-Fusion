using Fusion;
using Fusion.Sockets;
using Player.Input;
using System;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

public class NetworkSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    public static NetworkSpawner Instance;

    [SerializeField] 
    private NetworkPlayer playerPrefab;
    
    [Space]
    [SerializedDictionary("ID","Player")] 
    public SerializedDictionary<PlayerRef, NetworkPlayer> spawnedCharacters;

    CharacterInput characterInput;
    UISessionListHandler sessionListHandler;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        sessionListHandler = FindObjectOfType<UISessionListHandler>(true);
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            // Create a unique position for the player
            Vector3 spawnPosition = new Vector3((player.RawEncoded % runner.Config.Simulation.PlayerCount) * 3, 1, 0);
            NetworkPlayer networkPlayerObject = runner.Spawn(playerPrefab, spawnPosition, Quaternion.identity, player);
            // Keep track of the player avatars for easy access
            spawnedCharacters.Add(player, networkPlayerObject);

        }

        Debug.Log("OnPlayerJoined"); 
    
    }
    public void OnInput(NetworkRunner runner, NetworkInput input) 
    { 
        if(characterInput == null && NetworkPlayer.Local != null)
        {
            characterInput = CharacterInput.Instance;
        }
        
        if (characterInput != null)
        {
            input.Set(characterInput.GetNetworkInput());
        }
    
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) 
    {
        if (!sessionListHandler) return;

        if(sessionList.Count > 0)
        {
            sessionListHandler.ClearSessionList();

            foreach (SessionInfo sessionInfo in sessionList)
            {
                sessionListHandler.AddSessionItemToList(sessionInfo);
            }
        }
        else
        {
            sessionListHandler.NoSessionFound();
        }
    
    
    }


    #region Rest of Network Runner Callbacks
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { Debug.Log("OnPlayerLeft"); }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { Debug.Log("OnShutdown | " + shutdownReason); }
    public void OnConnectedToServer(NetworkRunner runner) { Debug.Log("OnConnectedToServer"); }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { Debug.Log("OnDisconnectedFromServer"); }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { Debug.Log("OnConnectRequest"); }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { Debug.Log($"OnConnectFailed | {reason} | {remoteAddress}"); }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnSceneLoadDone(NetworkRunner runner) { Debug.Log("Scene Load Done"); }
    public void OnSceneLoadStart(NetworkRunner runner) { Debug.Log("Scene Load Start"); }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }

    #endregion










}//class
