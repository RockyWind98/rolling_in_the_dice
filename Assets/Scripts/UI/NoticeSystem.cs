using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening; // 引入DOTween命名空间

public class NoticeSystem : Singleton<NoticeSystem>
{
    public TMP_Text noticeText;

    void Start()
    {
        // 确保noticeText默认不显示
        if (noticeText != null)
        {
            noticeText.alpha = 0; // 设置透明度为0
        }
    }

    /// <summary>
    /// 显示提示文本，并通过缩放和淡化效果提示玩家
    /// </summary>
    /// <param name="message">要显示的提示信息</param>
    public void ShowNotice(string message)
    {
        if (noticeText == null) return;

        // 设置文本内容
        noticeText.text = message;

        // 确保文本初始状态
        noticeText.alpha = 0;
        noticeText.transform.localScale = Vector3.one * 0.8f;

        // 动画：缩放并显示 -> 停留 -> 淡出
        Sequence sequence = DOTween.Sequence();
        sequence.Append(noticeText.DOFade(1, 0.03f)) // 淡入
                .Join(noticeText.transform.DOScale(1.2f, 0.05f)) // 放大
                .Append(noticeText.transform.DOScale(1f, 0.03f)) // 缩回正常大小
                .AppendInterval(1f) // 停留1秒
                .Append(noticeText.DOFade(0, 0.1f)) // 淡出
                .OnComplete(() =>
                {
                    // 动画结束后确保透明度为0
                    noticeText.alpha = 0;
                });
    }
}
