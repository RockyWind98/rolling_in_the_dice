using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NaviHands : MonoBehaviour
{
    [SerializeField] private TMP_Text handsName;
    [SerializeField] private TMP_Text baseValue;
    [SerializeField] private TMP_Text multiplierValue;

    // Start is called before the first frame update
    void Start()
    {
        baseValue.text = "0";
        multiplierValue.text = "0";
        handsName.text = "未中奖";
        ScoreManager.Instance.OnRuleCount += UpdateRuleText;
        ScoreManager.Instance.OnBaseCount += UpdateBaseText;
        ScoreManager.Instance.OnScoreClear += UpdateOnNewRound;
        //BtnManger.Instance.OnScoreBtnClicked += InitAllText;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateOnNewRound(int remainBase)
    {
        UpdateBaseValue(remainBase);
        UpdateMultiplierValue(0);
        UpdateHandsName("未中奖");
    }

    private void UpdateRuleText(ScoreRule rule)
    {
        UpdateHandsName(rule.ruleName);
        UpdateMultiplierValue(rule.multiplier);
    }

    private void UpdateBaseText(int value)
    {
        UpdateBaseValue(value);
    }

    private void UpdateHandsName(string name)
    {
        handsName.text = name;
    }

    private void UpdateBaseValue(int value)
    {
        baseValue.text = value.ToString();
    }

    private void UpdateMultiplierValue(int value) 
    {
        multiplierValue.text = value.ToString();
    }
}
