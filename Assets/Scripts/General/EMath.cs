using UnityEngine;

public static class EMath
{
    public static Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
    {
        // 1. 计算水平面上的中间插值点
        var mid = Vector3.Lerp(start, end, t);

        // 2. 核心公式：计算垂直方向（Y轴）的偏移量
        // 这是一个二次函数，模拟了上升减速、下降加速的过程
        // 当t=0.5时，函数取得最大值height，从而实现顶点最高
        float yOffset = 4 * (-height * t * t + height * t);

        // 3. 叠加起点和终点的Y值，确保抛物线在斜向投射时也正确
        yOffset += Mathf.Lerp(start.y, end.y, t);

        return new Vector3(mid.x, yOffset, mid.z);
    }
}