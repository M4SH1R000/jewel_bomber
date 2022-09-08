using TMPro;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
  [SerializeField] private BulletView _bulletView;
  [SerializeField] private NextBlockView _nextBlockView;
  [SerializeField] private TextMeshProUGUI _scoreText;
  [SerializeField] private TimeView _timeView;

  protected override void Configure(IContainerBuilder builder)
  {
    builder.Register<BulletModel>(Lifetime.Singleton);
    builder.Register<RandomItem>(Lifetime.Singleton);
    builder.Register<NextBlockModel>(Lifetime.Singleton);
    builder.Register<TimeModel>(Lifetime.Singleton);
    builder.Register<LevelView>(Lifetime.Singleton);

    builder.Register<ScorePresenter>(Lifetime.Singleton);
    builder.Register<ScoreView>(Lifetime.Singleton);
    builder.Register<ScoreModel>(Lifetime.Singleton);
    builder.Register<GameFlowPresenter>(Lifetime.Singleton);
    builder.RegisterComponent(_timeView);
    builder.RegisterComponent(_scoreText);
    builder.RegisterComponent(_nextBlockView);
    builder.RegisterComponent(_bulletView);
    builder.RegisterEntryPoint<BulletPresenter>();
  }
}