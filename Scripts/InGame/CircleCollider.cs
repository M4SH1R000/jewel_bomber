using System.Linq;
using UnityEngine;

public class CircleCollider : MonoBehaviour
{
  public bool removeExistingColliders = true;

  private void Start()
  {
    CreateInvertedMeshCollider();
  }

  public void CreateInvertedMeshCollider()
  {
    if (removeExistingColliders)
      RemoveExistingColliders();

    InvertMesh();

    gameObject.AddComponent<MeshCollider>();
  }

  private void RemoveExistingColliders()
  {
    var colliders = GetComponents<Collider>();
    for (var i = 0; i < colliders.Length; i++)
      DestroyImmediate(colliders[i]);
  }

  private void InvertMesh()
  {
    var mesh = GetComponent<MeshFilter>().mesh;
    mesh.triangles = mesh.triangles.Reverse().ToArray();
  }
}