using System;
using UnityEngine;

namespace Network
{
    [AddComponentMenu("Network/Shared State Manager")]
    public class SharedStateManager : MonoBehaviour
    {
        private WsClient ws;
        private NetworkHud hud;
        private float nextPing;

        private int sharedCounter;

        [Serializable]
        private class StateMessage
        {
            public string type = "state";
            public int counter;
        }

        [Serializable]
        private class StateBroadcast
        {
            public string type;
            public int counter;
        }

        private void Awake()
        {
            ws = GetComponent<WsClient>();
            if (ws == null) ws = gameObject.AddComponent<WsClient>();
            hud = GetComponent<NetworkHud>();

            ws.OnMessage += HandleMessage;
        }

        private void Update()
        {
            if (ws == null || !ws.IsConnected) return;
            if (UnityEngine.Time.time < nextPing) return;
            nextPing = UnityEngine.Time.time + 2f;

            // El host (primer jugador) pot fer increments locals i enviar l'estat.
            // Per simplificar, tots els clients envien un ping d'estat amb el seu valor actual.
            var msg = new StateMessage { counter = sharedCounter };
            ws.Send(JsonUtility.ToJson(msg));
        }

        public void IncrementLocalCounter()
        {
            sharedCounter++;
            hud?.SetStatus("Counter local: " + sharedCounter);
            if (ws != null && ws.IsConnected)
            {
                var msg = new StateMessage { counter = sharedCounter };
                ws.Send(JsonUtility.ToJson(msg));
            }
        }

        private void HandleMessage(string json)
        {
            StateBroadcast msg;
            try { msg = JsonUtility.FromJson<StateBroadcast>(json); }
            catch { return; }

            if (msg == null || msg.type != "state") return;

            // Simple: adoptem el màxim per coordinar
            if (msg.counter > sharedCounter) sharedCounter = msg.counter;
            hud?.SetStatus("Counter compartit: " + sharedCounter);
        }

        private void OnDestroy()
        {
            if (ws != null) ws.OnMessage -= HandleMessage;
        }
    }
}
