using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class ScoreView : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreCurrentText_;
    [SerializeField] private TMP_Text scorePassText_;

    // 保存当前显示值，避免每次从文本解析
    private int _currentDisplayed = 0;
    private int _scoreGets = 0;
    private int _scorePass = 0;

    // 保存 Tween 引用以便取消/替换正在进行的动画
    private Tweener _valueTween;
    private Tweener _shakeTween;
    private int currentBase = 0;
    private int currentMultiplier = 0;

    void Start()
    {
        if (scoreCurrentText_ == null)
        {
            Debug.LogError("ScoreView: scoreCurrentText_ is null.");
            return;
        }
        else
        {
            scoreCurrentText_.text = "0";
        }

        ScoreManager.Instance.OnRuleCount += UpdateRuleText;
        ScoreManager.Instance.OnBaseCount += UpdateBaseText;
        DeskManager.Instance.OnDeskClear += ShowScoreGets;
        _scorePass = StageManager.Instance.GetPassScore();
        scorePassText_.text = _scorePass.ToString();
    }

    void OnDestroy()
    {
        // 清理残留 Tween
        if (_valueTween != null && _valueTween.IsActive()) _valueTween.Kill();
        if (_shakeTween != null && _shakeTween.IsActive()) _shakeTween.Kill();
    }

    private void UpdateRuleText(ScoreRule rule)
    {
        UpdateMultiplierValue(rule.multiplier);
    }

    private void UpdateBaseText(int value)
    {
        UpdateBaseValue(value);
    }

    private void UpdateBaseValue(int value)
    {
        currentBase = value;
    }

    private void UpdateMultiplierValue(int value) 
    {
        currentMultiplier = value;
    }

    private void SetScoreGets()
    {
        _scoreGets += currentBase * currentMultiplier;
        Debug.Log($"ScoreView SetScoreGets called, new _scoreGets: {_scoreGets}");
    }

    // 播放从当前显示值到 _scoreGets 的快速增/减动画，同时伴随震动
    public void ShowScoreGets(Dictionary<string, int> diceNumRemain)
    {
        SetScoreGets();
        if (scoreCurrentText_ == null)
        {
            Debug.LogError("ScoreView.ShowScoreGets called but scoreCurrentText_ is null.");
            return;
        }

        // 终止任何正在进行的动画，保证新动画干净开始
        if (_valueTween != null && _valueTween.IsActive()) _valueTween.Kill();
        if (_shakeTween != null && _shakeTween.IsActive()) _shakeTween.Kill();

        int start = _currentDisplayed;
        int delta = Mathf.Abs(_scoreGets - start);

        // 根据差值计算动画时长（差值越大，时长略长），可按需调整范围
        float duration = Mathf.Clamp(0.2f + (delta / 100f) * 0.8f, 0.2f, 1.2f);

        // 数字动画（使用 DOVirtual.Float 简化数值插值）
        _valueTween = DOVirtual.Float(start, _scoreGets, duration, value =>
        {
            _currentDisplayed = Mathf.RoundToInt(value);
            scoreCurrentText_.text = _currentDisplayed.ToString();
        }).SetEase(Ease.OutCubic);

        // 伴随震动（锚点抖动），与数字动画同长
        // 强度、vibrato、随机度可根据视觉需求微调
        _shakeTween = scoreCurrentText_.rectTransform.DOShakeAnchorPos(duration, new Vector2(12f, 6f), vibrato: 12, randomness: 90f).SetEase(Ease.Linear);

        // 完成后确保最终值精确
        _valueTween.OnComplete(() =>
        {
            _currentDisplayed = _scoreGets;
            scoreCurrentText_.text = _scoreGets.ToString();

            if (_shakeTween != null && _shakeTween.IsActive())
                _shakeTween.Kill();
        });
    }
}
