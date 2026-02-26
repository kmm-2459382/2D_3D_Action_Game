using UnityEngine;

[ExecuteInEditMode]
public class AreaTracker : MonoBehaviour
{
    void OnEnable() { AreaManager.Register(transform); }
    void OnDisable() { AreaManager.Unregister(transform); }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 1, 0.5f);
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
    }
}