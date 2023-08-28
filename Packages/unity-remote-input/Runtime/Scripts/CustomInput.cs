using UnityEngine;
using UnityEngine.EventSystems;

namespace Kukumberman.UnityRemoteInput
{
    public sealed class CustomInput : BaseInput
    {
        private bool _mouseDown;
        private bool _mouseUp;

        private Vector2 _mousePosition;

        public override bool mousePresent => true;

        public override Vector2 mousePosition => _mousePosition;

        public override bool GetMouseButtonDown(int button)
        {
            return _mouseDown;
        }

        public override bool GetMouseButtonUp(int button)
        {
            return _mouseUp;
        }

        public void SetActive(bool active)
        {
            var inputModule = GetComponent<BaseInputModule>();
            inputModule.inputOverride = active ? this : null;

            //EventSystem.current.currentInputModule.inputOverride = this;
            //Debug.Log(EventSystem.current.currentInputModule); // null
        }

        public void ResetInputs()
        {
            _mouseDown = false;
            _mouseUp = false;
        }

        public void SendMouseDown()
        {
            _mouseDown = true;
        }

        public void SendMouseUp()
        {
            _mouseUp = true;
        }

        public void SendMousePosition(int x, int y)
        {
            _mousePosition.x = x;
            _mousePosition.y = y;
        }
    }
}
