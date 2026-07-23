using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class KeyCollector : MonoBehaviour
{
    [System.Serializable]
    public class KeyData
    {
        [Header("識別ID")]
        public string keyID = "KeyA";

        [Header("必要な欠片数")]
        public int requiredFragments = 5;

        [HideInInspector] public int currentFragments = 0;
        [HideInInspector] public bool keyCreated = false;

        [Header("完成時に出す鍵Prefab")]
        public GameObject keyPrefab;

        [Header("対応するロックオブジェクト")]
        public LockObject linkedLock;

        [Header("鍵完成イベント")]
        public UnityEvent onKeyCompleted;
    }

    [Header("プレイヤー参照")]
    public Transform player;

    [Header("鍵を追従させるオフセット")]
    public Vector3 followOffset = new Vector3(0, 1.5f, 0);

    [Header("鍵追従速度")]
    public float followSpeed = 5f;

    [Header("管理する鍵リスト")]
    public List<KeyData> keys = new List<KeyData>();

    public void CollectFragment(string keyID)
    {
        var key = keys.Find(k => k.keyID == keyID);
        if (key == null)
        {
            Debug.LogWarning($"未登録の鍵ID: {keyID}");
            return;
        }

        if (key.keyCreated) return;

        key.currentFragments++;
        Debug.Log($"[{name}] {key.keyID}: {key.currentFragments}/{key.requiredFragments} に増えた");

        if (key.currentFragments >= key.requiredFragments)
        {
            Debug.Log($"[{name}] → 条件達成！ CreateKey を実行");
            CreateKey(key);
        }
    }

    private void CreateKey(KeyData key)
    {
        key.keyCreated = true;
        Debug.Log($"🔑 {key.keyID} の鍵が完成しました！");

        // 鍵Prefabを生成
        if (key.keyPrefab != null)
        {
            GameObject keyObj = Instantiate(key.keyPrefab, transform.position + Vector3.up * 1.5f, Quaternion.identity);

            // プレイヤー追従スクリプトを付与
            FollowPlayer follow = keyObj.AddComponent<FollowPlayer>();
            follow.target = player;
            follow.offset = followOffset;
            follow.followSpeed = followSpeed;
        }

        // イベント発火
        key.onKeyCompleted?.Invoke();

        // 対応するロック解除
        if (key.linkedLock != null)
        {
            key.linkedLock.Unlock();
        }
    }
}
