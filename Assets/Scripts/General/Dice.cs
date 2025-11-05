using UnityEngine;

public enum DiceType
{
    Common,
    Odd,
    Even,
    Wild
}

[System.Serializable]
public abstract class Dice
{
    public DiceType diceType;

    public virtual void Roll()
    {
        // 默认实现为空，由子类重写
    }
}

[System.Serializable]
public class CommonDice : Dice
{
    public int value;
    public CommonDice()
    {
        // value 取随机 1 到 6 之间的一个值
        value = Random.Range(1, 7);
        diceType = DiceType.Common;
    }

    public override void Roll()
    {
        value = Random.Range(1, 7);
    }
}

[System.Serializable]
public class OddDice : Dice
{
    public int value;
    public OddDice()
    {
        // 从 {1,3,5} 中随机选一个
        int idx = Random.Range(0, 3); // 0,1,2
        value = 1 + idx * 2;
        diceType = DiceType.Odd;
    }

    public override void Roll()
    {
        int idx = Random.Range(0, 3); // 0,1,2
        value = 1 + idx * 2;
    }
}

[System.Serializable]
public class EvenDice : Dice
{
    public int value;
    public EvenDice()
    {
        // 从 {2,4,6} 中随机选一个
        int idx = Random.Range(0, 3); // 0,1,2
        value = 2 + idx * 2;
        diceType = DiceType.Even;
    }

    public override void Roll()
    {
        int idx = Random.Range(0, 3); // 0,1,2
        value = 2 + idx * 2;
    }
}

[System.Serializable]
public class WildDice : Dice
{
    public WildDice()
    {
        diceType = DiceType.Wild;
    }
}
