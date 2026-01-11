using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

public class GameBootstrap : MonoBehaviour, INetworkRunnerCallbacks
{
  private NetworkRunner _runner;
  [SerializeField] private bool forceServerInEditor = false;


  private async void Start()
  {
    _runner = gameObject.AddComponent<NetworkRunner>();

    // Client provides input. Dedicated server should not.
  var mode =
  Application.isBatchMode ? GameMode.Server :
  (forceServerInEditor ? GameMode.Server : GameMode.Client);


    _runner.ProvideInput = (mode == GameMode.Client);

    // Needed so Fusion can manage scene loading.
    var sceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>();

    Debug.Log($"Starting Fusion in {mode} mode...");

    // Fusion 2 expects a NetworkSceneInfo, not an int buildIndex
    var sceneRef  = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
    var sceneInfo = new NetworkSceneInfo();
    sceneInfo.AddSceneRef(sceneRef, LoadSceneMode.Single);

    var result = await _runner.StartGame(new StartGameArgs
    {
      GameMode     = mode,
      SessionName  = "desert_local",
      Scene        = sceneInfo,
      SceneManager = sceneManager
    });

    Debug.Log(result.Ok
      ? "Fusion StartGame OK"
      : $"Fusion StartGame FAILED: {result.ShutdownReason}");
  }

  // ---- INetworkRunnerCallbacks (empty for now) ----

  public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) {
    Debug.Log($"✅ Brad joined: {player} | ActivePlayers: {runner.ActivePlayers.Count()}");
  }

  public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }

  public void OnInput(NetworkRunner runner, NetworkInput input) { }
  public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }

  public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
  public void OnConnectedToServer(NetworkRunner runner) { }

  public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }

  public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
  public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }

  public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }

  public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
  public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
  public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }

  public void OnSceneLoadStart(NetworkRunner runner) { }
  public void OnSceneLoadDone(NetworkRunner runner) { }

  public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
  public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

  public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
  public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
}
