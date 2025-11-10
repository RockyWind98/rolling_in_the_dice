using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NaviHands : MonoBehaviour
{
    [SerializeField] private TMP_Text handsName;
    [SerializeField] private TMP_Text baseValue;
    [SerializeField] private TMP_Text multiplierValue;

    private int currentBase = 0;
    private int currentMultiplier = 0;
    // Start is called before the first frame update
    void Start()
    {
        baseValue.text = "0";
        multiplierValue.text = "0";
        handsName.text = "未中奖";
        ScoreRules.Instance.OnRuleCount += OnRuleUpdate;
        ScoreRules.Instance.OnTotalCount += OnBaseValueUpdate;
        BtnManger.Instance.OnScoreBtnClicked += InitAllText;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InitAllText()
    {
        UpdateHandsName(name: "未中奖");
        UpdateBaseValue(value: 0);
        UpdateMultiplierValue(value: 0);
    }

    private void OnRuleUpdate(ScoreRule rule)
    {
        UpdateHandsName(rule.ruleName);
        UpdateMultiplierValue(rule.multiplier);
    }

    private void OnBaseValueUpdate(int value)
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
        currentBase = value;
    }

    private void UpdateMultiplierValue(int value) 
    {
        multiplierValue.text = value.ToString();
        currentMultiplier = value;
    }
}
