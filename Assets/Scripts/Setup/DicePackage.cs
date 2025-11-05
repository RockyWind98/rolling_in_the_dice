using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DicePackage : Singleton<DicePackage>
{
    public List<Dice> diceStore = new List<Dice>();
    public List<Dice> diceDiscard = new List<Dice>();
    // Start is called before the first frame update
    void Start()
    {
        TestInit();
        ShuffleDice();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TestInit()
    {
        if (diceStore.Count <= 0)
        {
            for (int i = 0; i < 5; i++)
            {
                diceStore.Add(new CommonDice());
            }

            for (int i = 0; i < 3; i++)
            {
                diceStore.Add(new OddDice());
            }

            for (int i = 0; i < 3; i++)
            {
                diceStore.Add(new EvenDice());
            }

            diceStore.Add(new WildDice());
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
                ShuffleDice();
                // 将打乱后的弃牌堆全部移回牌库并清空弃牌堆
                diceStore.AddRange(diceDiscard);
                diceDiscard.Clear();
                Debug.Log("补充牌库成功");
            }
            else
            {
                Debug.Log("牌库为全空！！");
                return null;
            }
        }

        // 从牌库取出一颗骰子并返回
        Dice result = diceStore[0];
        diceDiscard.Add(result);
        diceStore.RemoveAt(0);
        return result;
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
