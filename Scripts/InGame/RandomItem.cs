#region

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#endregion

public class RandomItem
{
  private int bombCounter; // 10回連続で爆弾が出なかった場合に爆弾を出現させるカウンター
  private Dictionary<int, float> itemDropDict;

  public void Initialize()
  {
    InitializeDicts();
  }

  private void InitializeDicts()
  {
    bombCounter = 0;
    itemDropDict = new Dictionary<int, float>();
    itemDropDict.Add(0, 22f);
    itemDropDict.Add(1, 22f);
    itemDropDict.Add(2, 22f);
    itemDropDict.Add(3, 22f);
    itemDropDict.Add(4, 12f);
  }

  public int Choose()
  {
    // 10回連続で爆弾が出なかった場合は爆弾を出現させる
    if (bombCounter > 12)
    {
      bombCounter = 0; // カウンターを0に戻す
      return 4;
    }

    var total = itemDropDict.Sum(elem => elem.Value); // 確率の合計値

    var randomPoint = Random.value * total;
    foreach (var elem in itemDropDict)
      if (randomPoint < elem.Value)
      {
        if (elem.Key == 4)
          bombCounter = 0;
        else
          bombCounter += 1;
        return elem.Key;
      }
      else
      {
        randomPoint -= elem.Value;
      }

    return 0;
  }
}