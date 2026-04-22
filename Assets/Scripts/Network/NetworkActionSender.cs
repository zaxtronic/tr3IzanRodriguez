using UnityEngine;
using Item;
using Item.Inventory.Interfaces;

namespace Network
{
    [AddComponentMenu("Network/Network Action Sender")]
    public class NetworkActionSender : MonoBehaviour, IUseItem
    {
        private WsClient ws;
        private SharedStateManager sharedState;

        private void Awake()
        {
            ws = Object.FindFirstObjectByType<WsClient>();
            sharedState = Object.FindFirstObjectByType<SharedStateManager>();
        }

        public void OnUseItem(int index, ItemData data, int amount)
        {
            if (ws == null || !ws.IsConnected) return;
            if (data == null) return;

            var msg = new ActionMessage
            {
                type = "action",
                action = "use_item",
                itemId = data.name,
                itemName = data.ItemName,
                slotIndex = index
            };

            ws.Send(JsonUtility.ToJson(msg));

            sharedState?.IncrementLocalCounter();
        }

        [System.Serializable]
        private class ActionMessage
        {
            public string type;
            public string action;
            public string itemId;
            public string itemName;
            public int slotIndex;
        }
    }
}
