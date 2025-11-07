using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider2D))]
public class DiceView : MonoBehaviour, IPointerDownHandler
{
    // Start is called before the first frame update
    [SerializeField] private TMP_Text txt;
    [SerializeField] private GameObject diceBg;
    [SerializeField] private Dice dice;

    void Start()
    {
        ShowDice();
    }

    void Update()
    {

    }

    public void ShowDice()
    {
        if(dice is CommonDice commonDice)
        {
            txt.text = commonDice.num.ToString();
        }
        else if(dice is OddDice oddDice)
        {
            txt.text = oddDice.num.ToString();
            diceBg.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.white, Color.blue, 0.5f);
        }
        else if(dice is EvenDice evenDice)
        {
            txt.text = evenDice.num.ToString();
            diceBg.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.white, Color.red, 0.5f);
        }
        else if(dice is WildDice)
        {
            txt.text = "*";
            diceBg.GetComponent<SpriteRenderer>().color = Color.yellow;
        }
    }

    public void RollDice()
    {
        dice.Roll();
        ShowDice();
    }

    public void SetDice(Dice d)
    {
        dice = d;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log($"DiceView clicked, type {dice.diceType}, value {dice.value}");
    }
}