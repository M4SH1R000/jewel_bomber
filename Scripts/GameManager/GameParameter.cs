using UnityEngine;

public class GameParameter : MonoBehaviour
{
  public int GameType;

  private void Awake()
  {
    DontDestroyOnLoad(gameObject);
  }
}