#region

using System.Collections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using KanKikuchi.AudioManager;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#endregion

public class TitleFlow : MonoBehaviour
{
  // コンポーネント
  private GameParameter _gameParameter;
  [SerializeField] private FireObj _fireObj;
  [SerializeField] private Transform Objects;
  [SerializeField] private GameObject TitleObjects;
  [SerializeField] private Transform FireObjPos;

  // 操作指示
  [SerializeField] private TextMeshProUGUI controlText;
  [SerializeField] private GameObject rightMouseClick_On;
  [SerializeField] private GameObject rightMouseClick_Off;
  [SerializeField] private Button SendTimeAttackButton;
  [SerializeField] private Transform SendTimeAttackButtonPos;

  private void Start()
  {
    Application.targetFrameRate = 60;
    _gameParameter = GameObject.Find("GameParameter").GetComponent<GameParameter>();
    Initialize();
  }
  //
  // [Inject]
  // public TitleFlow(
  //   FireObj _fireObj,
  //   Transform Objects,
  //   GameObject TitleObjects
  // )
  // {
  //   this._fireObj = _fireObj;
  //   this.Objects = Objects;
  //   this.TitleObjects = TitleObjects;
  // }

  // 最初の動き
  private async UniTaskVoid Initialize()
  {
    BGMManager.Instance.Play(BGMPath.TITLE_1);

    await UniTask.Delay(3000);
    Objects.DOMove(new Vector3(0, 0, 70), 1f);
    await UniTask.Delay(1000);

    _fireObj.gameObject.SetActive(true);
    _fireObj.transform.DOMoveY(FireObjPos.transform.position.y, .3f);

    // controlText.gameObject.SetActive(true);
    var setIcon = SetIcon();
    StartCoroutine(setIcon);

    await UniTask.WaitUntil(() => Input.GetMouseButtonDown(1));

    BGMManager.Instance.Stop(BGMPath.TITLE_1);

    // controlText.gameObject.SetActive(false);
    StopCoroutine(setIcon);
    rightMouseClick_On.SetActive(false);
    rightMouseClick_Off.SetActive(false);

    _fireObj.SetFireObj();
    _fireObj
      .UpdateAsObservable()
      .Where(_ => _fireObj.transform.position.y > 6)
      .Subscribe(_ => _fireObj.gameObject.SetActive(false));
    await UniTask.Delay(2000);
    Objects.DOMove(new Vector3(0, -10, 70), .5f);
    await UniTask.Delay(1000);
    TitleObjects.transform.DOMove(new Vector3(0, 0, .3f), .5f);
    await UniTask.Delay(200);
    SEManager.Instance.Play(
      SEPath.BOMB,
      .7f
    );
    await UniTask.Delay(1000);
    BGMManager.Instance.Play(
      BGMPath.ELEMENTS,
      .5f);

    await UniTask.Delay(500);
    SendTimeAttackButton.transform.DOMoveY(SendTimeAttackButtonPos.position.y, .3f);
  }

  private IEnumerator SetIcon()
  {
    while (true)
    {
      rightMouseClick_On.SetActive(true);
      rightMouseClick_Off.SetActive(false);
      yield return new WaitForSeconds(1);
      rightMouseClick_On.SetActive(false);
      rightMouseClick_Off.SetActive(true);
      yield return new WaitForSeconds(1);
    }
  }

  public void SendTimeAttack()
  {
    _gameParameter.GameType = 1;
    SendTimeAttackButton.gameObject.SetActive(false);
    SendTimeAttackTask();
  }

  private async UniTaskVoid SendTimeAttackTask()
  {
    SEManager.Instance.Play(SEPath.DECIDE, volumeRate: 1f);
    await UniTask.Delay(500);
    SceneManager.LoadScene("MainScene");
  }

  public void SendEndless()
  {
    _gameParameter.GameType = 2;
    SceneManager.LoadScene("MainScene");
  }
}