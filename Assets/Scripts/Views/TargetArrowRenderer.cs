using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class TargetArrowRenderer : Singleton<TargetArrowRenderer>
{
    [Header("核心组件")]
    [Tooltip("箭头身体的小圆点/方块预制体")]
    public GameObject arrowNodePrefab;
    [Tooltip("箭头尖端的三角形预制体")]
    public GameObject arrowHeadPrefab;

    [Header("路径设置")]
    [Tooltip("身体节点的数量。数量越多越连贯，但性能消耗越大。建议20-30。")]
    public int nodeCount = 25;
    [Tooltip("曲线拱起的高度基数。")]
    public float baseArcHeight = 2.5f;
    [Tooltip("起始点的额外偏移。例如(0, 0.5, 0)可以让箭头从卡牌顶部而不是中心发出。")]
    public Vector3 startOffset = new Vector3(0, 1, 0);

    [Header("外观调整")]
    [Tooltip("节点大小随路径变化的曲线。横轴0是起点，1是终点。")]
    public AnimationCurve scaleCurve = new AnimationCurve(new Keyframe(0, 0.4f), new Keyframe(1, 1.2f));
    [Tooltip("如果你的节点素材默认向上指，填-90；向右指，填0。")]
    public float nodeRotationOffset = -90f;
    [Tooltip("如果你的尖端素材默认向上指，填-90；向右指，填0。")]
    public float headRotationOffset = 0;

    private Transform startTransform;
    private List<Transform> nodes = new List<Transform>();
    private Transform arrowHead;
    private bool isActive = false;
    private bool isStopSet = false;
    private Vector3 stopPos;

    void Start()
    {
        InitializePool();
    }

    private void InitializePool()
    {
        // 清理旧的（如果在编辑器里动态修改了nodeCount）
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        nodes.Clear();

        // 1. 生成身体节点池
        for (int i = 0; i < nodeCount; i++)
        {
            GameObject node = Instantiate(arrowNodePrefab, transform);
            node.SetActive(false);
            nodes.Add(node.transform);
        }

        // 2. 生成箭头尖端
        GameObject head = Instantiate(arrowHeadPrefab, transform);
        head.SetActive(false);
        arrowHead = head.transform;
    }

    void Update()
    {
        if (!isActive || startTransform == null) return;

        // 计算实际起点（加上偏移量）
        Vector3 actualStartPos = Camera.main.ScreenToWorldPoint(startTransform.position);
        actualStartPos.y -= 0.5f;
        actualStartPos.z = 0;

        if (isStopSet)
        {
            // 使用预设的停止位置
            UpdateArrowPath(actualStartPos, stopPos);
            return;
        }

        // 获取鼠标位置
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        UpdateArrowPath(actualStartPos, mousePos);
    }

    // 开启箭头
    public void EnableArrow(Transform newStartTransform)
    {
        if (nodes.Count != nodeCount) InitializePool(); //以此确保编辑器修改参数后生效

        startTransform = newStartTransform;
        isActive = true;
        foreach (var node in nodes) node.gameObject.SetActive(true);
        arrowHead.gameObject.SetActive(true);
    }

    // 关闭箭头
    public void DisableArrow()
    {
        isActive = false;
        foreach (var node in nodes) if (node) node.gameObject.SetActive(false);
        if (arrowHead) arrowHead.gameObject.SetActive(false);
        startTransform = null;
    }

    private void UpdateArrowPath(Vector3 p0, Vector3 p3)
    {
        // 1. 动态计算控制点
        float distance = Vector3.Distance(p0, p3);
        Vector3 midPoint = (p0 + p3) / 2;

        // 动态高度：距离越远拱得越高，但也受限于最大值，避免过分夸张
        float currentArcHeight = Mathf.Clamp(distance * 0.3f, baseArcHeight * 0.5f, baseArcHeight * 1.5f);

        // p1 固定在 p0 正下方 y -1
        Vector3 p1 = p0 + Vector3.down * 1f;
        // p2 用作拱高控制，靠近中点并向上偏移
        Vector3 p2 = midPoint + Vector3.up * currentArcHeight;

        // 2. 更新身体节点
        // 我们让身体节点分布在 t = 0 到 t = 0.95 之间，把最后的 1.0 留给尖端
        float tStep = 0.95f / (float)(nodes.Count - 1);

        for (int i = 0; i < nodes.Count; i++)
        {
            float t = i * tStep;

            // 位置（三次贝塞尔）
            Vector3 currentPos = CalculateCubicBezierPoint(t, p0, p1, p2, p3);
            nodes[i].position = currentPos;

            // 旋转：计算一个非常近的未来点来确定切线方向 (解决旋转不连贯问题)
            Vector3 slightlyForwardPos = CalculateCubicBezierPoint(t + 0.01f, p0, p1, p2, p3);
            Vector3 direction = (slightlyForwardPos - currentPos).normalized;
            SetRotation(nodes[i], direction, nodeRotationOffset);

            // 缩放：使用曲线控制
            float scale = scaleCurve.Evaluate(t);
            nodes[i].localScale = new Vector3(scale, scale, 1f);
        }

        // 3. 更新箭头尖端 (位于终点 p3)
        arrowHead.position = p3;
        // 尖端的方向是由曲线末端的切线决定的，我们取 t=0.99 和 t=1.0 的方向
        Vector3 preEndPos = CalculateCubicBezierPoint(0.99f, p0, p1, p2, p3);
        Vector3 headDir = (p3 - preEndPos).normalized;
        SetRotation(arrowHead, headDir, headRotationOffset);
        arrowHead.localScale = new Vector3(scaleCurve.Evaluate(1f), scaleCurve.Evaluate(1f), 1f);
    }

    // 设置旋转的通用方法
    private void SetRotation(Transform target, Vector3 direction, float offsetAngle)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        target.rotation = Quaternion.Euler(0, 0, angle + offsetAngle);
    }

    // 贝塞尔公式 (保持不变)
    private Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        t = Mathf.Clamp01(t); // 确保t在0-1之间
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        Vector3 p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;
        return p;
    }

    // 三次贝塞尔公式
    private Vector3 CalculateCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        t = Mathf.Clamp01(t); // 确保t在0-1之间
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;
        Vector3 p = uuu * p0; // (1-t)^3 * P0
        p += 3 * uu * t * p1; // 3*(1-t)^2*t * P1
        p += 3 * u * tt * p2; // 3*(1-t)*t^2 * P2
        p += ttt * p3; // t^3 * P3
        return p;
    }

    public bool IsActive()
    {
        return isActive;
    }

    public void SetStopPosition(Vector3 pos)
    {
        stopPos = pos;
        isStopSet = true;
    }

    public void ClearStopPosition()
    {
        isStopSet = false;
    }
}