using UniRx;

public class ScoreModel
{
  private readonly int basePoint = 10;
  public ReactiveProperty<int> Score = new ReactiveProperty<int>();

  // スコアを計算して返す
  public int SetScore(int _fireCount, int _chainCount)
  {
    var setScore = _chainCount * (_fireCount ^ 2) * basePoint;
    return Score.Value += setScore;
  }
}