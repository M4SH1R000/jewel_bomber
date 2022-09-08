#region

using System;
using System.Collections.Generic;
using KanKikuchi.AudioManager;
using UniRx;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

#endregion

public class BulletView : MonoBehaviour
{
  // コンポーネント
  [Header("コンポーネント")] [SerializeField] private GameObject block_bomb;
  [SerializeField] private GameObject block_red;
  [SerializeField] private GameObject block_blue;
  [SerializeField] private GameObject block_yellow;
  [SerializeField] private GameObject block_purple;
  [SerializeField] private FireObj _fireObj;

  [Header("パラメータ")] [SerializeField] private float fireSpeed;

  [Header("エフェクト")] [SerializeField] private ParticleSystem partice;
  [SerializeField] private Vector3 screenPoint;

  private readonly List<GameObject> BulletPool = new List<GameObject>();
  public ScorePresenter _scorePresenter;
  private Transform blocks;
  private BulletPresenter _bulletPresenter;

  // パラメータ
  private int currentAngle; // ランダムな位置にブロックを落とす角度

  // UniRx
  private readonly Subject<Unit> CheckBlockCounts = new Subject<Unit>();
  public IObservable<Unit> OnBlockFallAsync => CheckBlockCounts;

  public void Initialize(Transform blocks, BulletPresenter _bulletPresenter)
  {
    this.blocks = blocks;
    this._bulletPresenter = _bulletPresenter;
    _fireObj.Initialize(_bulletPresenter);
  }

  /// <summary>
  ///   弾を発射する
  /// </summary>
  /// <param name="key"></param>
  /// <param name="_fireSpeed"></param>
  /// <param name="_bulletPresenter"></param>
  public void SetShoot(int blockKey, float _fireSpeed, BulletPresenter _bulletPresenter)
  {
    var block = GetBlock(blockKey); // キーに基づいたブロックを生成
    SEManager.Instance.Play(
      SEPath.PON,
      0.5f
    );
    Instantiate(partice, transform.position, quaternion.identity);
    block.GetComponent<BlockPhysics>().SetBlockShoot(gameObject, _fireSpeed, _bulletPresenter); // ブロックの発射
  }

  /// <summary>
  ///   ランダムな弾の落下
  /// </summary>
  /// <param name="blockKey"></param>
  public void SetFallBlock(int blockKey)
  {
    while (true)
    {
      var angle = Random.Range(30, 150);

      if (Mathf.Abs(currentAngle - angle) < 30) continue; // あまりにも角度が近かったら再帰

      currentAngle = angle;

      // ブロックを落とす円周上の座標を求める
      var x = (float)(3 * Math.Cos(angle * (Math.PI / 180)));
      var y = (float)(3 * Math.Sin(angle * (Math.PI / 180)));

      // ブロックを生成して落とす
      var block = GetBlock(blockKey);
      block.transform.position = new Vector3(x, y, 70);

      // ブロックの数の計算をお願いする
      CheckBlockCounts.OnNext(Unit.Default);

      break;
    }
  }

  // キーに基づいたブロックを指定
  public GameObject GetBlock(int key)
  {
    GameObject block = null;
    switch (key)
    {
      case 0:
        block = Instantiate(block_purple, blocks);
        block.GetComponent<BlockCommonPhysics>()._scorePresenter = _scorePresenter;
        break;
      case 1:
        block = Instantiate(block_red, blocks);
        block.GetComponent<BlockCommonPhysics>()._scorePresenter = _scorePresenter;
        break;
      case 2:
        block = Instantiate(block_blue, blocks);
        block.GetComponent<BlockCommonPhysics>()._scorePresenter = _scorePresenter;
        break;
      case 3:
        block = Instantiate(block_yellow, blocks);
        block.GetComponent<BlockCommonPhysics>()._scorePresenter = _scorePresenter;
        break;
      case 4:
        block = Instantiate(block_bomb, blocks);
        block.GetComponent<BlockBombPhysics>()._scorePresenter = _scorePresenter;
        break;
    }

    return block;
  }

  /// <summary>
  ///   着火弾を発射する
  /// </summary>
  public void FireObj()
  {
    _fireObj.SetFireObj();
  }
}