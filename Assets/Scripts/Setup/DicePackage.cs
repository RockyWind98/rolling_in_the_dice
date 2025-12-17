using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DicePackage : Singleton<DicePackage>
{
    public TMP_Text storeNum;
    public TMP_Text discardNum;
    public List<Dice> diceStore = new List<Dice>();
    public List<Dice> diceDiscard = new List<Dice>();
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DiceStoreInit(List<DiceItem> playerDiceItems)
    {
        if(GlobalManager.Instance == null)
        {
            Debug.LogError("GlobalManager Instance is null!");
            return;
        }
        if (diceStore.Count <= 0)
        {
            if(playerDiceItems == null || playerDiceItems.Count <= 0)
            {
                Debug.LogError("玩家骰子物品列表为空，无法初始化骰子牌库！");
                return;
            }

            foreach (DiceItem item in playerDiceItems)
            {
                if(item is CommonDiceItem)
                {
                    diceStore.Add(new CommonDice(item));
                }
                else if (item is OddDiceItem)
                {
                    diceStore.Add(new OddDice(item));
                }
                else if (item is EvenDiceItem)
                {
                    diceStore.Add(new EvenDice(item));
                }
                else if (item is WildDiceItem)
                {
                    diceStore.Add(new WildDice(item));
                }
                else
                {
                    Debug.LogError("unknown dice type!");
                    return;
                }
            }
        }
    }

    public Dice GetDice()
    {
        if(diceStore.Count <= 0)
        {
            Debug.Log("牌库空了，尝试从弃牌堆补充牌库");
            // 抽出 diceDiscard 中的所有骰子，打乱顺序后放回 diceStore 中
            if (diceDiscard.Count > 0)
            {
                // 将打乱后的弃牌堆全部移回牌库并清空弃牌堆
                diceStore.AddRange(diceDiscard);
                diceDiscard.Clear();
                Debug.Log("补充牌库成功");
                ShuffleDice();
            }
            else
            {
                Debug.Log("牌库为全空！！");
                return null;
            }
        }

        // 从牌库取出一颗骰子并返回
        Dice result = diceStore[0];
        diceStore.RemoveAt(0);
        result.RollDice();
        if (storeNum != null)
        {
            storeNum.text = diceStore.Count.ToString();
        }

        if (discardNum != null)
        {
            discardNum.text = diceDiscard.Count.ToString();
        }

        return result;
    }

    public void DiscardDice(Dice dice)
    {
        diceDiscard.Add(dice);
        if (discardNum != null)
        {
            discardNum.text = diceDiscard.Count.ToString();
        }
    }

    public void ShuffleDice()
    {
        // Fisher–Yates shuffle
        for (int i = diceStore.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, diceStore.Count);
            Dice tmp = diceStore[i];
            diceStore[i] = diceStore[j];
            diceStore[j] = tmp;
        }
        for (int i = diceStore.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, diceStore.Count);
            Dice tmp = diceStore[i];
            diceStore[i] = diceStore[j];
            diceStore[j] = tmp;
        }
    }
}
