using System;
using UnityEngine;
using UnityEngine.UI;
using Text = UnityEngine.UI.Text;

public sealed class ClickCounterView : MonoBehaviour
{
    public event Action OnClicked;

    [SerializeField]
    private Text _txt;

    [SerializeField]
    private Button _btn;

    private void OnEnable()
    {
        _btn.onClick.AddListener(ButtonClickHandler);
    }

    private void OnDisable()
    {
        _btn.onClick.RemoveListener(ButtonClickHandler);
    }

    public void UpdateVisual(int counter)
    {
        _txt.text = $"{counter}";
    }

    private void ButtonClickHandler()
    {
        if (OnClicked != null)
        {
            OnClicked.Invoke();
        }
    }
}
