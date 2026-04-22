using UnityEngine;
using UnityEngine.SceneManagement;

namespace OpenSpec
{
    public static class OpenSpecPowerupSpawner
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SpawnIfNeeded(SceneManager.GetActiveScene());
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SpawnIfNeeded(scene);
        }

        private static void SpawnIfNeeded(Scene scene)
        {
            if (scene.name != "Core") return;
            if (GameObject.Find("OpenSpec Powerups") != null) return;

            var spec = OpenSpecPowerupSpec.LoadFromResources();
            if (spec?.powerups == null || spec.powerups.Length == 0)
            {
                Debug.LogWarning("OpenSpec powerups: no entries found.");
                return;
            }

            var root = new GameObject("OpenSpec Powerups");
            Object.DontDestroyOnLoad(root);

            for (int i = 0; i < spec.powerups.Length; i++)
            {
                var entry = spec.powerups[i];
                if (entry == null) continue;

                var go = new GameObject("Powerup-" + (string.IsNullOrEmpty(entry.id) ? i.ToString() : entry.id));
                go.transform.SetParent(root.transform);
                go.transform.position = new Vector3(entry.x, entry.y, 0f);

                var pickup = go.AddComponent<HealthPowerupPickup>();
                pickup.Configure(entry.id, entry.healAmount, entry.respawnSeconds, entry.radius);
            }

            Debug.Log("OpenSpec powerups loaded: " + spec.powerups.Length);
        }
    }
}
