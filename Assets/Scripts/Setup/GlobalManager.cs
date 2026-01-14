using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalManager : PersistentSingleton<GlobalManager>
{
    public GameItemDataBase gameItemDataBase;
    public GameSessionData currentSessionData;
    public TextAsset initDataCsv;

    protected override void Awake()
    {
        base.Awake();
        gameItemDataBase.LoadAllItems();
        gameItemDataBase.Init();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitialGameSession()
    {
        if(currentSessionData == null)
        {
            LoadPlayerInitData();
        }

        DicePackage.Instance.DiceStoreInit(currentSessionData.playerDiceItems);
        DicePackage.Instance.ShuffleDice();
        BtnManger.Instance.SetRerollNum(currentSessionData.playerRerollCount);

        if (ConsumableSlot.Instance != null)
        {
            if (ConsumableSlot.Instance.slots.Count != 0)
            {
                var image = ConsumableSlot.Instance.slots[0].GetComponentInChildren<UnityEngine.UI.Image>();
                if (image != null)
                {
                    ConsumableSlot.Instance.SetDefaultSprite(image.sprite);
                }
                else
                {
                    Debug.LogError("ConsumableSlot first slot Image component is null!");
                }
            }

            ConsumableSlot.Instance.BindToSession(currentSessionData);
        }
        else
        {
            Debug.LogError("ConsumableSlot Instance is null!");
        }
    }

    public void QuitToMain()
    {
        currentSessionData = null;
    }

    private void LoadPlayerInitData()
    {
        string[] data = initDataCsv.text.Split(new char[] { '\n' });
        int lineCount = 0;

        if(data.Length != 2)
        {
            Debug.LogError($"Player Init Data CSV is empty or missing data lines {data.Length}.");
            return;
        }

        foreach (string line in data)
        {
            if(lineCount == 0)
            {
                lineCount++;
                continue; // skip header
            }

            string[] entries = line.Split(new char[] { ',' });
            if (entries.Length != 3)
            {
                Debug.LogError($"break because of a corrupt line{lineCount}: {entries.Length}");
                break;
            }
            else
            {
                int rerollCnt = int.Parse(entries[0]);
                int coins = int.Parse(entries[1]);
                List<DiceItem> diceList = new List<DiceItem>();
                List<ConsumableItem> consumableList = new List<ConsumableItem>();
                string[] starterItemIds = entries[2].Split(new char[] { ' ' });

                //Debug.Log($"Player Init Data Loaded: Rerolls={playerRerollCount}, Coins={playerCoins}, StarterItems={string.Join(", ", starterItemIds)}");

                foreach(string itemId in starterItemIds)
                {
                    GameItem item = gameItemDataBase.GetItemById(itemId);
                    if(item != null && item is DiceItem diceItem)
                    {
                        diceList.Add(diceItem);
                        //Debug.Log($"Added starter item: {itemId}");
                    } 
                    else
                    {
                        Debug.LogWarning($"Starter item ID {itemId} not found or is not a DiceItem.");
                    }
                }
                currentSessionData = new GameSessionData(diceList, consumableList, coins, rerollCnt);
            }
        }
    }
}
