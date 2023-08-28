using System;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Kukumberman.UnityRemoteInput
{
    public sealed class WebsocketConnectionHandler : WebSocketBehavior
    {
        private WebsocketServerBehaviour _serverBehaviour;

        public void SetOwner(WebsocketServerBehaviour server)
        {
            _serverBehaviour = server;
        }

        public void SendText(string data)
        {
            try
            {
                Send(data);
            }
            catch (InvalidOperationException)
            {
                RemoveConnection();
            }
        }

        protected override void OnOpen()
        {
            _serverBehaviour.Connections.TryAdd(ID, this);
        }

        protected override void OnMessage(MessageEventArgs evt)
        {
            if (evt.IsText)
            {
                var text = evt.Data;
                _serverBehaviour.JsonMessageQueue.Enqueue(new(ID, text));
            }
        }

        protected override void OnClose(CloseEventArgs e)
        {
            RemoveConnection();
        }

        private void RemoveConnection()
        {
            _serverBehaviour.Connections.TryRemove(ID, out var _);
        }
    }
}
