#region

using UnityEngine;

#endregion

namespace MyAsset.Scripts.InGame
{
  public class FrameModel : MonoBehaviour
  {
    [SerializeField] private EdgeCollider2D _edgeCollider2D;
    [SerializeField] private BoxCollider2D _boxCollider2D;

    public void SetCollider(bool flag)
    {
      _edgeCollider2D.enabled = flag;
      _boxCollider2D.enabled = flag;
    }
  }
}