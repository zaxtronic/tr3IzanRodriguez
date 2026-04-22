using UnityEngine;

namespace Network
{
    public static class NetworkRuntimeInstaller
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            var existing = GameObject.Find("Network Runtime");
            if (existing != null) return;

            var go = new GameObject("Network Runtime");
            Object.DontDestroyOnLoad(go);
            go.AddComponent<NetworkBootstrap>();
            go.AddComponent<WsClient>();
            go.AddComponent<RealtimeClient>();
            go.AddComponent<ClientConfigBinder>();
            go.AddComponent<ResultReporter>();
            go.AddComponent<NetworkHud>();
            go.AddComponent<SharedStateManager>();
        }
    }
}
