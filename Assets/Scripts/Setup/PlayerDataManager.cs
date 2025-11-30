using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerDataManager : PersistentSingleton<PlayerDataManager>
{
    public GameItemDataBase gameItemDataBase;
    public TextAsset initDataCsv;

    public string[] starterItemIds;

    public List<DiceItem> playerDiceItems;
    public int playerCoins = 0;
    public int playerRerollCount = 0;

    void Start()
    {
        LoadPlayerInitData();
        gameItemDataBase.LoadAllItems();
        gameItemDataBase.Init();
        DicePackage.Instance.DiceStoreInit();
        DicePackage.Instance.ShuffleDice();
        BtnManger.Instance.SetRerollNum(playerRerollCount);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetPlayerCoins()
    {
        return playerCoins;
    }

    public int GetPlayerRerollCount()
    {
        return playerRerollCount;
    }

    public void ChangePlayerCoins(int bonus)
    {
        playerCoins += bonus;
    }

    public void ChangePlayerRerollCount(int bonus)
    {
        playerRerollCount += bonus;
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
                playerRerollCount = int.Parse(entries[0]);
                playerCoins = int.Parse(entries[1]);
                starterItemIds = entries[2].Split(new char[] { ' ' });

                Debug.Log($"Player Init Data Loaded: Rerolls={playerRerollCount}, Coins={playerCoins}, StarterItems={string.Join(", ", starterItemIds)}");

                foreach(string itemId in starterItemIds)
                {
                    GameItem item = gameItemDataBase.GetItemById(itemId);
                    if(item != null && item is DiceItem diceItem)
                    {
                        playerDiceItems.Add(diceItem);
                        //Debug.Log($"Added starter item: {itemId}");
                    }
                    else
                    {
                        Debug.LogWarning($"Starter item ID {itemId} not found or is not a DiceItem.");
                    }
                }
            }
        }
    }
}
