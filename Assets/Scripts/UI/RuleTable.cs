using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.UI;

public class RuleTable : MonoBehaviour
{
    public GameObject ruleTable;
    public GameObject ruleView;
    public GameObject handRankingPanel;
    public List<RuleView> ruleViews = new List<RuleView>();
    public Button closeBtn;

    // Start is called before the first frame update
    void Start()
    {
        if (closeBtn == null)
        {
            // 尝试在子物体中查找，假设关闭按钮是当前物体的子物体
            closeBtn = GetComponentInChildren<Button>(); // 请确保关闭按钮是直接子物体或有Button组件
        }

        if (closeBtn != null)
        {
            closeBtn.onClick.AddListener(CloseRuleTable);
        }
        else
        {
            Debug.LogError("Close button not assigned or found on HandRankingPanel!");
        }

        foreach (var rule in ScoreManager.Instance.rulesList)
        {
            if (rule != null)
            {
                GameObject ruleObj = Instantiate(ruleView, ruleTable.transform);
                RuleView rv = ruleObj.GetComponent<RuleView>();
                rv.SetRuleView(rule);
                ruleViews.Add(rv);
            }
            else
            {
                Debug.LogError("Encountered a null rule in ScoreManager's rulesList!");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CloseRuleTable()
    {
        if(handRankingPanel != null)
        {
            handRankingPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("HandRankingPanel is not assigned!");
        }
    }
}
