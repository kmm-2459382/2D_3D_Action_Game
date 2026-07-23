using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MimicSpawner : MonoBehaviour
{
    [Header("生成設定")]
    public GameObject mimicPrefab;
    public int spawnCount = 10;       // 生成する数
    public float spawnInterval = 1f;  // 生成間隔
    public float baseDelay = 1f;      // Mimicの遅延時間の基準値

    private bool triggered = false;

    private void Awake()
    {
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;
        GetComponent<Collider>().enabled = false;

        StartCoroutine(SpawnMimics());
    }

    private IEnumerator SpawnMimics()
    {
        // 🕒 最初の Mimic は baseDelay 秒待ってから出現
        yield return new WaitForSeconds(baseDelay);

        for (int i = 0; i < spawnCount; i++)
        {
            GameObject obj = Instantiate(mimicPrefab, transform.position, Quaternion.identity);
            MimicFollower follower = obj.GetComponent<MimicFollower>();
            if (follower != null)
            {
                // 各 Mimic に異なる遅延時間を設定
                follower.delay = baseDelay + (spawnInterval * i);
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
