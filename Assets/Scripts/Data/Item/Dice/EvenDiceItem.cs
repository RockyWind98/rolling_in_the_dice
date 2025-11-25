using UnityEngine;

[CreateAssetMenu(menuName = "Game/Item/Dice/EvenDice")]
public class EvenDiceItem : DiceItem
{
    public override void Roll()
    {
        num = Random.Range(1, 4) * 2; // 生成 2、4、6 中的一个偶数
        diceString = num.ToString();
    }

}
