using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class ScoreRule
{
    public string ruleName;
    public int priority;
    public Dictionary<string, int> requiredDice;
    public int multiplier;
    public string description;

    public ScoreRule()
    {
        ruleName = "未中奖";
        priority = 0;
        requiredDice = new Dictionary<string, int>();
        multiplier = 0;
        description = "This is a default scoring rule.";
    }

    public ScoreRule(string[] entries)
    {
        //1 ----------- parse entries[0]: priority
        priority = int.Parse(entries[0]);

        //2 ----------- parse entries[1]: rule sequence
        string ruleSeq = entries[1];
        // number "0" represents numbers of this rule required
        requiredDice = new Dictionary<string, int>()
            {
                {"0", 0 },
                {"1", 0 },
                {"2", 0 },
                {"3", 0 },
                {"4", 0 },
                {"5", 0 },
                {"6", 0 },
                {"7", 0 },
                {"8", 0 },
                {"9", 0 }
            };

        for (int i = 0; i < ruleSeq.Length; i++)
        {
            string key = ruleSeq[i].ToString();
            if (requiredDice.ContainsKey(key))
            {
                requiredDice[key] = requiredDice[key] + 1;
                requiredDice["9"] += 1;
            } 
            else
            {
                Debug.LogError($"Invalid rule sequence character: {ruleSeq[i]}");
            }
        }

        //3 ----------- parse entries[2]: multiplier
        multiplier = int.Parse(entries[2]);

        //4 ----------- parse entries[3]: rule name
        ruleName = entries[3];

        //5 ----------- parse entries[4]: description
        description = entries[4];

        Debug.Log($"parse end!!!!!!!!!!!!!!!!!!{ruleName}");
    }
}

public class ScoreRules : Singleton<ScoreRules>
{
    public TextAsset rulesCsv;
    public List<ScoreRule> rulesList = new List<ScoreRule>();
    public int currentScore = 0;
    public ScoreRule currentRule;
    public string currentRuleName = "";
    public event System.Action<int> OnBaseCount;
    public event System.Action<ScoreRule> OnRuleCount;
    public event System.Action<int> OnScoreClear;

    private int _remainBase = 0;
    // Start is called before the first frame update
    void Start()
    {
        LoadRules();
        DeskManager.Instance.OnDiceSelect += ScoreUpdate;
        DeskManager.Instance.OnDeskClear += ScoreClear;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void LoadRules()
    {
        string[] data = rulesCsv.text.Split(new char[] { '\n' });
        int lineCount = 0;
        //Debug.Log("total line:" + data.Length);
        foreach (string line in data)
        {
            lineCount++;
            //Debug.Log("line content:" + line);

            string[] entries = line.Split(new char[] { ',' });
            if (entries.Length != 5)
            {
                Debug.LogError($"break because of a corrupt line{lineCount}: {entries.Length}");
            }

            rulesList.Add(new ScoreRule(entries));
        }
    }

    private void UpdateRemainBase(Dictionary<string, int> diceNumRemain)
    {
        int diceNumRemainValue = 0;
        foreach (var kvp in diceNumRemain)
        {
            if (kvp.Value > 0)
            {
                diceNumRemainValue += int.Parse(kvp.Key) * kvp.Value;
            }
        }

        if(diceNumRemainValue > 0) 
        {
            _remainBase += diceNumRemainValue;
        }
        else
        {
            _remainBase = 0;
        }
    }

    private void ScoreClear(Dictionary<string, int> diceNumRemain)
    {
        UpdateRemainBase(diceNumRemain);
        currentScore = _remainBase;
        currentRule = null;
        OnScoreClear?.Invoke(currentScore);
    }

    private void ScoreUpdate(Dictionary<string, int> diceSelected)
    {
        //Debug.Log("ScoreUpdate called");
        currentScore = _remainBase;
        //ScoreRule rule = null;
        for (int i = 0; i < rulesList.Count; i++)
        {
            ScoreRule rule = rulesList[i];
            if(rule == null)
            {
                Debug.LogError("rule is null!");
                continue;
            }

            int numRemains = rule.requiredDice["9"];

            foreach (var kvp in rule.requiredDice)
            {
                if(kvp.Key == "9")
                {
                    continue;
                }

                if (kvp.Value > 0)
                {
                    if (diceSelected.ContainsKey(kvp.Key))
                    {
                        if (kvp.Value > diceSelected[kvp.Key])
                        {
                            numRemains = numRemains - (diceSelected[kvp.Key]);
                        }
                        else
                        {
                            numRemains = numRemains - (kvp.Value);
                        }
                    }
                    else
                    {
                        Debug.LogError($"input dict needs to contain key:{kvp.Key}");
                        break;
                    }
                }
            }

            numRemains = numRemains - (diceSelected["9"]);
            //Debug.Log($"rule {rule.ruleName} remains {numRemains}!!!!");
            if (numRemains <= 0)
            {
                //Debug.Log($"Score rule matched: {rule.ruleName}");

                //算分并记录rule
                foreach (var kvp in diceSelected)
                {
                    //算分
                    if (kvp.Value > 0) {
                        currentScore += int.Parse(kvp.Key) * kvp.Value;
                    }
                }
                currentRule = rule;
                Debug.Log($"public base -- currentScore: {currentScore}");
                OnBaseCount?.Invoke(currentScore);
                OnRuleCount?.Invoke(currentRule);
                Debug.Log($"Current Score: {currentScore} using rule: {currentRule.ruleName}");
                return;
            }
            else
            {
                OnBaseCount?.Invoke(currentScore);
                OnRuleCount?.Invoke(new ScoreRule());
            }
        }
    }
}
