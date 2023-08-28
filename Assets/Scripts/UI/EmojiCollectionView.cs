using System;
using UnityEngine;

public sealed class EmojiCollectionView : MonoBehaviour
{
    public event Action<int> OnItemClicked;

    [SerializeField]
    private EmojiCollectionItemView _itemPrefab;

    [SerializeField]
    private Transform _itemContainer;

    public void Create(Sprite[] sprites)
    {
        _itemContainer.DestroyChildrens();

        for (int i = 0; i < sprites.Length; i++)
        {
            var item = Instantiate(_itemPrefab, _itemContainer);
            item.OnClicked += Item_OnClicked;
            item.Initialize(i);
            item.UpdateVisual(sprites[i], sprites[i].name);
        }
    }

    private void Item_OnClicked(int index)
    {
        if (OnItemClicked != null)
        {
            OnItemClicked.Invoke(index);
        }
    }
}
