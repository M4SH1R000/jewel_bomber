#region

using UnityEngine;
using VContainer;
using VContainer.Unity;

#endregion

public class TitleLifetimeScope : LifetimeScope
{
  [SerializeField] private FireObj _fireObj;
  [SerializeField] private Transform Objects;
  [SerializeField] private GameObject TitleObjects;

  protected override void Configure(IContainerBuilder builder)
  {
    builder.RegisterEntryPoint<TitleFlow>();
  }
}