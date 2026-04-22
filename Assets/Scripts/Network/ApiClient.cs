using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Network
{
    public class ApiClient : MonoBehaviour
    {
        public string ApiBaseUrl = "http://37.27.199.54";
        public int RequestTimeoutSeconds = 10;

        public IEnumerator PostJson(string path, string json, System.Action<string> onSuccess, System.Action<string> onError)
        {
            var url = ApiBaseUrl.TrimEnd('/') + path;
            var req = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            req.timeout = RequestTimeoutSeconds;

            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                var detail = req.downloadHandler != null ? req.downloadHandler.text : string.Empty;
                onError?.Invoke(string.IsNullOrEmpty(detail) ? req.error : req.error + " | " + detail);
            }
            else
            {
                onSuccess?.Invoke(req.downloadHandler.text);
            }
        }

        public IEnumerator GetJson(string path, System.Action<string> onSuccess, System.Action<string> onError)
        {
            var url = ApiBaseUrl.TrimEnd('/') + path;
            var req = UnityWebRequest.Get(url);
            req.timeout = RequestTimeoutSeconds;
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                var detail = req.downloadHandler != null ? req.downloadHandler.text : string.Empty;
                onError?.Invoke(string.IsNullOrEmpty(detail) ? req.error : req.error + " | " + detail);
            }
            else
            {
                onSuccess?.Invoke(req.downloadHandler.text);
            }
        }
    }
}
