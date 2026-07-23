using UnityEngine;

public class FloorData : MonoBehaviour
{
    [Header("各種ポイントをセットしてください")]
    public Transform spawnPoint;
    public Transform startPoint;
    public Transform goalPoint;

    private bool hasWarned = false;

    public void CheckReferences()
    {
        if (hasWarned) return;

        if (spawnPoint == null || startPoint == null || goalPoint == null)
        {
            Debug.LogWarning($"[FloorData] {gameObject.name} の SpawnPoint, StartPoint, GoalPoint のいずれかが未設定です。", this);
            hasWarned = true;
        }
    }
}
