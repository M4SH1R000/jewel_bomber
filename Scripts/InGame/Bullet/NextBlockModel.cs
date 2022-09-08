using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

public class NextBlockModel
{
  // コンポーネント
  private readonly RandomItem _randomItem;

  // リスト
  public ReactiveCollection<int> nextBlockList { get; private set; } // UniRxのList

  // コンテナを生成
  public NextBlockModel(
    RandomItem _randomItem
  )
  {
    this._randomItem = _randomItem;
  }

  // アイテム配列の作成
  public void Initilize()
  {
    nextBlockList = new ReactiveCollection<int>();
    for (var i = 0; i < 4; i++) nextBlockList.Add(_randomItem.Choose());
  }

  // 先頭のアイテムを渡す
  public int GetNextBlock()
  {
    return nextBlockList[0];
  }

  // アイテムを詰める
  public void SetNextBlock()
  {
    nextBlockList.RemoveAt(0);
    nextBlockList.Add(_randomItem.Choose());
  }
}