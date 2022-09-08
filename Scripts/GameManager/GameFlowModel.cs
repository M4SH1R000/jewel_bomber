#region

using System;
using KanKikuchi.AudioManager;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

#endregion

public class GameFlowModel : IDisposable
{
  // パラメータ
  private float setFallBlockTime;
  private float setBlockInterval;
  private readonly float defaultSetBlockInterval = 5;

  // ブロックの落下
  private readonly Subject<Unit> SetFallBlockSubject = new();
  public IObservable<Unit> OnSetFallBlockTimeAsync => SetFallBlockSubject;

  // ゲームオーバー処理
  private float gameOverTime; // ゲームオーバーまでの残り時間
  private bool gameOverFlag; // ゲームオーバーモードか判定
  private IDisposable gameUpdate;
  private readonly CompositeDisposable disposable = new CompositeDisposable(); // UniTaskのキャンセル

  // UniRx
  private readonly AsyncSubject<Unit> _onGameOverSubject = new AsyncSubject<Unit>();
  public IObservable<Unit> OnGameOverAsync => _onGameOverSubject;

  public void SetTime(float setFallBlockTime)
  {
    this.setFallBlockTime = setFallBlockTime;
    setBlockInterval = defaultSetBlockInterval;
    gameOverFlag = false;
  }

  /// <summary>
  ///   ランダムでブロックを降らすタイミングをチェック
  /// </summary>
  /// <param name="time"></param>
  /// <returns></returns>
  public void CheckFallBlock(float time)
  {
    if (!(setFallBlockTime - time > setBlockInterval)) return;
    // 時間によって降る頻度を変更
    setBlockInterval = time switch
    {
      > 60 => 5,
      > 30 => 3,
      > 15 => 2,
      > 0 => 1,
      _ => setBlockInterval
    };

    setFallBlockTime = time;
    SetFallBlockSubject.OnNext(Unit.Default);
  }

  /// <summary>
  ///   ゲームオーバー処理の発行
  /// </summary>
  /// <param name="flag"></param>
  public void SetGameOverTime(bool flag)
  {
    if (gameOverFlag.Equals(flag)) return; // 現在の状態と引数が同じならリターン（重複処理を避ける）
    gameOverFlag = flag;
    if (gameOverFlag)
    {
      gameOverTime = 0;
      gameUpdate = Observable
        .EveryUpdate()
        .Subscribe(_ => CalcGameOverTime())
        .AddTo(disposable);
    }
    else
    {
      gameUpdate.Dispose();
    }
  }

  void IDisposable.Dispose()
  {
    disposable.Dispose();
  }

  /// <summary>
  ///   ゲームオーバーまでの時間を計算
  /// </summary>
  private void CalcGameOverTime()
  {
    gameOverTime += Time.deltaTime;
    // ５秒経過したら以下を実行
    if (!(gameOverTime > 5)) return;
    gameUpdate.Dispose();
    _onGameOverSubject.OnNext(Unit.Default);
    _onGameOverSubject.OnCompleted();
  }

  /// <summary>
  ///   現在のブロック数を記憶
  /// </summary>
  public ReactiveProperty<int> _blockCounts = new ReactiveProperty<int>();

  /// <summary>
  ///   BGMをセットする
  /// </summary>
  public void SetBGM()
  {
    var bgmNum = Random.Range(0, 3);
    switch (bgmNum)
    {
      case 0:
        BGMManager.Instance.Play(
          BGMPath.NC205403,
          0.7f,
          isLoop: true
        );
        break;
      case 1:
        BGMManager.Instance.Play(
          BGMPath.NC237945,
          0.7f,
          isLoop: true
        );
        break;
      case 2:
        BGMManager.Instance.Play(
          BGMPath.NC223213,
          0.7f,
          isLoop: true
        );
        break;
    }
  }
}