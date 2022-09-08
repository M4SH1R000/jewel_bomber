#region

using UnityEngine;
using UnityEngine.UI;

#endregion

namespace MyAsset.Scripts
{
  public static class ImageExt
  {
    /// <summary>
    ///   Imageの不透明度を設定する
    /// </summary>
    /// <param name="image">設定対象のImageコンポーネント(this)</param>
    /// <param name="alpha">不透明度。0=透明 1=不透明</param>
    public static void SetOpacity(this Image image, float alpha)
    {
      var c = image.color;
      image.color = new Color(c.r, c.g, c.b, alpha);
    }
  }
}