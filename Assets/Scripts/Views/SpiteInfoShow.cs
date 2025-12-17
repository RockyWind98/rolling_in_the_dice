using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpiteInfoShow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("OnPointerEnter: " + this.name);
        Vector3 worldPos = this.transform.position;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        Debug.Log($"pos is {screenPos}");

        // 简单决定显示在上方还是下方：若指针在屏幕下半部分，则在上方显示（isUp = true）
        bool isUp = screenPos.y < (Screen.height * 0.5f);

        if (ItemShowSystem.Instance != null)
        {
            ItemShowSystem.Instance.ShowItemInfo(defaultName, defaultDes, isUp, screenPos, 80);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (ItemShowSystem.Instance != null && ItemShowSystem.Instance.showObj != null)
        {
            ItemShowSystem.Instance.showObj.SetActive(false);
        }
    }
}
