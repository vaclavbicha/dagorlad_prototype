using NavMeshPlus.Components;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(NavMeshSurface))]
public class NavMeshManager : MonoBehaviour
{
    public static NavMeshManager Instance;
    NavMeshSurface navMeshSurface;

    void Start()
    {
        if (Instance == null) Instance = this;
        else Destroy(Instance);

        navMeshSurface = GetComponent<NavMeshSurface>();
        StartCoroutine(UpdateNavMeshOnStart());
    }

    IEnumerator UpdateNavMeshOnStart() {
        yield return new WaitForEndOfFrame();
        yield return new WaitForFixedUpdate();
        navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);
    }

    public void UpdateNavMesh() {
        StartCoroutine(UpdateNavMesAfterPhysicsUpdate());
    }

    IEnumerator UpdateNavMesAfterPhysicsUpdate() {
        yield return new WaitForFixedUpdate();
        navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);
    }
}
