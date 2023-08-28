using UnityEngine;
using UnityEngine.EventSystems;

public sealed class DraggableObjectUI : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    private Canvas _canvas;

    private Vector2 _clickedPosition;
    private Vector2 _clickOffset;

    private void Awake()
    {
        _canvas = GetComponentInParent<Canvas>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        // same behaviour 1
        //transform.position += (Vector3)eventData.delta;

        // same behaviour 2
        //(transform as RectTransform).anchoredPosition += eventData.delta / _canvas.scaleFactor;

        var dir = eventData.position - _clickedPosition;
        transform.position = _clickedPosition + dir + _clickOffset;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _clickedPosition = eventData.position;
        _clickOffset = (Vector2)(transform as RectTransform).position - _clickedPosition;
    }
}
