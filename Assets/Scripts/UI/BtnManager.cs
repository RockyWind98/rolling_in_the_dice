using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BtnManger : Singleton<BtnManger>
{
    public GameObject rollBtn;
    public GameObject scoreBtn;
    public GameObject rerollBtn;
    public GameObject comsumableItemUseBtn;
    public GameObject comsumableItemDiscardBtn;
    public GameObject comsumableItemCtrl;
    public TMP_Text rerollNumText;
    public TMP_Text scoringCountText;
    public bool isCtrlActive = false;

    private int _rerollRemain = 5;
    private int _scoringRemain = 5;

    void Start()
    {
        // 场景加载时，确保 State A 是可见的，State B 是隐藏的。
        // 这是初始状态设置。
        rollBtn.SetActive(true);
        scoreBtn.SetActive(false);
        comsumableItemCtrl.SetActive(false);
        DeskManager.Instance.OnDiceSelect += UpdateRerollBtnState;
    }

    public void SetScoringCount(int count)
    {
        _scoringRemain = count;
        scoringCountText.text = count.ToString();
    }

    public void SetRerollNum(int num)
    {
        _rerollRemain = num;
        rerollNumText.text = num.ToString();
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

        _rerollRemain = GlobalManager.Instance.currentSessionData.GetPlayerRerollCount();
        rerollNumText.text = _rerollRemain.ToString();
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
        rerollBtn.SetActive(false);
        DeskManager.Instance.ClearDeskSettlement();
        _scoringRemain--;
        scoringCountText.text = _scoringRemain.ToString();

        if(_scoringRemain <= 0)
        {
            // 如果没有剩余的得分机会，设置状态为 GameOver
        }
        else
        {
            rollBtn.SetActive(true);
        }

        Debug.Log("ScoreBtnClicked: scoring remain " + _scoringRemain);
        //todo: check game over
    }

    public void RerollBtnClicked()
    {
        if (DeskManager.Instance == null)
        {
            Debug.LogError("DeskManager Instance is null!");
            return;
        }

        if(_rerollRemain > 0)
        {
            _rerollRemain--;
            rerollNumText.text = _rerollRemain.ToString();
            DeskManager.Instance.ClearSelectedDice();
            DeskManager.Instance.RollDice();
        }

        NoticeSystem.Instance.ShowNotice("Reroll! Remaining: " + _rerollRemain);
    }

    private void UpdateRerollBtnState(Dictionary<string, int> dict)
    {
        if (DeskManager.Instance.GetSelectedDiceCount() > 0)
        {
            if (rerollBtn != null && !rerollBtn.activeSelf && _rerollRemain > 0)
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

    public void HideConsumableCtrl()
    {
        comsumableItemCtrl.SetActive(false);
        isCtrlActive = false;
    }

    public void ShowConsumableCtrl(Vector3 screenPosition, bool isUp)
    {
        // 计算显示位置
        comsumableItemCtrl.transform.position = isUp ? new Vector3(screenPosition.x, screenPosition.y + 110, comsumableItemCtrl.transform.position.z) :
            new Vector3(screenPosition.x, screenPosition.y - 110, comsumableItemCtrl.transform.position.z);

        // 激活显示对象
        comsumableItemCtrl.SetActive(true);
        isCtrlActive = true;
    }

    public void ConsumableItemDiscardClicked()
    {
        comsumableItemCtrl.SetActive(false);
        isCtrlActive = false;
        GlobalManager.Instance.currentSessionData.RemovePlayerConsumableItem(ConsumableSlot.Instance.activeItem);
    }

    public void ConsumableUseClicked()
    {
        TargetArrowRenderer.Instance.EnableArrow(ConsumableSlot.Instance.activeView.transform);
        comsumableItemCtrl.SetActive(false);
        isCtrlActive = false;
    }
}