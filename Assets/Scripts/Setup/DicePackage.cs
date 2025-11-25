using System.Collections.Generic;
using UnityEngine;

public class DicePackage : Singleton<DicePackage>
{
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

    public void DiceStoreInit()
    {
        if(PlayerDataManager.Instance == null)
        {
            Debug.LogError("PlayerDataManager Instance is null!");
            return;
        }
        if (diceStore.Count <= 0)
        {
            foreach (string itemId in PlayerDataManager.Instance.starterItemIds)
            {
                GameItem item = PlayerDataManager.Instance.gameItemDataBase.GetItemById(itemId);
                if (item is DiceItem diceItem)
                {
                    if(diceItem is CommonDiceItem)
                    {
                        diceStore.Add(new CommonDice(diceItem));
                    }
                    else if (diceItem is OddDiceItem)
                    {
                        diceStore.Add(new OddDice(diceItem));
                    }
                    else if (diceItem is EvenDiceItem)
                    {
                        diceStore.Add(new EvenDice(diceItem));
                    }
                    else if (diceItem is WildDiceItem)
                    {
                        diceStore.Add(new WildDice(diceItem));
                    }
                }
                else
                {
                    Debug.LogWarning($"未找到物品ID对应的骰子：{itemId}");
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

        return result;
    }

    public void DiscardDice(Dice dice)
    {
        diceDiscard.Add(dice);
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
