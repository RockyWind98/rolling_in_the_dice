using UnityEngine;

[CreateAssetMenu(menuName = "Game/Item/Dice/OddDice")]
public class OddDiceItem : DiceItem
{
    public override void Roll()
    {
        num = Random.Range(0, 3) * 2 + 1; // 生成 1、3、5 中的一个奇数
        diceString = num.ToString();
    }
}
