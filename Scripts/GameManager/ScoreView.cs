using TMPro;
using UnityEngine;

public class ScoreView : MonoBehaviour
{
  [SerializeField] private TextMeshProUGUI scoreText;

  // スコアを表示
  public void SetScoreGUI(int score)
  {
    scoreText.text = score.ToString();
  }
}