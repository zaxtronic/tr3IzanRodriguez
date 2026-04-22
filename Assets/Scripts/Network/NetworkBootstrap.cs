using System.Collections;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Network
{
    public class NetworkBootstrap : MonoBehaviour
    {
        public string ApiBaseUrl = "http://37.27.199.54:3000";
        public string WsUrl = "ws://37.27.199.54:3000/ws";
        public int MaxPlayers = 2;
        public string Mode = "realtime";
        public string DefaultPlayerName = "Player";
        [SerializeField] private bool showCoopWindow = true;

        private ApiClient api;
        private Button newGameButton;
        private Button loadGameButton;
        private string playerName;
        private string loginPassword = "";
        private bool isStartingSession;
        private string menuStatus = "";
        private float requestStartedAt = -1f;
        private string currentUserId = "";
        private const string ClientInstanceKey = "client_instance_id";
        private const string RoomIdKey = "room_id";
        private string roomIdInput = "";

        private void Awake()
        {
            EnsureApiClient();

            var modePref = PlayerPrefs.GetString("game_mode", "");
            if (!string.IsNullOrEmpty(modePref)) Mode = modePref;

            var maxPlayersPref = PlayerPrefs.GetInt("max_players", 0);
            if (maxPlayersPref > 0) MaxPlayers = maxPlayersPref;

            playerName = PlayerPrefs.GetString("player_name", "");
            if (string.IsNullOrEmpty(playerName))
            {
                playerName = DefaultPlayerName + UnityEngine.Random.Range(1000, 9999);
                PlayerPrefs.SetString("player_name", playerName);
            }
            currentUserId = PlayerPrefs.GetString("user_id", "");
            roomIdInput = PlayerPrefs.GetString(RoomIdKey, "");
            if (!string.IsNullOrEmpty(currentUserId) && !currentUserId.Contains(":"))
            {
                currentUserId = BuildRealtimeUserId(currentUserId);
                PlayerPrefs.SetString("user_id", currentUserId);
                PlayerPrefs.Save();
            }

            newGameButton = FindButton("Button New Game") ?? FindButtonByLabel("new", "nuevo", "nueva", "host", "crear");
            loadGameButton = FindButton("Button Load Game") ?? FindButtonByLabel("load", "join", "unirse", "continuar");

            if (newGameButton != null) newGameButton.onClick.AddListener(OnNewGame);
            if (loadGameButton != null) loadGameButton.onClick.AddListener(OnLoadGame);
        }

        private bool EnsureApiClient()
        {
            if (api == null)
            {
                api = GetComponent<ApiClient>();
                if (api == null) api = gameObject.AddComponent<ApiClient>();
            }

            if (api == null)
            {
                menuStatus = "Error interno: ApiClient no disponible";
                return false;
            }

            api.ApiBaseUrl = ApiBaseUrl;
            return true;
        }

        private Button FindButton(string name)
        {
            var go = GameObject.Find(name);
            if (go == null) return null;
            return go.GetComponent<Button>();
        }

        private Button FindButtonByLabel(params string[] tokens)
        {
            if (tokens == null || tokens.Length == 0) return null;
            var buttons = UnityEngine.Object.FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (int i = 0; i < buttons.Length; i++)
            {
                var label = ExtractButtonLabel(buttons[i]);
                if (string.IsNullOrEmpty(label)) continue;
                var lower = label.ToLowerInvariant();
                for (int t = 0; t < tokens.Length; t++)
                {
                    if (lower.Contains(tokens[t])) return buttons[i];
                }
            }
            return null;
        }

        private string ExtractButtonLabel(Button button)
        {
            if (button == null) return null;
            var text = button.GetComponentInChildren<Text>(true);
            if (text != null && !string.IsNullOrEmpty(text.text)) return text.text;
            var tmp = button.GetComponentInChildren<TMP_Text>(true);
            if (tmp != null && !string.IsNullOrEmpty(tmp.text)) return tmp.text;
            return button.name;
        }

        private void OnGUI()
        {
            if (!showCoopWindow) return;
            if (SceneManager.GetActiveScene().name == "Core") return;

            GUILayout.BeginArea(new Rect(20, 20, 360, 260), "Co-op Multiplayer", GUI.skin.window);
            GUILayout.Label("Nombre");
            playerName = GUILayout.TextField(playerName ?? "", 24);
            GUILayout.Label("Password (opcional)");
            loginPassword = GUILayout.PasswordField(loginPassword ?? "", '*', 24);
            GUILayout.Label("ID sala (para unirse)");
            roomIdInput = GUILayout.TextField(roomIdInput ?? "", 64);
            GUI.enabled = !isStartingSession;
            if (GUILayout.Button("Login")) OnLogin();
            if (GUILayout.Button("Host co-op (Nueva partida)")) OnNewGame();
            if (GUILayout.Button("Unirse a co-op")) OnLoadGame();
            GUI.enabled = true;
            if (!string.IsNullOrEmpty(menuStatus)) GUILayout.Label(menuStatus);
            GUILayout.EndArea();
        }

        private void Update()
        {
            if (!isStartingSession) return;
            if (requestStartedAt < 0f) return;
            if (UnityEngine.Time.unscaledTime - requestStartedAt < 15f) return;

            isStartingSession = false;
            requestStartedAt = -1f;
            menuStatus = "Timeout de red (15s). Reintenta.";
        }

        private void OnNewGame()
        {
            if (isStartingSession) return;
            if (string.IsNullOrEmpty(currentUserId))
            {
                menuStatus = "Haz login primero";
                return;
            }
            if (!EnsureApiClient()) return;
            menuStatus = "Creando partida...";
            requestStartedAt = UnityEngine.Time.unscaledTime;
            StartCoroutine(CreateAndStart());
        }

        private void OnLoadGame()
        {
            if (isStartingSession) return;
            if (string.IsNullOrEmpty(currentUserId))
            {
                menuStatus = "Haz login primero";
                return;
            }
            if (!EnsureApiClient()) return;
            menuStatus = string.IsNullOrWhiteSpace(roomIdInput) ? "Buscando partida..." : "Uniendose por ID...";
            requestStartedAt = UnityEngine.Time.unscaledTime;
            if (!string.IsNullOrWhiteSpace(roomIdInput))
            {
                StartCoroutine(JoinById(roomIdInput.Trim()));
            }
            else
            {
                StartCoroutine(JoinFirstOpen());
            }
        }

        private void OnLogin()
        {
            if (isStartingSession) return;
            if (!EnsureApiClient()) return;
            if (string.IsNullOrWhiteSpace(playerName))
            {
                menuStatus = "Nombre requerido";
                return;
            }

            menuStatus = "Iniciando sesion...";
            requestStartedAt = UnityEngine.Time.unscaledTime;
            StartCoroutine(LoginOnly());
        }

        private IEnumerator LoginOnly()
        {
            isStartingSession = true;
            string loginJson = JsonUtility.ToJson(new LoginRequest { name = playerName.Trim(), password = loginPassword });
            string userResponse = null;
            yield return api.PostJson("/api/users/login", loginJson, r => userResponse = r, e => { Debug.LogError(e); menuStatus = "Login fallo: " + e; });
            if (string.IsNullOrEmpty(userResponse))
            {
                menuStatus = "Error de red al iniciar sesion";
                isStartingSession = false;
                requestStartedAt = -1f;
                yield break;
            }

            var user = JsonUtility.FromJson<UserResponse>(userResponse);
            if (user == null || string.IsNullOrEmpty(user.id))
            {
                menuStatus = "Login invalido";
                isStartingSession = false;
                requestStartedAt = -1f;
                yield break;
            }

            currentUserId = BuildRealtimeUserId(user.id);
            PlayerPrefs.SetString("auth_user_id", user.id);
            playerName = user.name;
            PlayerPrefs.SetString("user_id", currentUserId);
            PlayerPrefs.SetString("player_name", playerName);
            PlayerPrefs.Save();
            menuStatus = "Login OK";
            isStartingSession = false;
            requestStartedAt = -1f;
        }

        private IEnumerator CreateAndStart()
        {
            if (!EnsureApiClient())
            {
                isStartingSession = false;
                requestStartedAt = -1f;
                yield break;
            }

            isStartingSession = true;
            string name = playerName;
            if (string.IsNullOrEmpty(name))
            {
                isStartingSession = false;
                yield break;
            }
            var user = new UserResponse { id = currentUserId, name = name };

            string gameJson = JsonUtility.ToJson(new CreateGameRequest { hostUserId = user.id, hostName = user.name, maxPlayers = MaxPlayers, mode = Mode });
            string gameResponse = null;
            yield return api.PostJson("/api/games", gameJson, r => gameResponse = r, e => { Debug.LogError(e); menuStatus = "Crear fallo: " + e; });
            if (string.IsNullOrEmpty(gameResponse))
            {
                menuStatus = "No se pudo crear la partida";
                isStartingSession = false;
                yield break;
            }
            var game = JsonUtility.FromJson<GameResponse>(gameResponse);
            if (game == null || string.IsNullOrEmpty(game.id))
            {
                menuStatus = "Respuesta de partida invalida";
                isStartingSession = false;
                requestStartedAt = -1f;
                yield break;
            }

            string joinJson = JsonUtility.ToJson(new JoinGameRequest { userId = user.id, name = user.name });
            var joinOk = false;
            yield return api.PostJson($"/api/games/{game.id}/join", joinJson, _ => joinOk = true, e => { Debug.LogError(e); menuStatus = "Join fallo: " + e; });
            if (!joinOk)
            {
                menuStatus = "No se pudo unir al host";
                isStartingSession = false;
                requestStartedAt = -1f;
                yield break;
            }

            menuStatus = "Entrando a la partida...";
            roomIdInput = game.id;
            PlayerPrefs.SetString(RoomIdKey, roomIdInput);
            SaveSession(user, game);
            SceneManager.LoadScene("Core");
            isStartingSession = false;
            requestStartedAt = -1f;
        }

        private IEnumerator JoinById(string gameId)
        {
            if (!EnsureApiClient())
            {
                isStartingSession = false;
                requestStartedAt = -1f;
                yield break;
            }

            isStartingSession = true;
            string name = playerName;
            if (string.IsNullOrEmpty(name))
            {
                isStartingSession = false;
                yield break;
            }
            var user = new UserResponse { id = currentUserId, name = name };

            string joinJson = JsonUtility.ToJson(new JoinGameRequest { userId = user.id, name = user.name });
            string joinResponse = null;
            yield return api.PostJson($"/api/games/{gameId}/join", joinJson, r => joinResponse = r, e => { Debug.LogError(e); menuStatus = "Join fallo: " + e; });

            if (string.IsNullOrEmpty(joinResponse))
            {
                menuStatus = "No se pudo unir a la sala " + gameId;
                isStartingSession = false;
                requestStartedAt = -1f;
                yield break;
            }

            var game = JsonUtility.FromJson<GameResponse>(joinResponse);
            if (game == null || string.IsNullOrEmpty(game.id))
            {
                menuStatus = "Respuesta de sala invalida";
                isStartingSession = false;
                requestStartedAt = -1f;
                yield break;
            }

            menuStatus = "Entrando a la partida...";
            roomIdInput = game.id;
            PlayerPrefs.SetString(RoomIdKey, roomIdInput);
            SaveSession(user, game);
            SceneManager.LoadScene("Core");
            isStartingSession = false;
            requestStartedAt = -1f;
        }

        private IEnumerator JoinFirstOpen()
        {
            if (!EnsureApiClient())
            {
                isStartingSession = false;
                requestStartedAt = -1f;
                yield break;
            }

            isStartingSession = true;
            string name = playerName;
            if (string.IsNullOrEmpty(name))
            {
                isStartingSession = false;
                yield break;
            }
            var user = new UserResponse { id = currentUserId, name = name };

            string gamesResponse = null;
            yield return api.GetJson("/api/games", r => gamesResponse = r, e => { Debug.LogError(e); menuStatus = "Listar fallo: " + e; });
            if (string.IsNullOrEmpty(gamesResponse))
            {
                menuStatus = "No se pudo obtener partidas";
                isStartingSession = false;
                yield break;
            }

            var list = JsonUtility.FromJson<GameListWrapper>("{\"items\":" + gamesResponse + "}");
            if (list.items == null || list.items.Length == 0)
            {
                Debug.LogWarning("No open games");
                menuStatus = "No hay partidas abiertas";
                isStartingSession = false;
                yield break;
            }

            GameResponse game = null;
            GameResponse emptyGame = null;
            DateTime bestWithPlayers = DateTime.MinValue;
            DateTime bestEmpty = DateTime.MinValue;
            for (int i = 0; i < list.items.Length; i++)
            {
                var candidate = list.items[i];
                if (candidate == null) continue;
                if (candidate.status != "open") continue;

                int currentPlayers = candidate.players != null ? candidate.players.Length : 0;
                int maxPlayers = candidate.maxPlayers > 0 ? candidate.maxPlayers : 2;

                if (currentPlayers >= maxPlayers) continue;

                DateTime createdAt = DateTime.MinValue;
                if (!string.IsNullOrEmpty(candidate.createdAt))
                {
                    DateTime.TryParse(candidate.createdAt, out createdAt);
                }

                if (currentPlayers > 0)
                {
                    if (game == null || createdAt >= bestWithPlayers)
                    {
                        game = candidate;
                        bestWithPlayers = createdAt;
                    }
                }
                else
                {
                    if (emptyGame == null || createdAt >= bestEmpty)
                    {
                        emptyGame = candidate;
                        bestEmpty = createdAt;
                    }
                }
            }

            if (game == null) game = emptyGame;

            if (game == null)
            {
                Debug.LogWarning("No open games with free slots");
                menuStatus = "No hay huecos libres";
                isStartingSession = false;
                yield break;
            }

            string joinJson = JsonUtility.ToJson(new JoinGameRequest { userId = user.id, name = user.name });
            var joinOk = false;
            yield return api.PostJson($"/api/games/{game.id}/join", joinJson, _ => joinOk = true, e => { Debug.LogError(e); menuStatus = "Join fallo: " + e; });
            if (!joinOk)
            {
                menuStatus = "No se pudo unir a la partida";
                isStartingSession = false;
                requestStartedAt = -1f;
                yield break;
            }

            menuStatus = "Entrando a la partida...";
            roomIdInput = game.id;
            PlayerPrefs.SetString(RoomIdKey, roomIdInput);
            SaveSession(user, game);
            SceneManager.LoadScene("Core");
            isStartingSession = false;
            requestStartedAt = -1f;
        }

        private void SaveSession(UserResponse user, GameResponse game)
        {
            // Keep the runtime-unique ID (authId:clientInstance) used by realtime WS.
            PlayerPrefs.SetString("user_id", currentUserId);
            PlayerPrefs.SetString("auth_user_id", user.id);
            PlayerPrefs.SetString("player_name", user.name);
            PlayerPrefs.SetString("game_id", game.id);
            PlayerPrefs.SetString("ws_url", WsUrl);
            PlayerPrefs.Save();
        }

        private string BuildRealtimeUserId(string baseUserId)
        {
            if (string.IsNullOrWhiteSpace(baseUserId)) return "";
            var cleanBase = baseUserId.Trim();
            if (cleanBase.Contains(":")) return cleanBase;

            var clientInstance = PlayerPrefs.GetString(ClientInstanceKey, "");
            if (string.IsNullOrEmpty(clientInstance))
            {
                clientInstance = Guid.NewGuid().ToString("N").Substring(0, 8);
                PlayerPrefs.SetString(ClientInstanceKey, clientInstance);
            }

            return cleanBase + ":" + clientInstance;
        }

        [System.Serializable]
        private class LoginRequest { public string name; public string password; }

        [System.Serializable]
        private class CreateGameRequest { public string hostUserId; public string hostName; public int maxPlayers; public string mode; }

        [System.Serializable]
        private class JoinGameRequest { public string userId; public string name; }

        [System.Serializable]
        private class UserResponse { public string id; public string name; }

        [System.Serializable]
        private class GameResponse
        {
            public string id;
            public string hostUserId;
            public string status;
            public int maxPlayers;
            public string createdAt;
            public JoinGameRequest[] players;
        }

        [System.Serializable]
        private class GameListWrapper { public GameResponse[] items; }
    }
}
