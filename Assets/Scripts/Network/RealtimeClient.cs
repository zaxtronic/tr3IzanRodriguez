using System;
using System.Collections.Generic;
using Entity_Components;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Network
{
    [AddComponentMenu("Network/Realtime Client")]
    public class RealtimeClient : MonoBehaviour
    {
        [Header("Connexio")]
        [SerializeField] private bool connectOnStart = true;
        [SerializeField] private string wsUrlOverride = "";

        [Header("Enviament")]
        [SerializeField] private float sendInterval = 0.1f;
        [SerializeField] private bool logIncoming = false;

        private WsClient ws;
        private Transform localPlayer;
        private float nextSendTime;
        private readonly Dictionary<string, RemotePlayerView> remotes =
            new Dictionary<string, RemotePlayerView>();
        private NetworkHud hud;

        private string gameId;
        private string userId;
        private string wsUserId;
        private string playerName;
        private Aimer localAimer;

        [Serializable]
        private class BaseMessage { public string type; }

        [Serializable]
        private class PlayerState
        {
            public float x;
            public float y;
            public string name;
            public string dir;
        }

        [Serializable]
        private class PlayerEntry
        {
            public string userId;
            public PlayerState state;
        }

        [Serializable]
        private class SnapshotMessage
        {
            public string type;
            public PlayerEntry[] players;
        }

        [Serializable]
        private class PlayerJoinedMessage
        {
            public string type;
            public string userId;
            public PlayerState state;
        }

        [Serializable]
        private class PlayerLeftMessage
        {
            public string type;
            public string userId;
        }

        [Serializable]
        private class JoinMessage
        {
            public string type = "join";
            public string gameId;
            public string userId;
            public string name;
        }

        [Serializable]
        private class MoveMessage
        {
            public string type = "move";
            public float x;
            public float y;
            public string dir;
        }

        [Serializable]
        private class MoveBroadcastMessage
        {
            public string type;
            public string userId;
            public float x;
            public float y;
            public string dir;
        }

        [Serializable]
        private class ActionBroadcastMessage
        {
            public string type;
            public string userId;
            public string action;
            public string data;
        }

        [Serializable]
        private class ErrorMessage
        {
            public string type;
            public string code;
            public string message;
        }

        private void Awake()
        {
            ws = GetComponent<WsClient>();
            if (ws == null) ws = gameObject.AddComponent<WsClient>();
            hud = GetComponent<NetworkHud>();
        }

        private void Start()
        {
            SceneManager.sceneLoaded += HandleSceneLoaded;
            if (connectOnStart)
            {
                TryConnect();
            }
        }

        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (!connectOnStart) return;
            if (scene.name != "Core") return;
            if (ws != null && ws.IsConnected) return;
            TryConnect();
        }

        public void TryConnect()
        {
            gameId = PlayerPrefs.GetString("game_id", "");
            userId = PlayerPrefs.GetString("auth_user_id", "");
            if (string.IsNullOrEmpty(userId))
            {
                userId = PlayerPrefs.GetString("user_id", "");
            }
            playerName = PlayerPrefs.GetString("player_name", "Player");
            wsUserId = BuildWsUserId(userId);

            if (string.IsNullOrEmpty(gameId) || string.IsNullOrEmpty(wsUserId))
            {
                Debug.LogWarning("RealtimeClient: faltan game_id o user_id. No se conecta.");
                return;
            }

            var wsUrl = string.IsNullOrEmpty(wsUrlOverride)
                ? PlayerPrefs.GetString("ws_url", "ws://37.27.199.54:3000/ws")
                : wsUrlOverride;

            ws.WsUrl = wsUrl;
            ws.OnConnected -= HandleConnected;
            ws.OnDisconnected -= HandleDisconnected;
            ws.OnMessage -= HandleMessage;
            ws.OnConnected += HandleConnected;
            ws.OnDisconnected += HandleDisconnected;
            ws.OnMessage += HandleMessage;
            ws.Connect();
        }

        private void Update()
        {
            if (ws == null || !ws.IsConnected) return;

            if (UnityEngine.Time.time < nextSendTime) return;
            nextSendTime = UnityEngine.Time.time + sendInterval;

            if (localPlayer == null) localPlayer = ResolveLocalPlayer();
            if (localPlayer == null) return;
            if (localAimer == null) localAimer = localPlayer.GetComponent<Aimer>();
            EnsureActionSender(localPlayer.gameObject);

            var pos = localPlayer.position;
            var msg = new MoveMessage
            {
                x = pos.x,
                y = pos.y,
                dir = ResolveDirectionToken()
            };
            ws.Send(JsonUtility.ToJson(msg));
        }

        private Transform ResolveLocalPlayer()
        {
            var tagged = GameObject.FindWithTag("Player");
            if (tagged != null) return tagged.transform;

            var named = GameObject.Find("Player");
            if (named != null) return named.transform;

            return null;
        }

        private void EnsureActionSender(GameObject player)
        {
            if (player == null) return;
            if (player.GetComponent<NetworkActionSender>() != null) return;
            player.AddComponent<NetworkActionSender>();
        }

        private void HandleConnected()
        {
            var join = new JoinMessage
            {
                gameId = gameId,
                userId = wsUserId,
                name = playerName
            };
            ws.Send(JsonUtility.ToJson(join));
            hud?.SetStatus("Connectat a la partida");
        }

        private void HandleDisconnected()
        {
            if (logIncoming) Debug.Log("RealtimeClient: desconectado.");
            hud?.SetStatus("Desconnectat. Reconnectant...");
        }

        private void HandleMessage(string json)
        {
            if (logIncoming) Debug.Log("WS <- " + json);

            BaseMessage baseMsg;
            try
            {
                baseMsg = JsonUtility.FromJson<BaseMessage>(json);
            }
            catch
            {
                return;
            }

            if (baseMsg == null || string.IsNullOrEmpty(baseMsg.type)) return;

            switch (baseMsg.type)
            {
                case "snapshot":
                    HandleSnapshot(json);
                    break;
                case "player-joined":
                    HandlePlayerJoined(json);
                    break;
                case "player-left":
                    HandlePlayerLeft(json);
                    break;
                case "move":
                    HandleMove(json);
                    break;
                case "action":
                    HandleAction(json);
                    break;
                case "error":
                    HandleError(json);
                    break;
            }
        }

        private void HandleSnapshot(string json)
        {
            var msg = JsonUtility.FromJson<SnapshotMessage>(json);
            if (msg?.players == null) return;

            var stillPresent = new HashSet<string>();
            int count = 1;
            for (int i = 0; i < msg.players.Length; i++)
            {
                var p = msg.players[i];
                if (p == null) continue;
                if (p.userId == wsUserId) continue;
                stillPresent.Add(p.userId);
                EnsureRemote(p.userId, p.state);
                count++;
            }

            var staleUserIds = new List<string>();
            foreach (var pair in remotes)
            {
                if (!stillPresent.Contains(pair.Key))
                {
                    staleUserIds.Add(pair.Key);
                }
            }

            for (int i = 0; i < staleUserIds.Count; i++)
            {
                var staleUserId = staleUserIds[i];
                if (remotes.TryGetValue(staleUserId, out var staleView) && staleView != null)
                {
                    Destroy(staleView.gameObject);
                }
                remotes.Remove(staleUserId);
            }

            hud?.SetPlayers(count);
        }

        private void HandlePlayerJoined(string json)
        {
            var msg = JsonUtility.FromJson<PlayerJoinedMessage>(json);
            if (msg == null || string.IsNullOrEmpty(msg.userId) || msg.userId == wsUserId) return;
            EnsureRemote(msg.userId, msg.state);
            hud?.SetStatus($"Jugador connectat: {msg.state?.name ?? msg.userId}");
            hud?.SetPlayers(1 + remotes.Count);
        }

        private void HandlePlayerLeft(string json)
        {
            var msg = JsonUtility.FromJson<PlayerLeftMessage>(json);
            if (msg == null || string.IsNullOrEmpty(msg.userId)) return;
            if (remotes.TryGetValue(msg.userId, out var view) && view != null)
            {
                Destroy(view.gameObject);
            }
            remotes.Remove(msg.userId);
            hud?.SetStatus($"Jugador desconnectat: {msg.userId}");
            hud?.SetPlayers(1 + remotes.Count);
        }

        private void HandleMove(string json)
        {
            var msg = JsonUtility.FromJson<MoveBroadcastMessage>(json);
            if (msg == null || string.IsNullOrEmpty(msg.userId) || msg.userId == wsUserId) return;
            var view = EnsureRemote(msg.userId, null);
            if (view == null) return;
            view.SetNetworkState(msg.x, msg.y, msg.dir);
        }

        private void HandleAction(string json)
        {
            var msg = JsonUtility.FromJson<ActionBroadcastMessage>(json);
            if (msg == null || string.IsNullOrEmpty(msg.userId)) return;
            if (msg.userId == wsUserId) return;
            var actionText = string.IsNullOrEmpty(msg.action) ? "accio" : msg.action;
            hud?.SetStatus($"Accio remota: {actionText} ({msg.userId})");
        }

        private string BuildWsUserId(string baseUserId)
        {
            if (string.IsNullOrWhiteSpace(baseUserId)) return "";
            return $"{baseUserId.Trim()}#{Guid.NewGuid():N}".Substring(0, baseUserId.Trim().Length + 7);
        }

        private void HandleError(string json)
        {
            var msg = JsonUtility.FromJson<ErrorMessage>(json);
            if (msg == null) return;

            if (msg.code == "SESSION_FULL")
            {
                hud?.SetStatus("Partida llena (2/2).");
                return;
            }

            hud?.SetStatus(string.IsNullOrEmpty(msg.message) ? "Error de red" : msg.message);
        }

        private RemotePlayerView EnsureRemote(string remoteUserId, PlayerState state)
        {
            if (remotes.TryGetValue(remoteUserId, out var existing) && existing != null)
            {
                if (state != null) existing.ApplyState(state.x, state.y, state.name, state.dir);
                return existing;
            }

            var go = new GameObject($"RemotePlayer-{remoteUserId}");
            var view = go.AddComponent<RemotePlayerView>();
            var color = Color.HSVToRGB(UnityEngine.Random.value, 0.6f, 0.9f);
            var displayName = state != null && !string.IsNullOrEmpty(state.name) ? state.name : "Player";
            view.Initialize(remoteUserId, displayName, color);
            if (state != null) view.ApplyState(state.x, state.y, state.name, state.dir);

            remotes[remoteUserId] = view;
            return view;
        }

        private string ResolveDirectionToken()
        {
            var direction = localAimer != null ? localAimer.GetAimDirection() : Vector2.zero;

            if (direction.sqrMagnitude < 0.0001f)
            {
                return "down";
            }

            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                return direction.x >= 0f ? "right" : "left";
            }

            return direction.y >= 0f ? "up" : "down";
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
            if (ws == null) return;
            ws.OnConnected -= HandleConnected;
            ws.OnDisconnected -= HandleDisconnected;
            ws.OnMessage -= HandleMessage;
        }
    }
}
