using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class FloorSystemManager : MonoBehaviour
{
    [Header("フェードイメージ（UI）")]
    public Image fadeImage;

    [Header("階層Prefabリスト")]
    public List<GameObject> floorPrefabs;

    [Header("階層親")]
    public Transform floorsRoot;

    [Header("フェード時間")]
    public float fadeDuration = 1f;

    [Header("トリガー距離")]
    public float triggerDistance = 1.5f;

    private GameObject player;
    private int currentFloorIndex = 0;
    private GameObject currentFloor;
    private FloorData currentFloorData;

    private bool isFirstLoad = true;
    private bool isSwitching = false;

    private void Awake()
    {
        if (fadeImage != null)
            fadeImage.color = new Color(0, 0, 0, 1);
    }

    private void Start()
    {
        // タグ "Player" でプレイヤーを自動取得
        player = GameObject.FindWithTag("Player");

        if (player == null)
        {
            Debug.LogError("[FloorSystemManager] シーンにタグ 'Player' が付いたオブジェクトが見つかりません。");
            return;
        }

        LoadFloor(currentFloorIndex, true);
        StartCoroutine(FadeIn(fadeDuration / 2f));
    }

    private void Update()
    {
        if (isSwitching || player == null || currentFloorData == null) return;

        Vector3 playerPos = player.transform.position;

        // StartPoint に到達したか判定
        if (currentFloorData.startPoint != null)
        {
            float distStart = Vector3.Distance(playerPos, currentFloorData.startPoint.position);
            if (distStart <= triggerDistance)
            {
                Debug.Log("[FloorSystemManager] StartPointに到達");
                if (currentFloorIndex > 0)
                    StartCoroutine(SwitchFloorRoutine(currentFloorIndex - 1, false));
                return;
            }
        }

        // GoalPoint に到達したか判定
        if (currentFloorData.goalPoint != null)
        {
            float distGoal = Vector3.Distance(playerPos, currentFloorData.goalPoint.position);
            if (distGoal <= triggerDistance)
            {
                Debug.Log("[FloorSystemManager] GoalPointに到達");
                if (currentFloorIndex + 1 < floorPrefabs.Count)
                    StartCoroutine(SwitchFloorRoutine(currentFloorIndex + 1, true));
                return;
            }
        }
    }

    IEnumerator SwitchFloorRoutine(int nextIndex, bool goingUp)
    {
        if (isSwitching) yield break;
        isSwitching = true;

        player.SetActive(false);
        yield return StartCoroutine(FadeOut(fadeDuration / 2f));

        if (currentFloor != null)
            Destroy(currentFloor);

        currentFloorIndex = nextIndex;

        currentFloor = Instantiate(floorPrefabs[currentFloorIndex], floorsRoot);
        currentFloorData = currentFloor.GetComponent<FloorData>();

        if (currentFloorData == null)
        {
            Debug.LogError($"[FloorSystemManager] FloorData コンポーネントがありません: {floorPrefabs[currentFloorIndex].name}");
            isSwitching = false;
            yield break;
        }

        currentFloorData.CheckReferences();

        Transform spawnPos = (isFirstLoad || goingUp) ? currentFloorData.spawnPoint : currentFloorData.goalPoint;
        player.transform.position = spawnPos != null ? spawnPos.position : Vector3.zero;

        player.SetActive(true);
        isFirstLoad = false;

        yield return StartCoroutine(FadeIn(fadeDuration / 2f));
        isSwitching = false;
    }

    void LoadFloor(int index, bool goingUp)
    {
        if (index < 0 || index >= floorPrefabs.Count) return;

        currentFloorIndex = index;

        currentFloor = Instantiate(floorPrefabs[index], floorsRoot);
        currentFloorData = currentFloor.GetComponent<FloorData>();

        if (currentFloorData == null)
        {
            Debug.LogError($"[FloorSystemManager] FloorData コンポーネントがありません: {floorPrefabs[index].name}");
            return;
        }

        currentFloorData.CheckReferences();

        Transform spawnPos = (isFirstLoad || goingUp) ? currentFloorData.spawnPoint : currentFloorData.goalPoint;
        player.transform.position = spawnPos != null ? spawnPos.position : Vector3.zero;

        isFirstLoad = false;
    }

    IEnumerator FadeOut(float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Clamp01(t / duration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        fadeImage.color = new Color(0, 0, 0, 1);
    }

    IEnumerator FadeIn(float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Clamp01(1f - t / duration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        fadeImage.color = new Color(0, 0, 0, 0);
    }
}
