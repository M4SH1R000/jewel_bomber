#region

using System;
using Cysharp.Threading.Tasks;
using KanKikuchi.AudioManager;
using UniRx;
using UnityEngine;
using VContainer;
using VContainer.Unity;

#endregion

public class BulletPresenter : IDisposable, IStartable
{
  // 弾の発生処理
  private readonly BulletView _bulletView;

  // 次の弾の選択
  private readonly NextBlockModel _nextBlockModel;

  // コンポーネント
  private readonly NextBlockView _nextBlockView;
  private readonly RandomItem _randomItem;
  private readonly FireObj _fireObj;

  // スコア計算
  private readonly ScorePresenter _scorePresenter;

  // GameFlowと連結
  private readonly Subject<Unit> CheckBlockCounts = new Subject<Unit>();
  public IObservable<Unit> OnBulletShootAsync => CheckBlockCounts;

  // UniTaskのキャンセル
  private readonly CompositeDisposable disposable = new CompositeDisposable();

  // コンポーネント
  public Transform blocks;

  // パラメータ
  private float bulletShootInterval; // 弾発射の間隔
  private int currentAngle; // ランダムな位置にブロックを落とす角度


  [Inject]
  public BulletPresenter(
    BulletView _bulletView,
    RandomItem _randomItem,
    NextBlockModel _nextBlockModel,
    NextBlockView _nextBlockView,
    ScorePresenter _scorePresenter,
    Transform blocks,
    FireObj _fireObj
  )
  {
    this._bulletView = _bulletView;
    this._randomItem = _randomItem;
    this._nextBlockModel = _nextBlockModel;
    this._nextBlockView = _nextBlockView;
    this._scorePresenter = _scorePresenter;
    this.blocks = blocks;
    this._fireObj = _fireObj;
  }

  // 行動可能フラグ
  public bool CanSetBullet { get; set; } = false;
  public bool CanSetFire { get; set; }

  public void Dispose()
  {
    disposable.Dispose();
  }

  void IStartable.Start()
  {
    // コンポーネントの初期化
    _randomItem.Initialize();
    _nextBlockModel.Initilize();
    _bulletView._scorePresenter = _scorePresenter;
    _bulletView.Initialize(blocks, this);

    // 弾のリストの監視を行う
    var nextBlockList = _nextBlockModel.nextBlockList;
    _nextBlockView.Initilize(nextBlockList);
    nextBlockList
      .ObserveAdd()
      .Subscribe(x => _nextBlockView.Initilize(_nextBlockModel.nextBlockList));

    //操作入力
    Observable
      .EveryUpdate()
      .Where(x => Input.GetMouseButtonDown(0) && CanSetBullet)
      .Subscribe(_ => SetBullet())
      .AddTo(disposable);
    Observable
      .EveryUpdate()
      .Where(_ => Input.GetMouseButtonDown(1) && CanSetFire)
      .Subscribe(_ =>
      {
        _bulletView.FireObj();
        CanSetFire = false; // すぐに次の弾が撃てないようにする
      })
      .AddTo(disposable);

    // 発射許可
    _fireObj
      .OnFireDelayAsync
      .Subscribe(_ => CanSetFire = true);

    // ブロックの自動落下でブロックの残り数カウントをトリガー
    _bulletView
      .OnBlockFallAsync
      .Subscribe(_ => CheckBlockCounts.OnNext(Unit.Default));
  }

  /// <summary>
  ///   弾を発射する
  /// </summary>
  private async UniTaskVoid SetBullet()
  {
    if (bulletShootInterval > 0) return;

    var fireSpeed = 1f;
    SEManager.Instance.Play(
      SEPath.CHARGE,
      volumeRate: 0.5f,
      isLoop: false
    );

    while (Input.GetMouseButton(0))
    {
      if (fireSpeed < 4f)
        fireSpeed += 3f * Time.deltaTime;
      else
        fireSpeed = 4f;
      await UniTask.Yield();
    }

    SEManager.Instance.Stop(SEPath.CHARGE);

    bulletShootInterval = 0.3f;

    _bulletView.SetShoot(_nextBlockModel.GetNextBlock(), fireSpeed, this); // ブロックの先頭を発射
    _nextBlockModel.SetNextBlock(); // 発射した分のブロックを削除して末尾に新たに弾のキーをセットする

    // ブロック数を確認
    CheckBlockCounts.OnNext(Unit.Default);

    while (bulletShootInterval > 0)
    {
      bulletShootInterval -= 1 * Time.deltaTime;
      await UniTask.Yield();
    }
  }

  /// <summary>
  ///   弾の落下を移譲
  /// </summary>
  public void SetFallBlock()
  {
    _bulletView.SetFallBlock(_randomItem.Choose());
  }
}