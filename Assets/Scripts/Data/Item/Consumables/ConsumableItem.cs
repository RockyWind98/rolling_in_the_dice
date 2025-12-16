using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConsumableTarget
{
    // target must be unique
    Dice,
    // just confirm to use
    Jit
}

public enum ConsumableTargetDice
{
    Any,
    Common,
    Odd,
    Even,
    Wild
}

public class ConsumableItem : GameItem
{
    [Tooltip("target type")]
    [SerializeField] public ConsumableTarget target;
    [Tooltip("image")]
    [SerializeField] public Sprite sprite;
    [Tooltip("target dice type")]
    [SerializeField] public ConsumableTargetDice targetDice;

    public virtual bool Use(GameObject targetObj)
    {
        Debug.Log($"使用了消耗品: {itemName}");
        return false;
    }
}
