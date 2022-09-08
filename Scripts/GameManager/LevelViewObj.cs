using UnityEngine;

public class LevelViewObj : MonoBehaviour
{
  [SerializeField] private GameObject levelView;
  [SerializeField] private GameObject levelViewCount;

  public void Initialize()
  {
    levelView.SetActive(true);
    levelViewCount.SetActive(true);
  }
}