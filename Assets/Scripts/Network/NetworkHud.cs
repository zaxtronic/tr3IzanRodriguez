using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Network
{
    [AddComponentMenu("Network/Network HUD")]
    public class NetworkHud : MonoBehaviour
    {
        private TextMeshProUGUI statusText;
        private TextMeshProUGUI playersText;

        public void SetStatus(string message)
        {
            EnsureUI();
            statusText.text = message;
        }

        public void SetPlayers(int localAndRemote)
        {
            EnsureUI();
            playersText.text = "Jugadors: " + localAndRemote;
        }

        private void EnsureUI()
        {
            if (statusText != null) return;

            var canvasGo = new GameObject("NetworkHUD");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGo.AddComponent<CanvasScaler>();
            canvasGo.AddComponent<GraphicRaycaster>();

            var statusGo = new GameObject("Status");
            statusGo.transform.SetParent(canvasGo.transform, false);
            var statusRect = statusGo.AddComponent<RectTransform>();
            statusRect.anchorMin = new Vector2(0f, 1f);
            statusRect.anchorMax = new Vector2(0f, 1f);
            statusRect.pivot = new Vector2(0f, 1f);
            statusRect.anchoredPosition = new Vector2(20f, -20f);
            statusRect.sizeDelta = new Vector2(600f, 80f);

            statusText = statusGo.AddComponent<TextMeshProUGUI>();
            statusText.fontSize = 22;
            statusText.color = Color.white;
            statusText.alignment = TextAlignmentOptions.TopLeft;

            var playersGo = new GameObject("Players");
            playersGo.transform.SetParent(canvasGo.transform, false);
            var playersRect = playersGo.AddComponent<RectTransform>();
            playersRect.anchorMin = new Vector2(0f, 1f);
            playersRect.anchorMax = new Vector2(0f, 1f);
            playersRect.pivot = new Vector2(0f, 1f);
            playersRect.anchoredPosition = new Vector2(20f, -60f);
            playersRect.sizeDelta = new Vector2(300f, 40f);

            playersText = playersGo.AddComponent<TextMeshProUGUI>();
            playersText.fontSize = 20;
            playersText.color = Color.white;
            playersText.alignment = TextAlignmentOptions.TopLeft;
        }
    }
}
