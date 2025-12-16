using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using System;

[RequireComponent(typeof(Collider2D))]
public class DiceView : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
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
            commonDice.diceString = commonDice.num.ToString();
            txt.text = commonDice.diceString;
        }
        else if(dice is OddDice oddDice)
        {
            oddDice.diceString = oddDice.num.ToString();
            txt.text = oddDice.diceString;
            diceBg.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.white, Color.blue, 0.5f);
        }
        else if(dice is EvenDice evenDice)
        {
            evenDice.diceString = evenDice.num.ToString();
            txt.text = evenDice.diceString;
            diceBg.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.white, Color.red, 0.5f);
        }
        else if(dice is WildDice wildDice)
        {
            txt.text = wildDice.diceString;
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

    public void SetDice(Dice d)
    {
        dice = d;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (TargetArrowRenderer.Instance.IsActive())
        {
            if(ConsumableSlot.Instance.activeItem.Use(this.gameObject))
            {
                GlobalManager.Instance.currentSessionData.RemovePlayerConsumableItem(ConsumableSlot.Instance.activeItem);
            }
            ShowDice();
            TargetArrowRenderer.Instance.DisableArrow();
            TargetArrowRenderer.Instance.ClearStopPosition();
        }
        else
        {
            OnDiceClicked?.Invoke(this);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(TargetArrowRenderer.Instance.IsActive())
        {
            TargetArrowRenderer.Instance.SetStopPosition(this.transform.position + new Vector3(0, 0.5f, 0));
            return;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (TargetArrowRenderer.Instance.IsActive())
        {
            TargetArrowRenderer.Instance.ClearStopPosition();
            return;
        }
    }
}