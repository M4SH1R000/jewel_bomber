using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class NextBlockView : MonoBehaviour
{
  // ブロックの画像
  [Header("ブロックの画像（小）")] [SerializeField]
  private GameObject block_bomb;

  [SerializeField] private GameObject block_red;
  [SerializeField] private GameObject block_blue;
  [SerializeField] private GameObject block_yellow;
  [SerializeField] private GameObject block_purple;

  [Header("ブロックの画像（大）")] [SerializeField]
  private GameObject block_bomb_big;

  [SerializeField] private GameObject block_red_big;
  [SerializeField] private GameObject block_blue_big;
  [SerializeField] private GameObject block_yellow_big;
  [SerializeField] private GameObject block_purple_big;

  [SerializeField] private GameObject Icon_1;
  [SerializeField] private GameObject Icon_2;
  [SerializeField] private GameObject Icon_3;
  [SerializeField] private GameObject Icon_4;

  // ブロック配列
  [Header("ブロック配列")] private List<GameObject> nextBlockViewList;

  public void Initilize(ReactiveCollection<int> nextBlockList)
  {
    // 最初にnextBlockList から受け取ったアイコンを設定していく
    nextBlockViewList = new List<GameObject>();
    nextBlockViewList.Add(GetBlockIconBig(nextBlockList[0], Icon_1.transform));
    nextBlockViewList.Add(GetBlockIconSmall(nextBlockList[1], Icon_2.transform));
    nextBlockViewList.Add(GetBlockIconSmall(nextBlockList[2], Icon_3.transform));
    nextBlockViewList.Add(GetBlockIconSmall(nextBlockList[3], Icon_4.transform));
  }

  public GameObject GetBlockIconBig(int key, Transform iconPanel)
  {
    GameObject blockIcon = null;
    switch (key)
    {
      case 0:
        SetBlockIconBig(block_purple_big, iconPanel);
        break;
      case 1:
        SetBlockIconBig(block_red_big, iconPanel);
        break;
      case 2:
        SetBlockIconBig(block_blue_big, iconPanel);
        break;
      case 3:
        SetBlockIconBig(block_yellow_big, iconPanel);
        break;
      case 4:
        SetBlockIconBig(block_bomb_big, iconPanel);
        break;
    }

    return blockIcon;
  }

  public void SetBlockIconBig(GameObject block, Transform iconPanel)
  {
    foreach (Transform n in iconPanel) Destroy(n.gameObject);

    Instantiate(block, iconPanel);
  }


  public GameObject GetBlockIconSmall(int key, Transform iconPanel)
  {
    GameObject blockIcon = null;
    switch (key)
    {
      case 0:
        SetBlockIconSmall(block_purple, iconPanel);
        break;
      case 1:
        SetBlockIconSmall(block_red, iconPanel);
        break;
      case 2:
        SetBlockIconSmall(block_blue, iconPanel);
        break;
      case 3:
        SetBlockIconSmall(block_yellow, iconPanel);
        break;
      case 4:
        SetBlockIconSmall(block_bomb, iconPanel);
        break;
    }

    return blockIcon;
  }


  public void SetBlockIconSmall(GameObject block, Transform iconPanel)
  {
    foreach (Transform n in iconPanel) Destroy(n.gameObject);

    Instantiate(block, iconPanel);
  }
}