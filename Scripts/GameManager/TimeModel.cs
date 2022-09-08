#region

using UniRx;

#endregion

public class TimeModel
{
  public ReactiveProperty<float> GameTime = new ReactiveProperty<float>();

  public void Initialize()
  {
    GameTime.Value = 99.99f;
  }
}