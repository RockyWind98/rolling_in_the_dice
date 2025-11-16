using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RuleView : MonoBehaviour
{
    [SerializeField] private TMP_Text RuleNameTxt;
    [SerializeField] private TMP_Text RuleContentTxt;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetRuleView(ScoreRule rule)
    {
        RuleNameTxt.text = rule.ruleName;
        RuleContentTxt.text = rule.description;
    }
}
