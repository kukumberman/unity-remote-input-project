using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kukumberman.UnityRemoteInput
{
    public sealed class MessageDispatcher : IDisposable
    {
        private readonly List<object> _listeners = new();
        private readonly Dictionary<Type, MethodInfo[]> _methodsByType = new();
        private readonly Dictionary<string, Type> _messageIdToType = new();

        public void Dispose()
        {
            _listeners.Clear();
            _methodsByType.Clear();
            _methodsByType.Clear();
        }

        public bool AddListener(object listener)
        {
            if (_listeners.Contains(listener))
            {
                return false;
            }

            // todo: allow multiple instances of same listener or not?
            _listeners.Add(listener);

            var listenerType = listener.GetType();

            if (_methodsByType.ContainsKey(listenerType))
            {
                return false;
            }

            var methodsWithAttribute = listenerType
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(m => m.GetCustomAttributes().OfType<MessageAttribute>().Any())
                .ToArray();

            var methods = new List<MethodInfo>();

            for (int i = 0; i < methodsWithAttribute.Length; i++)
            {
                var method = methodsWithAttribute[i];
                var parameters = method.GetParameters();

                var attribute = method.GetCustomAttribute<MessageAttribute>();

                if (_messageIdToType.TryGetValue(attribute.Id, out var existingType))
                {
                    Debug.LogWarning(
                        $"Found method duplicate, '{listenerType.Name}.{method.Name}' will not be called"
                    );
                }
                else
                {
                    if (parameters.Length > 1)
                    {
                        Debug.LogWarning(
                            $"Skipping '{listenerType.Name}.{method.Name}', method should have 0 or 1 parameter"
                        );

                        continue;
                    }

                    if (parameters.Length == 0)
                    {
                        _messageIdToType.Add(attribute.Id, null);
                    }
                    else if (parameters.Length == 1)
                    {
                        _messageIdToType.Add(attribute.Id, parameters[0].ParameterType);
                    }

                    methods.Add(method);
                }
            }

            _methodsByType[listenerType] = methods.ToArray();

            return true;
        }

        public bool RemoveListener(object listener)
        {
            return _listeners.Remove(listener);
        }

        public void InvokeMessage(string id, object data)
        {
            for (int i = 0; i < _listeners.Count; i++)
            {
                InvokeMessage(_listeners[i], id, data);
            }
        }

        private void InvokeMessage(object listener, string id, object data)
        {
            if (listener == null)
            {
                return;
            }

            if (id == null)
            {
                return;
            }

            var listenerType = listener.GetType();

            var methods = _methodsByType[listenerType];

            var paramsArray = new object[] { data };

            for (int j = 0; j < methods.Length; j++)
            {
                var method = methods[j];

                var attribute = method.GetCustomAttribute<MessageAttribute>();

                if (attribute.Id != id)
                {
                    continue;
                }

                if (!_messageIdToType.TryGetValue(id, out var messageType))
                {
                    // event handler is not registered
                    continue;
                }

                if (data != null && data.GetType() == messageType)
                {
                    method.Invoke(listener, paramsArray);
                }
                else if (data == null && messageType == null)
                {
                    method.Invoke(listener, null);
                }
                else
                {
                    // unknown signature (e.g registered with 0 params and tried to call with 1 or vice-versa)
                }
            }
        }

        public void TryHandleJsonMessage(string json)
        {
            try
            {
                HandleJsonMessage(json);
            }
            catch (JsonReaderException e)
            {
                Debug.LogWarning(e);
            }
            catch (JsonSerializationException e)
            {
                Debug.LogWarning(e);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private void HandleJsonMessage(string json)
        {
            var message = JsonConvert.DeserializeObject<JObject>(json);

            if (message.TryGetValue("id", out var idValue))
            {
                var id = idValue.ToObject<string>();

                if (_messageIdToType.TryGetValue(id, out var messageType))
                {
                    if (message.TryGetValue("data", out var dataValue))
                    {
                        var data = dataValue.ToObject(messageType);
                        InvokeMessage(id, data);
                    }
                    else
                    {
                        InvokeMessage(id, null);
                    }
                }
                else
                {
                    InvokeMessage(id, null);
                }
            }
        }
    }
}
