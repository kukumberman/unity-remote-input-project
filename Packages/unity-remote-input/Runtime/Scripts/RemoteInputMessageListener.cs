using UnityEngine;

namespace Kukumberman.UnityRemoteInput
{
    public sealed class RemoteInputMessageListener : MonoBehaviour
    {
        [SerializeField]
        private CustomInput _input;

        [SerializeField]
        private RectTransform _cursor;

        [SerializeField]
        private bool _smoothnessEnabled;

        [SerializeField]
        private float _smoothness01;

        private Vector2 _mousePosition;
        private Vector2 _remotePosition;

        private void OnEnable()
        {
            _input.SetActive(true);
        }

        private void OnDisable()
        {
            // prevent MissingReferenceException
            if (_input != null)
            {
                _input.SetActive(false);
            }
        }

        private void Update()
        {
            _input.ResetInputs();

            if (_smoothnessEnabled)
            {
                var t01 = _smoothness01 * Time.deltaTime * 50;
                _mousePosition.x = Mathf.Lerp(_mousePosition.x, _remotePosition.x, t01);
                _mousePosition.y = Mathf.Lerp(_mousePosition.y, _remotePosition.y, t01);
            }
            else
            {
                _mousePosition.x = _remotePosition.x;
                _mousePosition.y = _remotePosition.y;
            }

            _input.SendMousePosition((int)_mousePosition.x, (int)_mousePosition.y);

            _cursor.position = _mousePosition;
        }

        [Message("mousedown")]
        private void MouseDownMessageHandler()
        {
            _input.SendMouseDown();
        }

        [Message("mouseup")]
        private void MouseUpMessageHandler()
        {
            _input.SendMouseUp();
        }

        [Message("position")]
        private void PositionMessageHandler(Vector2 position)
        {
            _remotePosition = position;
        }
    }
}
