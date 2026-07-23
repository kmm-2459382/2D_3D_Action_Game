using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequentialMover : MonoBehaviour
{
    [System.Serializable]
    public class WaypointSettings
    {
        public Transform point;
        public bool triggerToggle = false;
    }

    [Header("移動ポイント設定")]
    public List<WaypointSettings> waypoints = new List<WaypointSettings>();
    public float speed = 3f;
    public float stopDuration = 0f;

    [Header("スイッチ（任意）")]
    public Switch startSwitch;

    [Header("オブジェクト切り替え設定")]
    public List<GameObject> targetObjects = new List<GameObject>();
    public bool useRandom = false;

    private int currentIndex = 0;
    private bool isWaiting = false;
    private bool hasStarted = false;
    private bool lastSwitchState = false;
    private Vector3 startPosition;

    // ここはリセットしないように管理
    private int currentObjectIndex = -1;
    private int lastObjectIndex = -1;

    private bool firstWaypointToggleIgnored = false;

    void Start()
    {
        startPosition = transform.position;
        if (startSwitch != null)
            lastSwitchState = startSwitch.IsPressed;

        UpdateObjectStates(-1);
    }

    void Update()
    {
        if (startSwitch == null) { Move(); return; }

        bool currentSwitchState = startSwitch.IsPressed;

        if (!lastSwitchState && currentSwitchState)
        {
            // 移動位置やコルーチンはリセット
            ResetMoverPositionOnly();

            // ★ オブジェクトのインデックスはリセットせず、次のものを決定
            DetermineNextObject();

            hasStarted = true;
            firstWaypointToggleIgnored = true;
        }
        lastSwitchState = currentSwitchState;

        if (!hasStarted) return;
        Move();
    }

    void Move()
    {
        if (waypoints.Count == 0 || isWaiting) return;

        WaypointSettings settings = waypoints[currentIndex];
        if (settings.point == null) return;

        Vector3 direction = (settings.point.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, settings.point.position) < 0.1f)
        {
            if (settings.triggerToggle)
            {
                if (currentIndex == 0 && firstWaypointToggleIgnored)
                {
                    // スイッチ押下時に切り替えたばかりなので、ここではスキップ
                }
                else
                {
                    DetermineNextObject();
                }
            }
            StartCoroutine(WaitAndMoveNext());
        }
    }

    private void DetermineNextObject()
    {
        if (targetObjects.Count == 0) return;

        if (useRandom && targetObjects.Count > 1)
        {
            int nextIndex;
            do
            {
                nextIndex = Random.Range(0, targetObjects.Count);
            } while (nextIndex == lastObjectIndex);
            currentObjectIndex = nextIndex;
        }
        else
        {
            // currentObjectIndexが-1（初回）なら0に、それ以外なら次へ
            currentObjectIndex = (currentObjectIndex + 1) % targetObjects.Count;
        }

        UpdateObjectStates(currentObjectIndex);
        lastObjectIndex = currentObjectIndex;
    }

    private void UpdateObjectStates(int disabledIndex)
    {
        for (int i = 0; i < targetObjects.Count; i++)
        {
            if (targetObjects[i] == null) continue;
            targetObjects[i].SetActive(i != disabledIndex);
        }
    }

    private IEnumerator WaitAndMoveNext()
    {
        isWaiting = true;
        if (stopDuration > 0f)
            yield return new WaitForSeconds(stopDuration);

        if (currentIndex == 0) firstWaypointToggleIgnored = false;

        currentIndex = (currentIndex + 1) % waypoints.Count;
        isWaiting = false;
    }

    // ★ メソッド名を役割に合わせて変更し、Indexリセットを削除
    void ResetMoverPositionOnly()
    {
        StopAllCoroutines();
        isWaiting = false;
        currentIndex = 0;
        // currentObjectIndex = -1; // ← ここを削除
        // lastObjectIndex = -1;    // ← ここを削除
        transform.position = startPosition;
        firstWaypointToggleIgnored = false;
    }
}