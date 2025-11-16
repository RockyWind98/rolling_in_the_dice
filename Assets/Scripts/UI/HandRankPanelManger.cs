using UnityEngine;
using UnityEngine.UI; // 如果使用标准UI Button，需要引入此命名空间

public class HandRankingPanelManager : MonoBehaviour
{
    [Header("牌型表面板")]
    public GameObject handRankingPanel; // 关联在Inspector中创建的HandRankingPanel

    [Header("触发按钮")]
    public Button showTableButton; // 关联场景中用于打开牌型表的UI按钮

    void Start()
    {
        // 初始时隐藏牌型表
        if (handRankingPanel != null)
        {
            handRankingPanel.SetActive(false);
        }

        // 为显示牌型表的按钮添加点击监听
        if (showTableButton != null)
        {
            showTableButton.onClick.AddListener(ShowHandRankingTable);
        }

        // 为牌型表面板上的关闭按钮设置监听（也可以在面板的CloseButton上直接绑定）
        // 建议在面板自身上处理关闭逻辑，如下所述
    }

    // 显示牌型表的方法
    public void ShowHandRankingTable()
    {
        if (handRankingPanel != null)
        {
            Debug.Log("Showing Hand Ranking Table");
            handRankingPanel.SetActive(true);
        }
    }

    // 隐藏牌型表的方法
    public void HideHandRankingTable()
    {
        if (handRankingPanel != null)
        {
            handRankingPanel.SetActive(false);
        }
    }
}