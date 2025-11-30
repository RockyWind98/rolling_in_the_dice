using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct StageSetup
{
    public int stage;
    public int scoreCount;
    public int scorePass;
    public int debuffNum;
    public List<string> debuffId;
    public int coinBonus;
    public int rewardNum;
    public List<string> rewardItemId;

    public StageSetup(int stage, int scoreCount, int scorePass, int debuffNum, List<string> debuffId, int coinBonus, int rewardNum, List<string> rewardItemId)
    {
        this.stage = stage;
        this.scoreCount = scoreCount;
        this.scorePass = scorePass;
        this.debuffNum = debuffNum;
        this.debuffId = debuffId;
        this.coinBonus = coinBonus;
        this.rewardNum = rewardNum;
        this.rewardItemId = rewardItemId;
    }
}

public class StageManager : PersistentSingleton<StageManager>
{
    public TextAsset StageConfigCsv;
    public Dictionary<int, StageSetup> stageSetupDict = new Dictionary<int, StageSetup>();
    public int currentStage = 0;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        LoadStageConfig();
        Debug.LogWarning($"currenstage is {++currentStage}");
    }

    // Start is called before the first frame update
    void Start()
    {
        BtnManger.Instance.SetScoringCount(stageSetupDict[currentStage].scoreCount);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetPassScore()
    {
        Debug.Log($"GetPassScore called for stage {currentStage}, pass score: {stageSetupDict[currentStage].scorePass}");
        return stageSetupDict[currentStage].scorePass;
    }

    private void LoadStageConfig()
    {
        string[] data = StageConfigCsv.text.Split(new char[] { '\n' });
        int lineCount = 0;

        foreach (string line in data)
        {
            if(lineCount == 0)
            {
                // skip header line
                lineCount++;
                continue;
            }

            string[] entries = line.Split(new char[] { ',' });
            if (entries.Length != 8)
            {
                Debug.LogError($"break because of a corrupt line{lineCount}: {entries.Length}");
                break;
            }

            int stage = int.Parse(entries[0]);
            if(stage != lineCount)
            {
                Debug.LogError($"Stage Config CSV stage number mismatch at line {lineCount}: {stage} vs {lineCount}");
                break;
            }

            int scoreCount = int.Parse(entries[1]);
            int scorePass = int.Parse(entries[2]);
            int debuffNum = int.Parse(entries[3]);
            string[] debuffIds = entries[4].Split(new char[] { ' ' });
            int coinBonus = int.Parse(entries[5]);
            int rewardNum = int.Parse(entries[6]);
            string[] rewardItemIds = entries[7].Split(new char[] { ' ' });
            stageSetupDict[stage] = new StageSetup(stage, scoreCount, scorePass, debuffNum, new List<string>(debuffIds), coinBonus, rewardNum, new List<string>(rewardItemIds));

            Debug.Log($"Loaded Stage {stage} Config: scoreCount={scoreCount}, scorePass={scorePass}, debuffNum={debuffNum}, coinBonus={coinBonus}, rewardNum={rewardNum}");

            lineCount++;
        }
    }
}
