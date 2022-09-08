#region

using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

#endregion

namespace MyAsset.Scripts.GameManager
{
  public class DangerView : MonoBehaviour
  {
    [SerializeField] private Image dangerTop;
    [SerializeField] private Image dangerBottom;
    private TweenerCore<Color, Color, ColorOptions> dangerTopTween;
    private TweenerCore<Color, Color, ColorOptions> dangerBottomTween;

    /// <summary>
    ///   デンジャー表示のオンオフ
    /// </summary>
    /// <param name="flag"></param>
    public void SetDanger(bool flag)
    {
      dangerTop.enabled = flag;
      dangerBottom.enabled = flag;
    }

    /// <summary>
    ///   デンジャー状態かどうかを返す
    /// </summary>
    /// <returns></returns>
    public bool GetDanger()
    {
      return dangerTop.enabled;
    }

    /// <summary>
    ///   デンジャー表示をグラデーション
    /// </summary>
    public void SetImageGradation()
    {
      dangerTopTween = DOTween.ToAlpha(
        () => dangerTop.color,
        value => dangerTop.color = value,
        .3f,
        2f
      ).SetLoops(-1, LoopType.Yoyo);
      dangerBottomTween = DOTween.ToAlpha(
        () => dangerBottom.color,
        value => dangerBottom.color = value,
        .3f,
        2f
      ).SetLoops(-1, LoopType.Yoyo);
    }

    /// <summary>
    ///   デンジャー表示のグラデーションを解除
    /// </summary>
    public void StopImageGradation()
    {
      dangerTopTween.Kill();
      dangerBottomTween.Kill();
    }
  }
}