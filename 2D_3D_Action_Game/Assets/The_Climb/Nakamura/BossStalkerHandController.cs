using UnityEngine;
using System.Collections;

public class BossStalkerHandController : MonoBehaviour
{
    private GameObject player;
    private Transform mainStalker;
    private Transform childStalker1;
    private Transform childStalker2;
    private Transform childStalker3;
    private Transform[] stalkers = new Transform[4];

    private float waitTime = 3.0f;       //行動：Waitの時間
    private float stalkTime = 8.0f;      //行動：Stalkの時間
    private float stopTime = 0.5f;       //行動：Stopの時間
    private float stalkerTimer = 0f;     //行動ローテーション用タイマー
    private float speed = 3.0f;          //移動速度
    private float childRadius = 6f;      //追従の半径
    private Vector3 stalkTarget = Vector3.zero;  //追跡ターゲット
    private bool isSlowed = false;       //減速中フラグ

    void Start()
    {
        if (GameObject.FindWithTag("Player") != null)
        {
            player = GameObject.FindWithTag("Player");
        }

        mainStalker = transform.GetChild(3).gameObject.transform;
        childStalker1 = transform.GetChild(2).gameObject.transform;
        childStalker2 = transform.GetChild(1).gameObject.transform;
        childStalker3 = transform.GetChild(0).gameObject.transform;
        stalkers[0] = mainStalker;
        stalkers[1] = childStalker1;
        stalkers[2] = childStalker2;
        stalkers[3] = childStalker3;
    }

    void Update()
    {
        if (player.transform.position.x > 600f)
        {
            //現在のプレイヤーの座標を覚える
            stalkTarget = player.transform.position;

            //行動ローテーション用タイマー
            stalkerTimer += Time.deltaTime;

            //行動：Wait
            if (stalkerTimer <= waitTime)
            {
                //LookAt時にZ前→X前の補正を入れて狙いを定める
                mainStalker.transform.LookAt(stalkTarget);
                mainStalker.transform.Rotate(0f, -90f, 0f);
            }
            else if (stalkerTimer <= waitTime + stalkTime) //行動：Stalk
            {
                //ターゲットまでの方向ベクトル
                Vector3 targetDir = (stalkTarget - mainStalker.transform.position).normalized;

                //今の方向ベクトル
                Vector3 currentDir = mainStalker.transform.right;

                //角度
                float diffY = targetDir.y - currentDir.y;

                //曲がる
                float rotSpeed = 60f * Time.deltaTime; //角度調整速度
                float rotZ = Mathf.Clamp(diffY * 20f, -rotSpeed, rotSpeed); //小さく安定させる

                mainStalker.transform.localEulerAngles += new Vector3(0f, 0f, rotZ);

                //進む
                mainStalker.transform.position += mainStalker.transform.right * speed * Time.deltaTime;
            }
            else //Stop
            {
                StartCoroutine(ChildPosReset());

                if (stalkerTimer > waitTime + stalkTime + stopTime)
                {
                    stalkerTimer = 0f;
                }
            }
        }

        //ChildStalkerの挙動
        for (int i = 0; i < stalkers.Length - 1; i++)
            if (Vector3.Distance(stalkers[i].position, stalkers[i + 1].position) > childRadius)
            {
                //方向ベクトルを求める
                Vector3 direction = (stalkers[i + 1].position - stalkers[i].position).normalized;

                //childRadius の位置に押し戻す
                stalkers[i + 1].position = stalkers[i].position + direction * childRadius;
            }
    }

    //ChildStalkerをMainStalkerHandの位置に
    private IEnumerator ChildPosReset()
    {
        for (float j = 0; j < 0.5f; j += Time.deltaTime)
        {
            for (int i = 0; i < stalkers.Length - 1; i++)
                stalkers[i + 1].position = Vector3.Lerp(stalkers[i + 1].position, stalkers[i].position, 9f * Time.deltaTime);
            yield return null;
        }
    }

    //ボスストーカーハンド減速処理
    public void BossStalkerSlow()
    {
        StartCoroutine(SlowSpeed());
    }

    private IEnumerator SlowSpeed()
    {
        // 爆発の力で回転などがかかると挙動がおかしくなるため、ヒットするたびに毎回リセットする
        MeshRenderer[] renderers = new MeshRenderer[stalkers.Length];
        for (int i = 0; i < stalkers.Length; i++)
        {
            if (stalkers[i] != null)
            {
                renderers[i] = stalkers[i].GetComponent<MeshRenderer>();
                
                var rb = stalkers[i].GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
            }
        }

        if (isSlowed) yield break;

        isSlowed = true;

        //元の速度を保存
        float originalSpeed = speed;
        speed = originalSpeed * 0.33f;

        float duration = 5.0f;  //点滅持続時間
        float blinkInterval = 0.1f;  //点滅間隔
        float timer = 0f;

        //点滅処理
        while (timer < duration)
        {
            bool isVisible = Mathf.Repeat(timer, blinkInterval * 2) < blinkInterval;

            foreach (var r in renderers)
            {
                if (r != null)
                    r.enabled = isVisible;
            }

            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        //最後は必ず表示
        foreach (var r in renderers)
        {
            if (r != null)
                r.enabled = true;
        }

        //速度戻
        speed = originalSpeed;
        isSlowed = false;
    }
}

