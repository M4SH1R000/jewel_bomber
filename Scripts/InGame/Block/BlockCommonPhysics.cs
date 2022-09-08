#region

using Cysharp.Threading.Tasks;
using KanKikuchi.AudioManager;
using UnityEngine;

#endregion

public class BlockCommonPhysics : MonoBehaviour
{
  // コンポーネント
  [SerializeField] private CircleCollider2D _circleCollider2D;
  [SerializeField] private ParticleSystem _effect;
  [SerializeField] private BlockPhysics _blockPhysics;

  // パラメータ
  [SerializeField] public int blockType;

  // スコア通知用のコンポーネント
  public ScorePresenter _scorePresenter;
  private int chainCount;

  // 非公開パラメータ
  private int fireCount;

  private void Start()
  {
    fireCount = 0;
    chainCount = 0;
  }

  // 自身が爆発する際他を巻き込む
  private void OnTriggerStay2D(Collider2D other)
  {
    if (!_blockPhysics.BombFlag) return;
    if (!other.gameObject.TryGetComponent(out BlockCommonPhysics blockCommonPhysics)) return;
    if (blockType == blockCommonPhysics.blockType)
      blockCommonPhysics.SetBombEffect(fireCount, chainCount); // 自身が持っているコンボ数を渡す
  }

  // 他の爆発が接触したときに発動
  public async UniTaskVoid SetBombEffect(int _fireCount, int _chainCount)
  {
    if (_blockPhysics.BombFlag) return;

    SEManager.Instance.Play(
      SEPath.KIRA,
      0.8f
    );
    fireCount = _fireCount++;
    chainCount = _chainCount++;

    _blockPhysics.BombFlag = true;
    _circleCollider2D.enabled = true;

    // 情報を送信
    _scorePresenter?.SetScore(fireCount, chainCount);

    // エフェクトの生成
    var exp = Instantiate(_effect);
    exp.transform.position = transform.position;

    // 破壊まで待機
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