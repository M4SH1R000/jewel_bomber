#region

using Cysharp.Threading.Tasks;
using KanKikuchi.AudioManager;
using UnityEngine;

#endregion

public class BlockBombPhysics : MonoBehaviour
{
  // コンポーネント
  [SerializeField] private ParticleSystem ExplosionEffect;
  private CircleCollider2D _circleCollider;

  // スコア通知用のコンポーネント
  public ScorePresenter _scorePresenter;

  // パラメータ
  private int fireCount;

  // フラグ
  private bool BombFlag { get; set; }

  private void Start()
  {
    _circleCollider = GetComponent<CircleCollider2D>();
    fireCount = 0; // インスタンス時に点火コンボを初期化
  }

  private void OnTriggerStay2D(Collider2D other)
  {
    if (BombFlag)
      if (other.gameObject.TryGetComponent(out BlockCommonPhysics _blockCommonPhysics))
        _blockCommonPhysics.SetBombEffect(fireCount, 1);
      else if (other.gameObject.TryGetComponent(out BlockBombPhysics _blockBombPhysics))
        _blockBombPhysics.SetBombEffect(fireCount);
  }

  // ブロックの爆発
  public async UniTaskVoid SetBombEffect(int _fireCount)
  {
    // 既に点火していたらreturn
    if (BombFlag) return;

    SEManager.Instance.Play(
      SEPath.BOMB,
      0.8f
    );

    fireCount = _fireCount++; // 点火コンボ数を渡す

    BombFlag = true;
    _circleCollider.enabled = true;
    var exp = Instantiate(ExplosionEffect);
    exp.transform.position = transform.position;
    await UniTask.DelayFrame(6);
    try
    {
      Destroy(gameObject);
    }
    catch
    {
    }
  }
}