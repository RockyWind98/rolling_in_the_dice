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
    public ItemRarity diceRarity => _diceItem.itemRarity;
    public int num;
    public string diceString;
    public DiceItem _diceItem;

    public virtual void RollDice()
    {

    }
}

[System.Serializable]
public class CommonDice : Dice
{
    public CommonDice(DiceItem item)
    {
        _diceItem = item;
        diceType = DiceType.Common;
    }

    public override void RollDice()
    {
        _diceItem.Roll();
        num = _diceItem.num;
        diceString = _diceItem.num.ToString();
    }
}

[System.Serializable]
public class OddDice : Dice
{
    public OddDice(DiceItem item)
    {
        diceType = DiceType.Odd;
        _diceItem = item;
    }

    public override void RollDice()
    {
        _diceItem.Roll();
        num = _diceItem.num;
        diceString = _diceItem.num.ToString();
    }
}

[System.Serializable]
public class EvenDice : Dice
{
    public EvenDice(DiceItem item)
    {
        diceType = DiceType.Even;
        _diceItem = item;
    }

    public override void RollDice()
    {
        _diceItem.Roll();
        num = _diceItem.num;
        diceString = _diceItem.num.ToString();
    }
}

[System.Serializable]
public class WildDice : Dice
{
    public WildDice(DiceItem item)
    {
        diceType = DiceType.Wild;
        _diceItem = item;
    }

    public override void RollDice()
    {
        _diceItem.Roll();
        num = _diceItem.num;
        diceString = _diceItem.diceString;
    }
}