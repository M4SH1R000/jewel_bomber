#region

using UniRx;
using UniRx.Triggers;
using UnityEngine;

#endregion

public class CenterPointController : MonoBehaviour
{
  public bool CanRotateFlag;

  private void Start()
  {
    this
      .UpdateAsObservable()
      .Subscribe(_ => SetRotation())
      .AddTo(this);
  }

  private void SetRotation()
  {
    if (!CanRotateFlag) return;
    var pos = Camera.main.WorldToScreenPoint(transform.localPosition);
    var rotation = Quaternion.LookRotation(Vector3.forward, Input.mousePosition - pos);

    rotation.z = rotation.z switch
    {
      > 0 and < .6f => .6f,
      < 0 and > -0.6f => -0.6f,
      _ => rotation.z
    };
    transform.localRotation = rotation;
  }
}