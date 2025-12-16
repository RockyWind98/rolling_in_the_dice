

using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;
public class GameSessionData
{
    public List<DiceItem> playerDiceItems;
    public int playerCoins = 0;
    public int playerRerollCount = 0;
    public List<ConsumableItem> playerConsumableItems;
    public event System.Action<ConsumableItem> OnConsumableAdd;
    public event System.Action<ConsumableItem> OnConsumableRemove;
    public event System.Action<int> OnPlayerCoinsChange;

    public GameSessionData(List<DiceItem> diceList, List<ConsumableItem> consumableList, int coins, int rerollCnt)
    {
        playerDiceItems = diceList;
        playerCoins = coins;
        playerRerollCount = rerollCnt;
        playerConsumableItems = consumableList;
        if(playerConsumableItems.Count > 3)
        {
            Debug.LogError($"playerConsumableItems.Count more than 3 {playerConsumableItems.Count}");
            playerConsumableItems = null;
        }

        // for test
        GameItem item = GlobalManager.Instance.gameItemDataBase.GetItemById("2000");
        if(item != null && item is ConsumableItem consumableItem)
        {
            AddPlayerConsumableItem(consumableItem);
        }
        else
        {
            Debug.LogError("can't find test consumable item with id 2000");
        }
        //for test end
    }

    public List<DiceItem> GetPlayerDices()
    {
        return playerDiceItems;
    }

    public List<ConsumableItem> GetPlayerConsumables()
    {
        return playerConsumableItems;
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
        OnPlayerCoinsChange?.Invoke(playerCoins);
    }

    public void ChangePlayerRerollCount(int bonus)
    {
        playerRerollCount += bonus;
    }

    public void AddPlayerDiceItem(DiceItem item)
    {
        playerDiceItems.Add(item);
    }

    public void AddPlayerConsumableItem(ConsumableItem item)
    {
        if (playerConsumableItems.Count >= 3)
        {
            Debug.LogWarning("Cannot add more consumable items. Inventory is full.");
            return;
        }
        playerConsumableItems.Add(item);
        OnConsumableAdd?.Invoke(item);
    }

    public void RemovePlayerConsumableItem(ConsumableItem item)
    {
        if (playerConsumableItems.Remove(item))
        {
            OnConsumableRemove?.Invoke(item);
        }
        else
        {
            Debug.LogWarning("Attempted to remove a consumable item that is not in the player's inventory.");
        }
    }
}
