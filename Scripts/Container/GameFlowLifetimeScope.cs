#region

using BattleScene.Common;
using MyAsset.Scripts.GameManager;
using MyAsset.Scripts.InGame;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

#endregion

public class GameFlowLifetimeScope : LifetimeScope
{
  // コンポーネント
  [SerializeField] private ScoreView _scoreView;
  [SerializeField] private TimeView _timeView;
  [SerializeField] private GameFlowView _gameFlowView;
  [SerializeField] private BulletView _bulletView;
  [SerializeField] private GameStartScreen _gameStartScreen;
  [SerializeField] private NextBlockView _nextBlockView;
  [SerializeField] private GameParameter _gameParameter;
  [SerializeField] private Transform _blocks;
  [SerializeField] private DangerView _dangerView;
  [SerializeField] private FireObj _fireObj;
  [SerializeField] private TextMeshProUGUI _alertText;
  [SerializeField] private Image _flashScreen;
  [SerializeField] private FrameModel _frameModel;
  [SerializeField] private CameraShake _cameraShake;
  [SerializeField] private CenterPointController _centerPointController;

  protected override void Configure(IContainerBuilder builder)
  {
    builder.RegisterEntryPoint<ScorePresenter>().AsSelf();
    builder.RegisterEntryPoint<BulletPresenter>().AsSelf();
    builder.RegisterEntryPoint<GameFlowPresenter>().AsSelf();

    builder.Register<ScoreModel>(Lifetime.Singleton);
    builder.Register<LevelView>(Lifetime.Singleton);
    builder.Register<TimeViewObj>(Lifetime.Singleton);
    builder.Register<TimeModel>(Lifetime.Singleton);
    builder.Register<BlockPhysics>(Lifetime.Singleton);
    builder.Register<LevelViewObj>(Lifetime.Singleton);
    builder.Register<GameFlowModel>(Lifetime.Singleton);
    builder.Register<BulletModel>(Lifetime.Singleton);
    builder.Register<RandomItem>(Lifetime.Singleton);
    builder.Register<NextBlockModel>(Lifetime.Singleton);

    builder.RegisterComponent(_scoreView);
    builder.RegisterComponent(_timeView);
    builder.RegisterComponent(_gameFlowView);
    builder.RegisterComponent(_bulletView);
    builder.RegisterComponent(_gameStartScreen);
    builder.RegisterComponent(_nextBlockView);
    builder.RegisterComponent(_gameParameter);
    builder.RegisterComponent(_blocks);
    builder.RegisterComponent(_dangerView);
    builder.RegisterComponent(_fireObj);
    builder.RegisterComponent(_alertText);
    builder.RegisterComponent(_flashScreen);
    builder.RegisterComponent(_frameModel);
    builder.RegisterComponent(_cameraShake);
    builder.RegisterComponent(_centerPointController);
  }
}