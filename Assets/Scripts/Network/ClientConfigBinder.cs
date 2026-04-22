using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Network
{
    [AddComponentMenu("Network/Client Config Binder")]
    public class ClientConfigBinder : MonoBehaviour
    {
        private const string PlayerNameKey = "player_name";
        private const string GameModeKey = "game_mode";
        private const string MaxPlayersKey = "max_players";

        private TMP_InputField nameInput;

        private void OnEnable()
        {
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
        }

        private void Start()
        {
            BindIfStartMenu();
        }

        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            BindIfStartMenu();
        }

        private void BindIfStartMenu()
        {
            if (SceneManager.GetActiveScene().name != "StartMenu") return;

            var nameRoot = GameObject.Find("TextField_Name");
            if (nameRoot == null) return;

            nameInput = nameRoot.GetComponentInChildren<TMP_InputField>(true);
            if (nameInput == null) return;

            var currentName = PlayerPrefs.GetString(PlayerNameKey, "");
            if (string.IsNullOrEmpty(currentName))
            {
                currentName = "Player";
            }

            nameInput.text = currentName;
            nameInput.onEndEdit.RemoveListener(OnNameEdited);
            nameInput.onEndEdit.AddListener(OnNameEdited);
        }

        private void OnNameEdited(string value)
        {
            var cleaned = string.IsNullOrWhiteSpace(value) ? "Player" : value.Trim();
            PlayerPrefs.SetString(PlayerNameKey, cleaned);
        }

        // Clau reservada per quan afegim camps UI de mode i max jugadors.
        public static void SetGameMode(string mode)
        {
            if (string.IsNullOrWhiteSpace(mode)) return;
            PlayerPrefs.SetString(GameModeKey, mode.Trim());
        }

        public static void SetMaxPlayers(int maxPlayers)
        {
            if (maxPlayers <= 0) return;
            PlayerPrefs.SetInt(MaxPlayersKey, maxPlayers);
        }
    }
}
