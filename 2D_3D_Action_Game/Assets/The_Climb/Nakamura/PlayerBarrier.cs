using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class PlayerBarrier : MonoBehaviour
{
    private Transform barrier;

    public bool barrierActive = false;    //バリア中判定
    private bool barrierChargeOK = false;  //バリア発動可能か
    public bool unlocking = false;        //バリア解除中
    //private float barrierDuration = 10f;  //バリアの効果時間
    private float barrierCoolTime = 7f;   //クールタイム
    private float barrierTimer = 0f;      //バリアの時間関係

    void Start()
    {
        barrier = transform.GetChild(0);

        barrierTimer = barrierCoolTime;
    }

    void Update()
    {
        //押してバリア発動
        if (barrierChargeOK)
        {
            BarrierStart();
        }

        //タイマー計測
        if (!barrierChargeOK)
        {
            barrierTimer -= Time.deltaTime;
        }

        if (barrierTimer < 0) {
            //バリア効果時間終了
            //if (barrierActive && !unlocking)
            //{
            //    StartCoroutine(BarrierFinish());
            //}
            if (!barrierChargeOK && !barrierActive) 　//バリアクールダウン終了
            {
                barrierChargeOK = true;
                //Debug.Log("可能");
            }
        }
    }

    //バリア発動
    private void BarrierStart()
    {
        barrierChargeOK = false;
        barrierActive = true;
        //barrierTimer = barrierDuration;
        StartCoroutine(BarrierScaleChange(1.6f, 0.07f, true));
        //Debug.Log("発動");
    }

    //バリア終わり
    public IEnumerator BarrierFinish()
    {
        barrierTimer = barrierCoolTime;
        unlocking = true;
        StartCoroutine(BarrierScaleChange(2.9f, 0.05f, false));
        //Debug.Log("解除");

        yield return new WaitForSeconds(0.4f);
        barrierActive = false;
        unlocking = false;
    }

    //バリアのスケール操作
    IEnumerator BarrierScaleChange(float toScale, float duration, bool active)
    {
        //バリア出現時の初期化
        if (active)
        {
            barrier.gameObject.SetActive(true);
            barrier.localScale = new Vector3(0f, 0f, 0f);
        }

        Vector3 fromScale = barrier.localScale;  //スケール初期値
        float timer = 0f;  //経過時間

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            barrier.localScale = Vector3.Lerp(fromScale, new Vector3(toScale, toScale, toScale), t);

            yield return null;
        }

        barrier.gameObject.SetActive(active);
    }

    private float lastBlockTime = -10f; // 最後に防御した時間

    // 攻撃を防げるか判定するメソッド
    public bool TryBlockAttack()
    {
        // 1. 無敵時間チェック (バリア解除後1秒間は無敵)
        if (Time.time < lastBlockTime + 1.0f)
        {
            return true; // 無敵なので防いだことにする
        }

        // 2. バリア有効チェック
        if (barrierActive && !unlocking)
        {
            Debug.Log("バリアで攻撃を無効化しました");
            StartCoroutine(BarrierFinish()); // バリア解除

            lastBlockTime = Time.time; // 無敵時間開始
            return true; // 防いだ
        }

        return false; // 防げなかった
    }
}
