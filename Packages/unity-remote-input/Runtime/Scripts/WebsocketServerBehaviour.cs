using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;
using WebSocketSharp.Server;

namespace Kukumberman.UnityRemoteInput
{
    public sealed class WebsocketServerBehaviour : MonoBehaviour
    {
        public readonly ConcurrentQueue<KeyValuePair<string, string>> JsonMessageQueue = new();
        public readonly ConcurrentDictionary<string, WebsocketConnectionHandler> Connections =
            new();

        [SerializeField]
        private ushort _port;

        [SerializeField]
        private bool _logIncomingMessages;

        [SerializeField]
        private List<MonoBehaviour> _monoListeners = new();

        private MessageDispatcher _messageDispatcher;

        private WebSocketServer _wss;

        private void OnEnable()
        {
            _messageDispatcher = new();

            for (int i = 0; i < _monoListeners.Count; i++)
            {
                _messageDispatcher.AddListener(_monoListeners[i]);
            }

            _wss = new WebSocketServer($"ws://127.0.0.1:{_port}");
            _wss.AddWebSocketService<WebsocketConnectionHandler>(
                "/",
                (instance) =>
                {
                    instance.SetOwner(this);
                }
            );
            _wss.Start();
        }

        private void OnDisable()
        {
            _wss.Stop();
            _wss = null;

            _messageDispatcher.Dispose();
            _messageDispatcher = null;
        }

        private void Update()
        {
            while (JsonMessageQueue.TryDequeue(out var kvp))
            {
                if (_logIncomingMessages)
                {
                    Debug.Log($"{kvp.Key}: {kvp.Value}");
                }

                _messageDispatcher.TryHandleJsonMessage(kvp.Value);
            }
        }

        public void AddListener(object listener)
        {
            _messageDispatcher.AddListener(listener);
        }

#if UNITY_EDITOR
        [ContextMenu(nameof(BroadcastTestMessage))]
        private void BroadcastTestMessage()
        {
            foreach (var id in Connections.Keys)
            {
                var client = Connections[id];
                Debug.Log(client.ID);
                client.SendText("Hello, World!");
            }
        }
#endif
    }
}
