using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Network
{
    [AddComponentMenu("Network/Result Reporter")]
    public class ResultReporter : MonoBehaviour
    {
        [Header("Final de partida")]
        [SerializeField] private KeyCode finishKey = KeyCode.F10;
        [SerializeField] private bool allowFinishKey = true;

        private ApiClient api;
        private TMP_Text overlayText;
        private bool finishedOnce;

        [Serializable]
        private class ResultData
        {
            public string userId;
            public string playerName;
            public int score;
            public float duration;
            public string winnerId;
        }

        [Serializable]
        private class ResultPayload
        {
            public string gameId;
            public ResultData data;
        }

        [Serializable]
        private class ResultRow
        {
            public string id;
            public string gameId;
            public ResultData data;
            public string createdAt;
        }

        [Serializable]
        private class ResultListWrapper
        {
            public ResultRow[] items;
        }

        private void Awake()
        {
            api = GetComponent<ApiClient>();
            if (api == null) api = gameObject.AddComponent<ApiClient>();
        }

        private void Update()
        {
            if (!allowFinishKey) return;
            if (finishedOnce) return;

            if (Input.GetKeyDown(finishKey))
            {
                EndSession();
            }
        }

        public void EndSession()
        {
            if (finishedOnce) return;
            finishedOnce = true;

            StartCoroutine(SendAndShowResults());
        }

        private IEnumerator SendAndShowResults()
        {
            var gameId = PlayerPrefs.GetString("game_id", "");
            var userId = PlayerPrefs.GetString("user_id", "");
            var playerName = PlayerPrefs.GetString("player_name", "Player");

            if (string.IsNullOrEmpty(gameId) || string.IsNullOrEmpty(userId))
            {
                ShowText("No hi ha partida activa per guardar resultats.");
                yield break;
            }

            var data = new ResultData
            {
                userId = userId,
                playerName = playerName,
                score = 0,
                duration = UnityEngine.Time.timeSinceLevelLoad,
                winnerId = userId
            };

            var payload = new ResultPayload { gameId = gameId, data = data };
            var json = JsonUtility.ToJson(payload);

            string postError = null;
            yield return api.PostJson("/api/results", json, _ => { }, e => postError = e);
            if (!string.IsNullOrEmpty(postError))
            {
                ShowText("Error guardant resultats: " + postError);
                yield break;
            }

            string listResponse = null;
            string listError = null;
            yield return api.GetJson($"/api/results/{gameId}", r => listResponse = r, e => listError = e);
            if (!string.IsNullOrEmpty(listError))
            {
                ShowText("Error obtenint resultats: " + listError);
                yield break;
            }

            var wrapperJson = "{\"items\":" + listResponse + "}";
            var wrapper = JsonUtility.FromJson<ResultListWrapper>(wrapperJson);
            ShowResults(wrapper);
        }

        private void ShowResults(ResultListWrapper wrapper)
        {
            if (wrapper == null || wrapper.items == null || wrapper.items.Length == 0)
            {
                ShowText("Resultats buits.");
                return;
            }

            var text = "Resultats de la partida\n";
            for (int i = 0; i < wrapper.items.Length; i++)
            {
                var row = wrapper.items[i];
                if (row?.data == null) continue;
                text += $"- {row.data.playerName}: {row.data.score} punts (durada {row.data.duration:0.0}s)\n";
            }

            ShowText(text.Trim());
        }

        private void ShowText(string message)
        {
            EnsureOverlay();
            overlayText.text = message;
        }

        private void EnsureOverlay()
        {
            if (overlayText != null) return;

            var canvasGo = new GameObject("ResultatsOverlay");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGo.AddComponent<CanvasScaler>();
            canvasGo.AddComponent<GraphicRaycaster>();

            var textGo = new GameObject("Text");
            textGo.transform.SetParent(canvasGo.transform, false);

            var rect = textGo.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = new Vector2(20f, -20f);
            rect.sizeDelta = new Vector2(800f, 400f);

            overlayText = textGo.AddComponent<TextMeshProUGUI>();
            overlayText.fontSize = 24;
            overlayText.color = Color.white;
            overlayText.alignment = TextAlignmentOptions.TopLeft;
        }
    }
}
