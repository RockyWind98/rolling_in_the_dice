using System.Collections.Generic;
using UnityEngine;

public class BtnManger : Singleton<BtnManger>
{
    // 在 Inspector 中拖拽 State A 按钮对象
    public GameObject RollBtn;

    // 在 Inspector 中拖拽 State B 按钮对象
    public GameObject ScoreBtn;

    public event System.Action OnScoreBtnClicked;

    void Start()
    {
        // 场景加载时，确保 State A 是可见的，State B 是隐藏的。
        // 这是初始状态设置。
        RollBtn.SetActive(true);
        ScoreBtn.SetActive(false);
    }

    // 当 Button A 被点击时调用此方法
    public void RollBtnClicked()
    {
        if(DeskManager.Instance ==  null)
        {
            Debug.LogError("DeskManager Instance is null!");
            return;
        }

        RollBtn.SetActive(false);
        ScoreBtn.SetActive(true);

        if(DeskManager.Instance.currentState == DeskManager.DeskState.Idle)
        {
            DeskManager.Instance.RollDice();
        }
        else
        {
            Debug.LogWarning("当前状态不允许点击 Roll 按钮");
        }
    }

    // 当 Button B 被点击时调用此方法
    public void ScoreBtnClicked()
    {
        if (DeskManager.Instance == null)
        {
            Debug.LogError("DeskManager Instance is null!");
            return;
        }

        if(DeskManager.Instance.GetSelectedDiceCount() <= 0)
        {
            Debug.LogWarning("当前状态不允许点击 Score 按钮");
            return;
        }

        ScoreBtn.SetActive(false);
        RollBtn.SetActive(true);
        DeskManager.Instance.ClearSelectedDice();
        OnScoreBtnClicked?.Invoke();
        // 【可选】可以在这里添加执行状态 B 对应的功能代码
    }
}