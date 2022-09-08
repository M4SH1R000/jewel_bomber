#region

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

#endregion

public class BlockPhysics : MonoBehaviour
{
  // パラメータ
  [Header("パラメータ")] [SerializeField] private float defFireSpeed;
  [SerializeField] public int blockType;

  // コンポーネント
  [SerializeField] private Rigidbody2D _rigidbody2D;
  private BulletPresenter _bulletPresenter;

  // フラグ
  public bool BombFlag { get; set; }

  // ブロックの発射
  public async UniTaskVoid SetBlockShoot(GameObject cursor, float _fireSpeed, BulletPresenter _bulletPresenter)
  {
    this._bulletPresenter = _bulletPresenter;

    var cts = new CancellationTokenSource();
    gameObject.transform.position = cursor.transform.position;
    var shootTransform = new Vector3(0, 0, 0) - transform.position;
    // var fireSpeed = defFireSpeed;
    while (_fireSpeed > 0)
    {
      try
      {
        if (!gameObject) cts.Cancel(); // UniTaskのキャンセル処理
      }
      catch (OperationCanceledException e)
      {
        Debug.Log("Canceled");
      }

      _rigidbody2D.AddForce(shootTransform * _fireSpeed * 60 * Time.deltaTime, ForceMode2D.Force); // オブジェクトを動かす

      _fireSpeed -= 16 * Time.deltaTime;

      await UniTask.Yield(cts.Token);
    }
  }
}