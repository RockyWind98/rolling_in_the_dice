using UnityEngine;

public enum DiceType
{
    Common,
    Odd,
    Even,
    Wild
}

public enum DiceRarity
{
    Common,
    Rare,
    Epic,
    Legendary
}

[System.Serializable]
public abstract class Dice
{
    public DiceType diceType;
    public DiceRarity diceRarity;
    public int value;
    public int valueMutiplier = 1;
    public int valueBonus = 0;

    public virtual void Roll()
    {
        // 默认实现为空，由子类重写
    }
}

[System.Serializable]
public class CommonDice : Dice
{
    public int num;
    public CommonDice()
    {
        // num 取随机 1 到 6 之间的一个值
        num = Random.Range(1, 7);
        value = (num + valueBonus) * valueMutiplier;
        diceType = DiceType.Common;
        diceRarity = DiceRarity.Common;
    }

    public override void Roll()
    {
        num = Random.Range(1, 7);
        value = (num + valueBonus) * valueMutiplier;
    }
}

[System.Serializable]
public class OddDice : Dice
{
    public int num;
    public OddDice()
    {
        // 从 {1,3,5} 中随机选一个
        int idx = Random.Range(0, 3); // 0,1,2
        num = 1 + idx * 2;
        value = (num + valueBonus) * valueMutiplier;
        //设置骰子的类型和稀有度
        diceType = DiceType.Odd;
        diceRarity = DiceRarity.Common;
    }

    public override void Roll()
    {
        int idx = Random.Range(0, 3); // 0,1,2
        num = 1 + idx * 2;
        value = (num + valueBonus) * valueMutiplier;
    }
}

[System.Serializable]
public class EvenDice : Dice
{
    public int num;
    public EvenDice()
    {
        // 从 {2,4,6} 中随机选一个
        int idx = Random.Range(0, 3); // 0,1,2
        num = 2 + idx * 2;
        value = (num + valueBonus) * valueMutiplier;
        //设置骰子的类型和稀有度
        diceType = DiceType.Even;
        diceRarity = DiceRarity.Rare;
    }

    public override void Roll()
    {
        int idx = Random.Range(0, 3); // 0,1,2
        num = 2 + idx * 2;
        value = (num + valueBonus) * valueMutiplier;
    }
}

[System.Serializable]
public class WildDice : Dice
{
    public WildDice()
    {
        value = (10 + valueBonus) * valueMutiplier;
        //设置骰子的类型和稀有度
        diceType = DiceType.Wild;
        diceRarity = DiceRarity.Legendary;
    }
}
