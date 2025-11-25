using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DeskManager : Singleton<DeskManager>
{
    public Button rerollBtn;

    struct DiceSlot
    {
        public Vector3 worldPos;
        public bool isOccupied;
        public DiceView diceView;

        public DiceSlot(Vector3 pos)
        {
            worldPos = pos;
            isOccupied = false;
            diceView = null;
        }
    }
    public enum DeskState
    {
        Idle,
        Rolling,
        ShowingResults
    }

    public DeskState currentState = DeskState.Idle;
    public event System.Action<Dictionary<string, int>> OnDiceSelect;
    public event System.Action<Dictionary<string, int>> OnDeskClear;

    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform destroyPoint;

    [Tooltip("每次取出的骰子数量")]
    [SerializeField] private int diceCount = 6;

    [Tooltip("掷骰耗时（秒）")]
    [SerializeField] private float throwDuration = 1.0f;

    [Tooltip("骰子抛出后的等待排列时间（秒）")]
    [SerializeField] private float waitForArrange = 0.01f;

    [Tooltip("掷骰抛物线高度（世界单位）")]
    [SerializeField] private float arcHeight = 2.0f;

    [Tooltip("计分骰子动画耗时（秒）")]
    [SerializeField] private float scoringDuration = 0.3f;

    private List<DiceView> diceViewList = new List<DiceView>();
    private List<DiceView> diceSelected = new List<DiceView>();
    private Vector2 diceArrangePoint = Vector2.zero;

    private List<Vector2> Slots2D = new List<Vector2>();
    private List<DiceSlot> diceSlots = new List<DiceSlot>();
    private Dictionary<string, int> _diceNumSelected;
    private Dictionary<string, int> _diceNumRemain;
    private int occupiedSlotCount = 0;
    private List<Dice> _diceTake = new List<Dice>();

    void Start()
    {
        diceArrangePoint = new Vector2(((Screen.width) / 2), (Screen.height / 2));
        for(int i = 0; i < diceCount; i++)
        {
            Slots2D.Add(new Vector2(diceArrangePoint.x + ((800f / diceCount) * i), diceArrangePoint.y));
            diceSlots.Add(new DiceSlot(ScreenPointToWorldAtSpawnDepth(Slots2D[i])));
        }

        _diceNumSelected = new Dictionary<string, int>()
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

        _diceNumRemain = new Dictionary<string, int>()
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
    }

    void Update()
    {

    }

    public int GetSelectedDiceCount()
    {
        return diceSelected.Count;
    }

    public void ClearDeskSettlement()
    {
        ClearSelectedDice();
        // get & public _diceNumRemain
        foreach (var dv in diceViewList)
        {
            if (dv != null)
            {
                if (_diceNumRemain.ContainsKey(dv.dice.num.ToString()))
                {
                    _diceNumRemain[dv.dice.num.ToString()] += 1;
                }
            }
        }

        OnDeskClear?.Invoke(_diceNumRemain);
        RemainDiceNumClear();
    }

    public void ClearSelectedDice()
    {
        //draw clear animation
        foreach (DiceView dv in diceSelected)
        {
            if (dv != null)
            {
                Vector3 start = dv.transform.position;
                Vector3 end = destroyPoint.position;

                // 使用 DOTween.To 实现抛物线动画
                DOTween.To(
                    () => 0f, // 起始值
                    t =>
                    {
                        // 使用 EMath.Parabola 计算抛物线位置
                        Vector3 position = EMath.Parabola(start, end, arcHeight, t);
                        dv.transform.position = position;
                    },
                    1f, // 结束值
                    scoringDuration // 动画持续时间
                ).SetEase(Ease.Linear) // 设置线性时间插值
                .OnComplete(() =>
                {
                    Destroy(dv.gameObject);
                });

                foreach (var slot in diceSlots)
                {
                    if (slot.diceView == dv)
                    {
                        DiceSlot clearedSlot = slot;
                        clearedSlot.isOccupied = false;
                        clearedSlot.diceView = null;
                        int index = diceSlots.IndexOf(slot);
                        diceSlots[index] = clearedSlot;
                        occupiedSlotCount--;
                        break;
                    }
                }

                dv.OnDiceClicked -= HandleDiceClicked;

                if (diceViewList.Contains(dv))
                {
                    DicePackage.Instance.DiscardDice(dv.dice);
                    diceViewList.Remove(dv);
                }
            }
        }

        diceSelected.Clear();

        //clear selection record
        foreach (string key in _diceNumSelected.Keys.ToList())
        {
            _diceNumSelected[key] = 0;
        }

        _diceTake.Clear();

        Debug.Log($"remain diceView{diceViewList.Count}");

        currentState = DeskState.Idle;
    }

    public void RollDice()
    {
        OnDiceSelect?.Invoke(_diceNumSelected);

        if (DicePackage.Instance == null)
        {
            Debug.LogWarning("DicePackage.Instance 为 null，无法抽取骰子。");
            return;
        }

        // 从牌库中尽量取 diceCount 个骰子
        for (int i = 0; i < diceCount - diceViewList.Count; i++)
        {
            Dice d = DicePackage.Instance.GetDice();
            if (d == null) break;
            _diceTake.Add(d);
        }

        if (_diceTake.Count == 0 && diceViewList.Count == 0)
        {
            Debug.Log("未能获取到任何骰子。");
            return;
        }

        currentState = DeskState.Rolling;

        Debug.Log($"抽取到 {_diceTake.Count} 个骰子进行掷骰。");

        // 对每个骰子实例化并抛掷到随机未被 UI 遮盖的位置
        for (int i = 0; i < _diceTake.Count; i++)
        {
            Vector2 screenPoint = new Vector2(
                Random.Range(600f, Screen.width - 100f),
                Random.Range(300, Screen.height - 100f)
            );

            Vector3 worldTarget = ScreenPointToWorldAtSpawnDepth(screenPoint);

            // 实例化
            DiceView diceView = DiceViewCreator.Instance.CreateDiceView(_diceTake[i], spawnPoint.position, Quaternion.identity);

            // 注册点击事件（委托会传回该 DiceView 实例）
            diceView.OnDiceClicked += HandleDiceClicked;

            // 加些随机旋转让效果更自然
            // 使用 DOTween 做抛物线（DOJump 会先移动到目标处并在过程中做一个抛物高度）
            // DOJump(target, jumpPower, numJumps, duration)
            diceView.transform.DOJump(worldTarget, arcHeight, 1, throwDuration).SetEase(Ease.OutQuad);
            diceViewList.Add(diceView);

            //1. find an unoccupied slot
            //2. draw dice to that slot
            for (int j = 0; j < diceSlots.Count; j++)
            {
                if (!diceSlots[j].isOccupied)
                {
                    // 先取出结构体，修改后再赋回去
                    DiceSlot slot = diceSlots[j];
                    slot.isOccupied = true;
                    slot.diceView = diceView;
                    diceSlots[j] = slot;
                    occupiedSlotCount++;
                    if(i == _diceTake.Count - 1)
                    {
                        diceView.transform.DOMove(diceSlots[j].worldPos, 0.5f).SetDelay(throwDuration + waitForArrange).OnComplete(() =>
                        {
                            currentState = DeskState.ShowingResults;
                        });
                    } 
                    else
                    {
                        diceView.transform.DOMove(diceSlots[j].worldPos, 0.5f).SetDelay(throwDuration + waitForArrange);
                    }
                    break;
                }
            }
        }
    }

    private void RemainDiceNumClear()
    {
        foreach (string key in _diceNumRemain.Keys.ToList())
        {
            _diceNumRemain[key] = 0;
        }
    }

    private void SelectDice(DiceView dv)
    {
        if (dv == null) return;

        // 确保字典包含该数字的键
        string key = dv.dice.num.ToString();
        if (!_diceNumSelected.ContainsKey(key))
        {
            _diceNumSelected[key] = 0;
        }

        // cansel selection implementation
        if (diceSelected.Contains(dv))
        {
            dv.ViewDown();
            diceSelected.Remove(dv);

            // 减少该数字的计数（不小于 0）
            _diceNumSelected[key] = Mathf.Max(0, _diceNumSelected[key] - 1);

            // 重新计算最大重复数（键 "0"），跳过键 "0" 本身
            int max = 0;
            foreach (var kvp in _diceNumSelected)
            {
                if (kvp.Key == "0") continue;
                if (kvp.Value > max) max = kvp.Value;
            }
            _diceNumSelected["0"] = max;
            return;
        }

        // selection implementation
        diceSelected.Add(dv);
        _diceNumSelected[key] = _diceNumSelected.ContainsKey(key) ? _diceNumSelected[key] + 1 : 1;
        dv.ViewUp();

        // 更新最大重复数（键 "0"）
        if (_diceNumSelected[key] > _diceNumSelected["0"])
        {
            _diceNumSelected["0"] = _diceNumSelected[key];
        }
    }

    // 点击事件处理器：对外暴露的 DiceView 会在被点时调用此方法
    private void HandleDiceClicked(DiceView dv)
    {
        if (dv == null) return;

        // 示例行为：当桌面处于展示结果状态时允许手动重掷该骰子；处于 Rolling 时忽略点击
        if (currentState != DeskState.ShowingResults)
        {
            Debug.Log("dice click handler can only trigger when DeskState.ShowingResults");
            return;
        }

        SelectDice(dv);
        OnDiceSelect?.Invoke(_diceNumSelected);
    }

    // 将屏幕点转换为在 spawnPoint 深度上的世界坐标
    private Vector3 ScreenPointToWorldAtSpawnDepth(Vector2 screenPoint)
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogWarning("Main camera 未找到，使用世界原点作为目标。");
            return Vector3.zero;
        }

        float zDistance = Mathf.Abs(cam.transform.position.z - spawnPoint.position.z);
        Vector3 sp = new Vector3(screenPoint.x, screenPoint.y, zDistance);
        return cam.ScreenToWorldPoint(sp);
    }
}
