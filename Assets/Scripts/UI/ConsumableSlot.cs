using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsumableSlot : Singleton<ConsumableSlot>
{
    [SerializeField] public List<ConsumableView> slots;
    public ConsumableView activeView;
    public ConsumableItem activeItem;
    private ConsumableItem[] slotItems = new ConsumableItem[3];
    private GameSessionData boundSession;
    private Sprite defaultSprite;

    void Start()
    {
        
    }

    public void SetDefaultSprite(Sprite defaultSprite)
    {
        this.defaultSprite = defaultSprite;
    }

    public void BindToSession(GameSessionData session)
    {
        // 解除旧的订阅
        if (boundSession != null)
        {
            boundSession.OnConsumableAdd -= HandleConsumableAdd;
            boundSession.OnConsumableRemove -= HandleConsumableRemove;
        }

        boundSession = session;

        if (boundSession == null) return;

        // 订阅新的会话事件
        boundSession.OnConsumableAdd += HandleConsumableAdd;
        boundSession.OnConsumableRemove += HandleConsumableRemove;

        // 使用会话当前数据初始化显示（如果已有消耗品）
        var existing = boundSession.GetPlayerConsumables();
        // 清空显示先（已在 Start/初始化过，但再次保证）
        for (int i = 0; i < slotItems.Length; i++)
        {
            if (i < existing.Count && existing[i] != null)
            {
                SetSlotAt(i, existing[i]);
            }
            else
            {
                ClearSlotAt(i);
            }
        }
    }

    public void SetActiveItem(ConsumableItem item)
    {
        activeItem = item;
    }

    public void SetActiveView(ConsumableView view)
    {
        this.activeView = view;
    }

    private void HandleConsumableAdd(ConsumableItem item)
    {
        int index = FindFirstEmptySlot();
        if (index == -1)
        {
            Debug.LogError("ConsumableSlot: no free slot available to add consumable.");
            return;
        }
        SetSlotAt(index, item);
    }

    private void HandleConsumableRemove(ConsumableItem item)
    {
        int index = FindSlotWithItem(item);
        if (index == -1)
        {
            Debug.LogWarning("ConsumableSlot: attempted to remove a consumable not present in any slot.");
            return;
        }
        ClearSlotAt(index);
    }

    private int FindFirstEmptySlot()
    {
        for (int i = 0; i < slotItems.Length; i++)
        {
            if (slotItems[i] == null) return i;
        }
        return -1;
    }

    private int FindSlotWithItem(ConsumableItem item)
    {
        for (int i = 0; i < slotItems.Length; i++)
        {
            if (slotItems[i] == item) return i;
        }
        return -1;
    }

    private void SetSlotAt(int index, ConsumableItem item)
    {
        if (index < 0 || index >= slots.Count) return;
        slotItems[index] = item;
        slots[index].SetConsumableItem(item);
    }

    private void ClearSlotAt(int index)
    {
        if (index < 0 || index >= slots.Count) return;
        slotItems[index] = null;
        slots[index].ClearView();
    }
}
