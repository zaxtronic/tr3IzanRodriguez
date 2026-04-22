using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Network
{
    public class WsClient : MonoBehaviour
    {
        public string WsUrl = "ws://37.27.199.54:3000/ws";
        public bool IsConnected => _socket != null && _socket.State == WebSocketState.Open;
        public bool AutoReconnect = true;
        public float ReconnectDelay = 2f;

        private ClientWebSocket _socket;
        private CancellationTokenSource _cts;
        private readonly ConcurrentQueue<string> _incoming = new ConcurrentQueue<string>();
        private bool reconnecting;

        public event System.Action<string> OnMessage;
        public event System.Action OnConnected;
        public event System.Action OnDisconnected;

        public async void Connect()
        {
            if (IsConnected) return;

            reconnecting = false;
            _cts = new CancellationTokenSource();
            _socket = new ClientWebSocket();
            try
            {
                await _socket.ConnectAsync(new Uri(WsUrl), _cts.Token);
                OnConnected?.Invoke();
                _ = ReceiveLoop();
            }
            catch (Exception ex)
            {
                Debug.LogError("WS connect error: " + ex.Message);
                TryReconnect();
            }
        }

        public async void Send(string message)
        {
            if (!IsConnected) return;
            var bytes = Encoding.UTF8.GetBytes(message);
            var seg = new ArraySegment<byte>(bytes);
            try
            {
                await _socket.SendAsync(seg, WebSocketMessageType.Text, true, _cts.Token);
            }
            catch (Exception ex)
            {
                Debug.LogError("WS send error: " + ex.Message);
            }
        }

        private async Task ReceiveLoop()
        {
            var buffer = new byte[4096];
            while (_socket != null && _socket.State == WebSocketState.Open)
            {
                WebSocketReceiveResult result;
                var builder = new StringBuilder();
                do
                {
                    result = await _socket.ReceiveAsync(new ArraySegment<byte>(buffer), _cts.Token);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", _cts.Token);
                        OnDisconnected?.Invoke();
                        TryReconnect();
                        return;
                    }
                    builder.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                } while (!result.EndOfMessage);

                _incoming.Enqueue(builder.ToString());
            }
        }

        private void Update()
        {
            while (_incoming.TryDequeue(out var msg))
            {
                OnMessage?.Invoke(msg);
            }
        }

        private async void OnDestroy()
        {
            try
            {
                if (_socket != null)
                {
                    await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                }
            }
            catch { }
            _cts?.Cancel();
        }

        private async void TryReconnect()
        {
            if (!AutoReconnect || reconnecting) return;
            reconnecting = true;
            OnDisconnected?.Invoke();
            await Task.Delay(TimeSpan.FromSeconds(ReconnectDelay));
            Connect();
        }
    }
}
