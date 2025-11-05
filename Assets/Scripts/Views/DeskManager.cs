using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class DeskManager : MonoBehaviour
{
    struct DiceSlot
    {
        public Vector3 worldPos;
        public bool isOccupied;

        public DiceSlot(Vector3 pos)
        {
            worldPos = pos;
            isOccupied = false;
        }
    }
    public enum DeskState
    {
        Idle,
        Rolling,
        ShowingResults
    }

    public DeskState currentState = DeskState.Idle;

    [SerializeField] private List<DiceView> diceViewList = new List<DiceView>();
    [SerializeField] private Transform spawnPoint;

    [Tooltip("每次取出的骰子数量")]
    [SerializeField] private int diceCount = 6;

    [Tooltip("抛掷耗时（秒）")]
    [SerializeField] private float throwDuration = 1.0f;

    [Tooltip("抛物线高度（世界单位）")]
    [SerializeField] private float arcHeight = 2.0f;

    private Vector2 diceArrangePoint = Vector2.zero;

    private List<Vector2> Slots2D = new List<Vector2>();
    private List<DiceSlot> diceSlots = new List<DiceSlot>();
    private int occupiedSlotCount = 0;

    void Start()
    {
        diceArrangePoint = new Vector2(((Screen.width) / 2), (Screen.height / 2));
        Debug.Log($"diceArrangePoint is {diceArrangePoint}");
        for(int i = 0; i < diceCount; i++)
        {
            Slots2D.Add(new Vector2(diceArrangePoint.x + ((800f / diceCount) * i), diceArrangePoint.y));
            Debug.Log($"Slot {i} 位置: {Slots2D[i]}");
            diceSlots.Add(new DiceSlot(ScreenPointToWorldAtSpawnDepth(Slots2D[i])));
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && currentState == DeskState.Idle)
        {
            TryRollDice();
        }
        else if (Input.GetMouseButtonDown(0) && currentState == DeskState.ShowingResults)
        {
            // 清理桌面
            foreach (DiceView dv in diceViewList)
            {
                Destroy(dv.gameObject);
            }
            diceViewList.Clear();
            // 重置骰子槽位
            for (int i = 0; i < diceSlots.Count; i++)
            {
                DiceSlot slot = diceSlots[i];
                slot.isOccupied = false;
                diceSlots[i] = slot;
            }
            occupiedSlotCount = 0;
            currentState = DeskState.Idle;
        }
    }

    private void TryRollDice()
    {
        if (DicePackage.Instance == null)
        {
            Debug.LogWarning("DicePackage.Instance 为 null，无法抽取骰子。");
            return;
        }

        // 从牌库中尽量取 diceCount 个骰子
        List<Dice> taken = new List<Dice>();
        for (int i = 0; i < diceCount - diceViewList.Count; i++)
        {
            Dice d = DicePackage.Instance.GetDice();
            if (d == null) break;
            taken.Add(d);
        }

        if (taken.Count == 0 && diceViewList.Count == 0)
        {
            Debug.Log("未能获取到任何骰子。");
            return;
        }

        currentState = DeskState.Rolling;

        // 对每个骰子实例化并抛掷到随机未被 UI 遮盖的位置
        for (int i = 0; i < taken.Count; i++)
        {
            Vector2 screenPoint = new Vector2(
                Random.Range(600f, Screen.width - 100f),
                Random.Range(300, Screen.height - 100f)
            );

            Vector3 worldTarget = ScreenPointToWorldAtSpawnDepth(screenPoint);

            // 实例化
            DiceView diceView = DiceViewCreator.Instance.CreateDiceView(taken[i], spawnPoint.position, Quaternion.identity);
            // 加些随机旋转让效果更自然
            // 使用 DOTween 做抛物线（DOJump 会先移动到目标处并在过程中做一个抛物高度）
            // DOJump(target, jumpPower, numJumps, duration)
            diceView.transform.DOJump(worldTarget, arcHeight, 1, throwDuration).SetEase(Ease.OutQuad);
            diceViewList.Add(diceView);

            // 替换 TryRollDice 方法中对 diceSlots[j].isOccupied 的直接赋值为结构体拷贝修改
            for (int j = 0; j < diceSlots.Count; j++)
            {
                if (!diceSlots[j].isOccupied)
                {
                    // 先取出结构体，修改后再赋回去
                    DiceSlot slot = diceSlots[j];
                    slot.isOccupied = true;
                    diceSlots[j] = slot;
                    occupiedSlotCount++;
                    if(i == taken.Count - 1)
                    {
                        diceView.transform.DOMove(diceSlots[j].worldPos, 0.5f).SetDelay(throwDuration + 0.5f).OnComplete(() =>
                        {
                            currentState = DeskState.ShowingResults;
                        });
                    } 
                    else
                    {
                        diceView.transform.DOMove(diceSlots[j].worldPos, 0.5f).SetDelay(throwDuration + 0.5f);
                    }
                    break;
                }
            }
        }
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
        Debug.Log($"将屏幕点 {screenPoint} 转换为世界坐标，zDistance={zDistance}");
        return cam.ScreenToWorldPoint(sp);
    }
}
