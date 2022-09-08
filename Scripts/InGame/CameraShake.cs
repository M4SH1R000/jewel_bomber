#region

using System.Collections;
using UnityEngine;

#endregion

namespace BattleScene.Common
{
  public class CameraShake : MonoBehaviour
  {
    private IEnumerator DoShakeCoroutine;

    public void Shake(float magnitude)
    {
      DoShakeCoroutine = DoShake(magnitude);
      StartCoroutine(DoShakeCoroutine);
    }

    public void Stop()
    {
      StopCoroutine(DoShakeCoroutine);
      DoShakeCoroutine = null;
    }

    private IEnumerator DoShake(float magnitude)
    {
      var pos = transform.localPosition;

      var elapsed = 0f;

      while (true)
      {
        var x = pos.x + Random.Range(-1f, 1f) * magnitude;
        var y = pos.y + Random.Range(-1f, 1f) * magnitude;

        transform.localPosition = new Vector3(x, y, pos.z);

        elapsed += Time.deltaTime;

        yield return null;
      }

      transform.localPosition = pos;
    }
  }
}