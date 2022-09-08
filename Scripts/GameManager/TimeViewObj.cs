using UnityEngine;

public class TimeViewObj : MonoBehaviour
{
  [SerializeField] private GameObject timeView;
  [SerializeField] private GameObject timeViewCount;

  public void Initialize()
  {
    timeView.SetActive(true);
    timeViewCount.SetActive(true);
  }
}