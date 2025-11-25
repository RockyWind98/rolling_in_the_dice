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

    public string id => _id;
    public string itemName => _itemName;
    public ItemRarity itemRarity => _itemRariry;
}
