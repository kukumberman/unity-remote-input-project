using UnityEngine;

public sealed class ClickCounterController : MonoBehaviour
{
    [SerializeField]
    private ClickCounterView _view;

    [SerializeField]
    private int _counter;

    private void OnEnable()
    {
        _view.OnClicked += View_OnClicked;
    }

    private void OnDisable()
    {
        _view.OnClicked -= View_OnClicked;
    }

    private void Start()
    {
        _view.UpdateVisual(_counter);
    }

    private void View_OnClicked()
    {
        _counter += 1;
        _view.UpdateVisual(_counter);
    }
}
