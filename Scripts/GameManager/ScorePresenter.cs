public class ScorePresenter
{
  public readonly ScoreModel _scoreModel;
  private readonly ScoreView _scoreView;

  public ScorePresenter
  (
    ScoreView _scoreView,
    ScoreModel _scoreModel
  )
  {
    this._scoreView = _scoreView;
    this._scoreModel = _scoreModel;
  }

  // void IStartable.Start()
  // {
  //   _scoreModel
  //     .Score
  //     .Subscribe(x =>
  //       Debug.Log(x)
  //     )
  //     .AddTo(disposable);
  // }

  public void SetScore(int _fireCount, int _chainCount)
  {
    _scoreView.SetScoreGUI(_scoreModel.SetScore(_fireCount, _chainCount));
  }
}