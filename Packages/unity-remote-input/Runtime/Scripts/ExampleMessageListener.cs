using UnityEngine;

namespace Kukumberman.UnityRemoteInput
{
    public sealed class ExampleMessageListener : MonoBehaviour
    {
        private MessageDispatcher _messageDispatcher = new();

        private void Awake()
        {
            _messageDispatcher.AddListener(this);
            _messageDispatcher.InvokeMessage("test_1", null); // ok
            _messageDispatcher.InvokeMessage("test_1", 42); // skip
            _messageDispatcher.InvokeMessage("test_3", null); // skip
            _messageDispatcher.InvokeMessage("test_3", 42); // skip
            _messageDispatcher.InvokeMessage("test_3", "42"); // ok
        }

        [Message("test_1")]
        private void MessageHandler()
        {
            Debug.Log("MessageHandler");
        }

        [Message("test_1")]
        private void MessageHandlerDuplicate()
        {
            Debug.Log("MessageHandlerDuplicate");
        }

        [Message("test_2")]
        private void MessageHandlerWithRedundantParameters(string a, string b)
        {
            Debug.Log("MessageHandlerWithRedundantParameters");
        }

        [Message("test_3")]
        private void MessageHandlerWithParam(string value)
        {
            Debug.Log($"MessageHandlerWithParam: {value}");
        }
    }
}
