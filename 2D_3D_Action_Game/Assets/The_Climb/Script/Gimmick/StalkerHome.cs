using System.Collections;
#if UNITY_EDITOR
using UnityEditor.ShortcutManagement;
#endif
using UnityEngine;
using UnityEngine.Playables;
using Zenject.SpaceFighter;

public class StalkerHome : MonoBehaviour
{
    [SerializeField] GameObject stalkerPrefab;
    BuddyController buddy;

    public bool spawned = false;      //スポーン済か判定
    private bool showing = false;     //画面内にあるか判定
    private bool called = false;      //ストーカーハンド生成関数呼び出したか判定
    private GameObject myStalker;     //生み出す敵


    void Start()
    {
        if (GameObject.FindWithTag("Buddy") != null)
        {
            buddy = GameObject.FindWithTag("Buddy").GetComponent<BuddyController>();
        }
    }

    void Update()
    {
        //ストーカーハンド、スポーン
        if (!called && showing)
        {
            StartCoroutine(StalkerSpawn());
        }

        // myStalkerがnull（未生成）または非アクティブ（倒された/消えた）なら再生成フラグを落とす
        if ((myStalker == null || !myStalker.activeSelf) && !buddy.beingKidnapped && spawned)
        {
            spawned = false;
            called = false;
        }
    }

    private IEnumerator StalkerSpawn()
    {
        called = true;

        yield return new WaitForSeconds(1.5f);

        if (myStalker == null)
        {
            // 初回生成
            myStalker = Instantiate(stalkerPrefab, transform);
        }
        else
        {
            // 2回目以降は再利用
            myStalker.transform.position = transform.position; // 位置をHomeに戻す
            myStalker.SetActive(true);
        }
        
        spawned = true;
        //myStalker.transform.SetParent(transform);
    }

    private void OnBecameVisible()
    {
        showing = true;
    }

    private void OnBecameInvisible()
    {
        showing = false;
    }
}
