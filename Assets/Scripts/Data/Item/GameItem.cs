using UnityEngine;

public enum ItemRarity
{
    Common,
    Rare,
    Epic,
    Legendary
}

public abstract class GameItem : ScriptableObject
{
    [Tooltip("Item ID")]
    [SerializeField] protected string _id;
    [Tooltip("Item Name")]
    [SerializeField] protected string _itemName;
    [Tooltip("Item Rarity")]
    [SerializeField] protected ItemRarity _itemRariry;
    [Tooltip("Item description")]
    [SerializeField] protected string _itemDescription;

    public string id => _id;
    public string itemName => _itemName;
    public string itemDescription => _itemDescription;
    public ItemRarity itemRarity => _itemRariry;
}
