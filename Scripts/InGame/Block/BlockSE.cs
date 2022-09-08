using KanKikuchi.AudioManager;
using UnityEngine;

public class BlockSE : MonoBehaviour
{
  private BoxCollider2D _boxCollider2D;
  private Rigidbody2D _rigidbody2D;

  private void Start()
  {
    _rigidbody2D = transform.parent.GetComponent<Rigidbody2D>();
  }

  private void OnCollisionEnter2D(Collision2D col)
  {
    if (_rigidbody2D.velocity.magnitude > 0.4f)
      if (col.gameObject.TryGetComponent(out BlockPhysics _blockPhysics))
        SEManager.Instance.Play(SEPath.KOKA);
  }
}