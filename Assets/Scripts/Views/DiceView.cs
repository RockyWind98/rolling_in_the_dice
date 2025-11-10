using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using System;

[RequireComponent(typeof(Collider2D))]
public class DiceView : MonoBehaviour, IPointerDownHandler
{
    // Start is called before the first frame update
    [SerializeField] private TMP_Text txt;
    [SerializeField] private GameObject diceBg;
    [SerializeField] public Dice dice;
    public event System.Action<DiceView> OnDiceClicked;

    private float upHeight;

    void Start()
    {
        ShowDice();
        upHeight = 0.2f; // fallback
        Collider2D col = this.GetComponent<Collider2D>();
        if (col != null)
        {
            upHeight = col.bounds.size.y;
        }
        else
        {
            SpriteRenderer sr = this.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                upHeight = sr.bounds.size.y;
            }
        }
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

    public void ViewUp()
    {
        // 上移 1.5 倍高度
        Vector3 upTarget = this.transform.position + Vector3.up * (upHeight * 1.0f);

        this.transform.DOMove(upTarget, 0.15f);
    }

    public void ViewDown()
    {
        // 上移 1.5 倍高度
        Vector3 upTarget = this.transform.position - Vector3.up * (upHeight * 1.0f);

        this.transform.DOMove(upTarget, 0.15f);
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
        OnDiceClicked?.Invoke(this);
        Debug.Log($"DiceView clicked, type {dice.diceType}");
    }
}