#region

using System;
using BattleScene.Common;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using KanKikuchi.AudioManager;
using MyAsset.Scripts;
using MyAsset.Scripts.GameManager;
using MyAsset.Scripts.InGame;
using naichilab;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

#endregion

public class GameFlowPresenter : IStartable, ITickable, IDisposable
{
  // コンストラクタ
  private readonly LevelView _levelView;
  private readonly ScorePresenter _scorePresenter;
  private readonly TimeModel _timeModel;
  private readonly TimeView _timeView;
  private readonly BulletPresenter _bulletPresenter;
  private readonly GameFlowModel _gameFlowModel;
  private readonly GameFlowView _gameFlowView;
  private readonly GameParameter _gameParameter;
  private readonly GameStartScreen _gameStartScreen;
  private readonly DangerView _dangerView;
  private readonly FireObj _fireObj;
  private readonly Transform _blocks;
  private readonly TextMeshProUGUI _alertText;
  private readonly Image _flashScreen;
  private readonly FrameModel _frameModel;
  private readonly CameraShake _cameraShake;
  private readonly CenterPointController _centerPointController;

  // UniTaskのキャンセル
  private readonly CompositeDisposable disposable = new CompositeDisposable();

  // フラグ
  private bool gameStartFlag_time;

  // パラメータ
  private readonly int defaultTimeUpTextPos = -1;
  private readonly int limitBlockCounts = 43;
  private readonly float cameraShakeMagunitude = .05f;

  [Inject]
  public GameFlowPresenter
  (
    TimeView _timeView,
    TimeModel _timeModel,
    LevelView _levelView,
    GameFlowView _gameFlowView,
    GameStartScreen _gameStartScreen,
    GameFlowModel _gameFlowModel,
    BulletPresenter _bulletPresenter,
    GameParameter _gameParameter,
    ScorePresenter _scorePresenter,
    DangerView _dangerView,
    FireObj _fireObj,
    Transform _blocks,
    TextMeshProUGUI _alertText,
    Image _flashScreen,
    FrameModel _frameModel,
    CameraShake _cameraShake,
    CenterPointController _centerPointController
  )
  {
    this._timeView = _timeView;
    this._timeModel = _timeModel;
    this._levelView = _levelView;
    this._gameFlowView = _gameFlowView;
    this._gameStartScreen = _gameStartScreen;
    this._gameFlowModel = _gameFlowModel;
    this._bulletPresenter = _bulletPresenter;
    this._gameParameter = _gameParameter;
    this._scorePresenter = _scorePresenter;
    this._dangerView = _dangerView;
    this._fireObj = _fireObj;
    this._blocks = _blocks;
    this._alertText = _alertText;
    this._flashScreen = _flashScreen;
    this._frameModel = _frameModel;
    this._cameraShake = _cameraShake;
    this._centerPointController = _centerPointController;
  }

  void IDisposable.Dispose()
  {
    disposable.Dispose();
  }

  // Start is called before the first frame update
  void IStartable.Start()
  {
    Application.targetFrameRate = 60;
    BGMManager.Instance.Stop();
    _gameStartScreen.GameStartBtn.onClick.AddListener(() =>
      SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex));

    // ゲームオーバー処理を監視
    _gameFlowModel
      .OnGameOverAsync
      .Subscribe(_ => SetGameOver())
      .AddTo(disposable);

    // ブロックを数える
    _bulletPresenter
      .OnBulletShootAsync
      .Subscribe(_ => CheckBlockCount())
      .AddTo(disposable);

    _fireObj
      .OnFireObjAsync
      .Subscribe(_ => CheckBlockCount())
      .AddTo(disposable);

    SetGameType();
  }

  void ITickable.Tick()
  {
    if (!gameStartFlag_time) return;
    // タイムの計測
    _timeModel.GameTime.Value -= Time.deltaTime;
    _timeView.timeText.text = _timeModel.GameTime.Value.ToString("n2");

    _gameFlowModel.CheckFallBlock(_timeModel.GameTime.Value);

    // タイムアップ
    if (_timeModel.GameTime.Value <= 0) SetTimeOut();
  }

  /// <summary>
  ///   ゲームモードを選択
  /// </summary>
  private void SetGameType()
  {
    _gameStartScreen.gameObject.SetActive(false);
    _gameFlowView.messageTextObject.transform.DOMoveY(-1, 0);
    _gameFlowView.messageTextObject.SetActive(false);

    switch (_gameParameter.GameType)
    {
      case 0:
        SetDebug();
        break;
      case 1:
        SetTimeAttack();
        break;
      case 2:
        SetEndless();
        break;
    }
  }

  // デバッグモード
  private void SetDebug()
  {
    _bulletPresenter.CanSetBullet = true;
    _bulletPresenter.CanSetFire = true;
  }

  // タイムアタック
  private async UniTaskVoid SetTimeAttack()
  {
    // オブジェクトを消去
    foreach (Transform block in _bulletPresenter.blocks.transform)
    {
      Object.Destroy(block.gameObject);
    }

    // 初期化処理
    _timeModel.Initialize(); // タイムをリセット
    _alertText.transform.position = new Vector2(-50, 0); // アラートを画面外に
    _flashScreen.gameObject.SetActive(false); // フラッシュを非アクティブ
    _frameModel.SetCollider(true); // コライダを有効化

    // UniRx を設定
    _gameFlowModel.SetTime(_timeModel.GameTime.Value);
    _gameFlowModel
      .OnSetFallBlockTimeAsync
      .Subscribe(_ => _bulletPresenter.SetFallBlock())
      .AddTo(disposable);

    // ゲーム開始処理
    _centerPointController.CanRotateFlag = true;

    // カウントダウン
    _alertText.transform.DOMoveX(0, .3f);

    _alertText.text = "3";
    SEManager.Instance.Play(SEPath.COUNTDOWN);
    await UniTask.Delay(1000);
    _alertText.text = "2";
    SEManager.Instance.Play(SEPath.COUNTDOWN);
    await UniTask.Delay(1000);
    _alertText.text = "1";
    SEManager.Instance.Play(SEPath.COUNTDOWN);
    await UniTask.Delay(1000);
    _alertText.text = "GAME START!!";
    SEManager.Instance.Play(SEPath.START);
    await UniTask.Delay(1000);
    _alertText.transform.DOMoveX(10, .3f);
    _alertText.gameObject.SetActive(false);

    _bulletPresenter.CanSetBullet = true;
    _bulletPresenter.CanSetFire = true;
    gameStartFlag_time = true;
    _gameFlowModel.SetBGM();

    // 最初にいくつかブロックを降らせる
    for (var i = 0; i < 5; i++)
    {
      _bulletPresenter.SetFallBlock();
      UniTask.Yield();
    }
  }

  // エンドレス
  private void SetEndless()
  {
    _gameFlowModel.SetBGM();
  }

  /// <summary>
  ///   タイムアウト処理
  /// </summary>
  private async UniTaskVoid SetTimeOut()
  {
    // 行動制御
    gameStartFlag_time = false;
    _bulletPresenter.CanSetBullet = false;
    _bulletPresenter.CanSetFire = false;
    _centerPointController.CanRotateFlag = false;

    // ゲーム表示
    _timeView.timeText.text = "TIME UP"; // 時間終了
    BGMManager.Instance.Stop();
    SEManager.Instance.Play(
      SEPath.SHAKIIN,
      .5f
    );
    _alertText.gameObject.SetActive(true);
    _alertText.transform.DOMoveX(-10f, 0);
    _alertText.text = "TIME UP!!";
    _alertText.transform.DOMoveX(0, .3f);

    _flashScreen.gameObject.SetActive(true);
    _flashScreen.SetOpacity(.4f);

    await UniTask.Delay(1000);

    _alertText.transform.DOMoveX(50, 0);

    await UniTask.Delay(500);

    _gameFlowView.messageTextObject.SetActive(true);
    _gameFlowView.messageTextObject.transform.DOMoveY(0, .3f);
    BGMManager.Instance.Play(
      BGMPath.TITLE_CALL,
      .5f,
      isLoop: false
    );
    await UniTask.Delay(2000);

    _timeView.timeText.text = "CLICK TO NEXT"; // ランキングに遷移

    await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0));

    // ランキング表示
    _gameFlowView.messageTextObject.SetActive(false);
    RankingLoader.Instance.SendScoreAndShowRanking(_scorePresenter._scoreModel.Score.Value);

    // ゲームループ
    _gameStartScreen.gameObject.SetActive(true);
  }

  /// <summary>
  ///   ゲームオーバー処理
  /// </summary>
  private async UniTaskVoid SetGameOver()
  {
    // 行動制御
    gameStartFlag_time = false;
    _bulletPresenter.CanSetBullet = false;
    _bulletPresenter.CanSetFire = false;
    _centerPointController.CanRotateFlag = false;

    // 画面効果
    _cameraShake.Shake(cameraShakeMagunitude);
    _fireObj.gameObject.SetActive(false);

    SEManager.Instance.Play(
      SEPath.GAMEOVER_1,
      volumeRate: .5f
    );
    _flashScreen.gameObject.SetActive(true);
    _flashScreen.SetOpacity(.4f);
    await UniTask.Delay(50);
    _flashScreen.gameObject.SetActive(false);
    SEManager.Instance.Play(
      SEPath.GAMEOVER_1
    );
    await UniTask.Delay(200);
    SEManager.Instance.Play(
      SEPath.GAMEOVER_1
    );
    _flashScreen.gameObject.SetActive(true);
    await UniTask.Delay(50);
    _flashScreen.gameObject.SetActive(false);
    await UniTask.Delay(500);

    SEManager.Instance.Play(
      SEPath.GAMEOVER_2
    );

    _frameModel.SetCollider(false);

    await UniTask.Delay(3000);
    _cameraShake.Stop();
    _flashScreen.gameObject.SetActive(true);

    // 演出
    _alertText.transform.DOMoveX(-10f, 0);
    _alertText.text = "GAME OVER";
    _alertText.gameObject.SetActive(true);
    _alertText.transform.DOMoveX(0, .3f);

    BGMManager.Instance.Play(
      BGMPath.MUS_MUS_JGL016,
      isLoop: false
    );

    await UniTask.Delay(3000);

    // ランキング表示
    RankingLoader.Instance.SendScoreAndShowRanking(_scorePresenter._scoreModel.Score.Value);
    _alertText.transform.DOMoveX(-600, 0);
    _alertText.gameObject.SetActive(false);

    // ゲームループ
    _gameStartScreen.gameObject.SetActive(true);
  }

  /// <summary>
  ///   ブロック数を数える
  /// </summary>
  /// <param name="_blocks"></param>
  private void CheckBlockCount()
  {
    _gameFlowModel._blockCounts.Value = _blocks.childCount;
    if (_gameFlowModel._blockCounts.Value > limitBlockCounts)
    {
      if (_dangerView.GetDanger()) return;
      _dangerView.SetDanger(true);
      _dangerView.SetImageGradation();
      _gameFlowModel.SetGameOverTime(true);
      SEManager.Instance.Play(
        SEPath.BEEP,
        volumeRate: .5f
      );
    }
    else
    {
      if (!_dangerView.GetDanger()) return;
      _dangerView.StopImageGradation();
      _dangerView.SetDanger(false);
      _gameFlowModel.SetGameOverTime(false);
    }
  }
}