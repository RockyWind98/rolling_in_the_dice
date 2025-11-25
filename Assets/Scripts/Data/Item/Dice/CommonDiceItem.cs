using UnityEngine;

[CreateAssetMenu(menuName = "Game/Item/Dice/CommonDice")]
public class CommonDiceItem : DiceItem
{
    public override void Roll()
    {
        num = Random.Range(1, 7);
        diceString = num.ToString();
    }
}
