using System.Text;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp.Net;
using WebSocketSharp.Server;
using Newtonsoft.Json;

namespace Kukumberman.UnityRemoteInput
{
    public sealed class HttpServerBehaviour : MonoBehaviour
    {
        delegate void MyAction(HttpRequestEventArgs evt, out bool consume);

        [SerializeField]
        private ushort _port;

        [SerializeField]
        private string _staticRootPath;

        private HttpServer _app;

        private List<MyAction> _handlersHttpGet;

        private void OnEnable()
        {
            _app = new HttpServer($"http://127.0.0.1:{_port}");
            _app.DocumentRootPath = _staticRootPath;

            _handlersHttpGet = new();
            _handlersHttpGet.Add(Cors);
            _handlersHttpGet.Add(ServeStatic);
            _handlersHttpGet.Add(ResponseWithVector2);

            _app.OnGet += (sender, evt) =>
            {
                var consume = false;

                foreach (var handler in _handlersHttpGet)
                {
                    if (!consume)
                    {
                        handler(evt, out consume);
                    }
                }

                if (!consume)
                {
                    evt.Response.StatusCode = (int)HttpStatusCode.NotFound;
                }
            };

            _app.Start();
        }

        private void OnDisable()
        {
            _app.Stop();
            _app = null;
        }

        private static void SendJson(HttpListenerResponse res, object value)
        {
            var encoding = Encoding.UTF8;

            res.StatusCode = (int)HttpStatusCode.OK;
            res.ContentType = "application/json";
            res.ContentEncoding = encoding;

            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(value, settings);
            byte[] contents = encoding.GetBytes(json);

            res.ContentLength64 = contents.LongLength;
            res.Close(contents, true);
        }

        private static void ServeStatic(HttpRequestEventArgs evt, out bool consume)
        {
            consume = false;

            var req = evt.Request;
            var res = evt.Response;

            var path = req.RawUrl;

            if (path == "/")
            {
                path += "index.html";
            }

            if (!evt.TryReadFile(path, out var contents))
            {
                res.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            if (path.EndsWith(".html"))
            {
                res.ContentType = "text/html";
                res.ContentEncoding = Encoding.UTF8;
            }
            else if (path.EndsWith(".js"))
            {
                res.ContentType = "application/javascript";
                res.ContentEncoding = Encoding.UTF8;
            }

            res.ContentLength64 = contents.LongLength;
            res.Close(contents, true);

            consume = true;
        }

        private static void Cors(HttpRequestEventArgs evt, out bool consume)
        {
            consume = false;

            evt.Response.SetHeader("Access-Control-Allow-Origin", "*");
            // "Access-Control-Allow-Methods": "DELETE, POST, GET, OPTIONS",
            // "Access-Control-Allow-Headers": "Origin, X-Requested-With, Content-Type, Accept"
        }

        private void ResponseWithVector2(HttpRequestEventArgs evt, out bool consume)
        {
            consume = false;

            if (evt.Request.RawUrl == "/test")
            {
                SendJson(evt.Response, new Vector2(100, 200));
                consume = true;
            }
        }
    }
}
