using System;
using UnityEngine;
using UnityEngine.UI;
using Text = UnityEngine.UI.Text;

public sealed class EmojiCollectionItemView : MonoBehaviour
{
    public event Action<int> OnClicked;

    [SerializeField]
    private Image _image;

    [SerializeField]
    private Text _txt;

    [SerializeField]
    private Button _btn;

    private int _index;

    private void OnEnable()
    {
        _btn.onClick.AddListener(ButtonClickHandler);
    }

    private void OnDisable()
    {
        _btn.onClick.RemoveListener(ButtonClickHandler);
    }

    private void ButtonClickHandler()
    {
        if (OnClicked != null)
        {
            OnClicked.Invoke(_index);
        }
    }

    public void Initialize(int index)
    {
        _index = index;
    }

    public void UpdateVisual(Sprite sprite, string name)
    {
        _image.sprite = sprite;
        _txt.text = name;
    }
}
