using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BtnManger : Singleton<BtnManger>
{
    public GameObject rollBtn;
    public GameObject scoreBtn;
    public GameObject rerollBtn;
    public TMP_Text rerollNumText;

    private int _rerollNum = 5;

    void Start()
    {
        // 场景加载时，确保 State A 是可见的，State B 是隐藏的。
        // 这是初始状态设置。
        rollBtn.SetActive(true);
        scoreBtn.SetActive(false);
        DeskManager.Instance.OnDiceSelect += UpdateRerollBtnState;
        rerollNumText.text = _rerollNum.ToString();
    }

    // 当 Button A 被点击时调用此方法
    public void RollBtnClicked()
    {
        if(DeskManager.Instance ==  null)
        {
            Debug.LogError("DeskManager Instance is null!");
            return;
        }

        rollBtn.SetActive(false);
        scoreBtn.SetActive(true);

        if(DeskManager.Instance.currentState == DeskManager.DeskState.Idle)
        {
            DeskManager.Instance.RollDice();
        }
        else
        {
            Debug.LogWarning("当前状态不允许点击 Roll 按钮");
        }

        _rerollNum = 5;
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

        scoreBtn.SetActive(false);
        rollBtn.SetActive(true);
        rerollBtn.SetActive(false);
        DeskManager.Instance.ClearDeskSettlement();
    }

    public void RerollBtnClicked()
    {
        if (DeskManager.Instance == null)
        {
            Debug.LogError("DeskManager Instance is null!");
            return;
        }

        if(_rerollNum > 0)
        {
            _rerollNum--;
            rerollNumText.text = _rerollNum.ToString();
            DeskManager.Instance.ClearSelectedDice();
            DeskManager.Instance.RollDice();
        }
    }

    private void UpdateRerollBtnState(Dictionary<string, int> dict)
    {
        if (DeskManager.Instance.GetSelectedDiceCount() > 0)
        {
            if (rerollBtn != null && !rerollBtn.activeSelf && _rerollNum > 0)
            {
                rerollBtn.SetActive(true);
            }
        }
        else
        {
            if (rerollBtn != null && rerollBtn.activeSelf)
            {
                rerollBtn.SetActive(false);
            }
        }
    }
}