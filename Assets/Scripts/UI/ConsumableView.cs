using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ConsumableView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public ConsumableItem consumableItem;
    [SerializeField] public Image iconImage;
    [SerializeField] public Sprite defaultSprite;
    [SerializeField] public string defaultName;
    [SerializeField] public string defaultDes;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowView()
    {
        if(consumableItem != null)
        {
            iconImage.sprite = consumableItem.sprite;
        }
        else
        {
            iconImage.sprite = defaultSprite;
        }
    }

    public void SetConsumableItem(ConsumableItem consumableItem)
    {
        this.consumableItem = consumableItem;
        ShowView();
    }

    public void ClearView()
    {
        this.consumableItem = null;
        ShowView();
    }

    // 鼠标移入：显示提示
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(BtnManger.Instance.isCtrlActive == true)
        {
            return;
        }

        // 准备显示的文本（使用 consumableItem 的信息或默认文本）
        string nameToShow = consumableItem != null ? consumableItem.itemName : defaultName;
        string desToShow = consumableItem != null ? consumableItem.itemDescription : defaultDes;

        Vector3 screenPos = this.transform.position;

        // 简单决定显示在上方还是下方：若指针在屏幕下半部分，则在上方显示（isUp = true）
        bool isUp = screenPos.y < (Screen.height * 0.5f);

        Debug.Log($"OnPointerEnter: {nameToShow}, {desToShow}, isUp: {isUp}, screenPos: {screenPos}");
        // 调用提示系统显示
        if (ItemShowSystem.Instance != null)
        {
            ItemShowSystem.Instance.ShowItemInfo(nameToShow, desToShow, isUp, screenPos);
        }
        else
        {
            Debug.LogError("3333333333333333333333");
        }
    }

    // 鼠标移出：隐藏提示
    public void OnPointerExit(PointerEventData eventData)
    {
        if (BtnManger.Instance.isCtrlActive == true)
        {
            return;
        }

        if (ItemShowSystem.Instance != null && ItemShowSystem.Instance.showObj != null)
        {
            ItemShowSystem.Instance.showObj.SetActive(false);
        }
    }

    // 鼠标点击：左键使用物品
    public void OnPointerClick(PointerEventData eventData)
    {
        if (ItemShowSystem.Instance != null && ItemShowSystem.Instance.showObj != null)
        {
            ItemShowSystem.Instance.showObj.SetActive(false);
        }

        if(TargetArrowRenderer.Instance.IsActive())
        {
            return;
        }

        ConsumableSlot.Instance.SetActiveItem(consumableItem);
        ConsumableSlot.Instance.SetActiveView(this);

        if (BtnManger.Instance != null)
        {
            Vector3 screenPos = this.transform.position;
            bool isUp = screenPos.y < (Screen.height * 0.5f);
            BtnManger.Instance.ShowConsumableCtrl(screenPos, isUp);
        }
    }
}
