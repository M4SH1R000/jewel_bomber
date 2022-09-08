#region

using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using KanKikuchi.AudioManager;
using MyAsset.Scripts;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

#endregion

public class FireObj : MonoBehaviour
{
  //パラメータ
  [SerializeField] private float speed;
  [SerializeField] private Transform fireObjPos;
  [SerializeField] private Image flashScreen;

  // コンポーネント
  private BoxCollider2D _boxCollider;
  private Rigidbody2D _rigidbody;
  private Renderer targetRenderer;
  private BulletPresenter _bulletPresenter;

  // UniRx
  // 残りブロック数の判定
  private readonly Subject<Unit> CheckBlockCounts_FireObj = new Subject<Unit>();

  public IObservable<Unit> OnFireObjAsync => CheckBlockCounts_FireObj;

  // 発射可能タイミングの判定
  private readonly Subject<Unit> CheckCanFireObj = new Subject<Unit>();
  public IObservable<Unit> OnFireDelayAsync => CheckCanFireObj;

  // 公開パラメータ
  public int FireCount { get; set; }

  // フラグ
  public bool IsFireFlag { get; private set; }

  private void Start()
  {
    _rigidbody = GetComponent<Rigidbody2D>();
    _boxCollider = GetComponent<BoxCollider2D>();
    targetRenderer = GetComponent<Renderer>();
    this.UpdateAsObservable()
      .Where(_ => transform.position.y >= 7)
      .Subscribe(_ => ArriveToCeiling());
  }

  private void Update()
  {
    if (IsFireFlag)
      _rigidbody.velocity = transform.up * speed * Time.deltaTime;
    else
      _rigidbody.velocity = new Vector2(0, 0);
  }

  public void Initialize(BulletPresenter _bulletPresenter)
  {
    this._bulletPresenter = _bulletPresenter;
  }

  // 爆弾に衝突
  private async UniTaskVoid OnTriggerEnter2D(Collider2D col)
  {
    if (col.gameObject.TryGetComponent(out BlockBombPhysics blockBombPhysics))
    {
      blockBombPhysics.SetBombEffect(FireCount);
      speed = 0;
      await UniTask.DelayFrame(12);
      speed = 1000;
      FireCount++; // 点火コンボを加算
    }
  }

  /// <summary>
  ///   点火
  /// </summary>
  public void SetFireObj()
  {
    SEManager.Instance.Play(
      SEPath.FIRE_SHOOT,
      0.8f
    );

    FireCount = 1; // 点火コンボ数を初期化
    gameObject.transform.position = fireObjPos.position;
    IsFireFlag = true;
  }

  /// <summary>
  ///   天井に到達
  /// </summary>
  private async UniTaskVoid ArriveToCeiling()
  {
    IsFireFlag = false; // 移動フラグをオフにする
    transform.position = new Vector3(0, -10, 65);
    transform.DOMoveY(-4.5f, 1f);

    // 歓声SE
    switch (FireCount)
    {
      case > 4:
        SEManager.Instance.Play(SEPath.FANFARE_3, volumeRate: .7f);
        break;
      case > 3:
        SEManager.Instance.Play(SEPath.FANFARE_2, volumeRate: .7f);
        break;
      case > 2:
        SEManager.Instance.Play(SEPath.FANFARE_1, volumeRate: .7f);
        break;
    }

    if (FireCount > 2)
    {
      Debug.Log("this");
      flashScreen.gameObject.SetActive(true);
      flashScreen.SetOpacity(.4f);
      DOTween.ToAlpha(
        () => flashScreen.color,
        value => flashScreen.color = value,
        0,
        .5f
      );
      await UniTask.Delay(500);
      flashScreen.gameObject.SetActive(false);
      flashScreen.SetOpacity(.4f);
    }

    // transform.position = new Vector3(0, -4.5f, 65); // オブジェクトの位置を戻す
    CheckBlockCounts_FireObj.OnNext(Unit.Default);

    await UniTask.Delay(2000);
    transform.DOMoveY(-4.5f, 1f);
    await UniTask.Delay(1000);

    CheckCanFireObj.OnNext(Unit.Default);
  }

  public void EndFireObj()
  {
    IsFireFlag = false;
  }
}