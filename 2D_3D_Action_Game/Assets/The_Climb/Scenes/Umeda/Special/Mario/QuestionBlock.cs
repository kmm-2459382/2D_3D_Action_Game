using UnityEngine;
using System.Collections;

public class QuestionBlock : MonoBehaviour
{
    [Header("ループ表示モデル（1〜4）")]
    public GameObject[] loopModels;
    public GameObject usedModel;

    [Header("出現オブジェクト")]
    public GameObject spawnPrefab;   // キノコ・コインなど
    public Transform spawnPoint;

    [Header("表示ループ")]
    public float loopInterval = 0.15f;

    [Header("ブロックバンプ")]
    public float bumpHeight = 0.2f;
    public float bumpDuration = 0.1f;

    [Header("出現アニメ")]
    public float spawnHeight = 0.6f;
    public float spawnDuration = 0.25f;

    bool isUsed = false;
    int currentIndex = 0;
    float loopTimer;

    Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;

        ShowOnly(loopModels[0]);
        usedModel.SetActive(false);
    }

    void Update()
    {
        if (isUsed) return;

        loopTimer += Time.deltaTime;
        if (loopTimer >= loopInterval)
        {
            loopTimer = 0f;
            currentIndex = (currentIndex + 1) % loopModels.Length;
            ShowOnly(loopModels[currentIndex]);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isUsed) return;
        if (!collision.gameObject.CompareTag("Player")) return;

        ContactPoint contact = collision.contacts[0];

        // 下から叩いた判定
        if (contact.normal.y > 0.5f)
        {
            StartCoroutine(ActivateBlock());
        }
    }

    IEnumerator ActivateBlock()
    {
        isUsed = true;

        // 同時開始
        StartCoroutine(Bump());
        yield return SpawnItem();

        // 使用済み表示
        ShowOnly(usedModel);
    }

    // ===== ブロックが少し動く =====
    IEnumerator Bump()
    {
        yield return MoveBlock(startPos + Vector3.up * bumpHeight);
        yield return MoveBlock(startPos);
    }

    IEnumerator MoveBlock(Vector3 target)
    {
        Vector3 from = transform.localPosition;
        float t = 0f;

        while (t < bumpDuration)
        {
            t += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(from, target, t / bumpDuration);
            yield return null;
        }

        transform.localPosition = target;
    }

    // ===== 中身を出す =====
    IEnumerator SpawnItem()
    {
        if (spawnPrefab == null || spawnPoint == null)
            yield break;

        GameObject item = Instantiate(
            spawnPrefab,
            spawnPoint.position,
            Quaternion.identity
        );

        // 物理を一旦止める
        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
        }

        Vector3 start = spawnPoint.position;
        Vector3 end = start + Vector3.up * spawnHeight;

        float t = 0f;
        while (t < spawnDuration)
        {
            t += Time.deltaTime;
            item.transform.position = Vector3.Lerp(start, end, t / spawnDuration);
            yield return null;
        }

        item.transform.position = end;

        // 出現しきってから物理ON
        if (rb != null)
        {
            rb.isKinematic = false;
        }
    }

    // ===== 表示制御 =====
    void ShowOnly(GameObject target)
    {
        foreach (var obj in loopModels)
            obj.SetActive(false);

        usedModel.SetActive(false);
        target.SetActive(true);
    }
}
