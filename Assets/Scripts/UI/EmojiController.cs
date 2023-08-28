using UnityEngine;
using UnityEngine.UI;

public sealed class EmojiController : MonoBehaviour
{
    [SerializeField]
    private Image _selectedImage;

    [SerializeField]
    private EmojiCollectionView _collectionView;

    [SerializeField]
    private Sprite[] _sprites;

    private void Start()
    {
        _collectionView.Create(_sprites);
        _collectionView.OnItemClicked += CollectionView_OnItemClicked;
    }

    private void CollectionView_OnItemClicked(int index)
    {
        _selectedImage.sprite = _sprites[index];
    }
}
