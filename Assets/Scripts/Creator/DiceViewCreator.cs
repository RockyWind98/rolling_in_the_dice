using DG.Tweening;
using UnityEngine;

public class DiceViewCreator : Singleton<DiceViewCreator>
{
    [SerializeField] private DiceView diceViewPrefab;

    public DiceView CreateDiceView(Dice dice, Vector3 position, Quaternion rotation)
    {
        DiceView diceView = Instantiate(diceViewPrefab, position, rotation);
        diceView.SetDice(dice);
        diceView.transform.localScale = Vector3.zero;
        diceView.transform.DOScale(Vector3.one, 0.3f);
        return diceView;
    }
}
