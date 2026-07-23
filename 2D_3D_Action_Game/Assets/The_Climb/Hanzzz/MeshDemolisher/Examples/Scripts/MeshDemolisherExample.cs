using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

namespace Hanzzz.MeshDemolisher
{
    public class MeshDemolisherExample : MonoBehaviour
    {
    
        [SerializeField] private GameObject targetGameObject;　　　//破壊対象のもとになるオブジェクト
        [SerializeField] private Transform breakPointsParent;　　　//どこで・どう割るかを決めるための目印
        [SerializeField] private Material interiorMaterial;　　　　//切れ目の中身のMaterial

        [SerializeField] private Transform resultParent;                   //破片のオブジェクト
        [SerializeField, Range(0f, 1f)] private float resultScale = 0.9f;　//破片のスケール倍率
        [SerializeField] private float shrinkDuration = 1.0f;　　　　　　　//飛んだ破片が縮小する時間
        [SerializeField] private float fallTimeBeforeShrink = 1.5f; 　　　 //落下してから縮小するまで猶予

     
        [SerializeField] private int maxFallingPieces = 8;　　　　　　　　 //飛ばす破片の数
        [SerializeField] private int processPerFrame = 3;　　　　　　　　　//1フレームで物理を有効にする数


        private  MeshDemolisher meshDemolisher = new MeshDemolisher();　//破片のGameObjectを生成するクラス　　

        private bool requestDemolish;　　　　　　　　　　　　　　　　　　　//外部からの要求フラグ
        private bool isDemolished;　　　　　　　　　　　　　　　　　　　　 //二重破壊防止用

        /// =============================
        /// 実行時に割れ目を作る
        /// =============================
        private void Awake()
        {
            meshDemolisher = new MeshDemolisher();
            Demolish();
        }
        // =============================
        // プレイヤーが乗ったかの判定or爆弾があったかの判定
        // =============================
        public void RequestDemolish()
        {
            if (isDemolished) return;
            requestDemolish = true;
        }

        private void Update()
        {
            if (!requestDemolish) return;

            requestDemolish = false;
            isDemolished = true;
            StartCoroutine(DemolishFlow());
        }

        // =============================
        // 破壊フロー（完成形）
        // =============================
        private IEnumerator DemolishFlow()
        {
            //初期化する
            foreach (Transform c in resultParent)
            Destroy(c.gameObject);

            //初期化するまでの待つ
            yield return null;

            List<Transform> breakPoints = new List<Transform>();
            int count = Mathf.Min(maxFallingPieces, breakPointsParent.childCount);

            //指定した数をListに追加する
            for (int i = 0; i < count; i++)
            breakPoints.Add(breakPointsParent.GetChild(i));

            //処理がどのくらい掛かるかの測定用
            //var watch = System.Diagnostics.Stopwatch.StartNew();

            //破片を受け取る箱
            List<GameObject> pieces = meshDemolisher.Demolish(targetGameObject, breakPoints, interiorMaterial);

            //watch.Stop();

            //破片の配置
            foreach (GameObject p in pieces)
            {
                p.transform.SetParent(resultParent, true);
                p.transform.localScale = resultScale * Vector3.one;
            }

            DisableBreakPointColliders();

            yield return StartCoroutine(ActivateGravityOnly());
            yield return new WaitForSeconds(fallTimeBeforeShrink);
            yield return StartCoroutine(ShrinkPieces());
        }

        // =============================
        // 破片に重力を付与する
        // =============================
        private IEnumerator ActivateGravityOnly()
        {
            int processed = 0;

            foreach (Transform piece in resultParent)
            {
                if (!piece.TryGetComponent(out Collider _))
                    piece.gameObject.AddComponent<BoxCollider>();

                Rigidbody rb = piece.gameObject.AddComponent<Rigidbody>();
                rb.useGravity = true;
                rb.isKinematic = false;
                rb.WakeUp();

                //処理を軽くする為
                processed++;
                if (processed >= processPerFrame)
                {
                    processed = 0;
                    yield return null;
                }
            }

            foreach (Transform piece in resultParent)
            {
                if (piece == null) continue;

                foreach (Collider col in piece.GetComponentsInChildren<Collider>())
                    col.enabled = false;
            }
        }

        // =============================
        // 縮小して消す
        // =============================
        private IEnumerator ShrinkPieces()
        {
            float t = 0f;

            while (t < shrinkDuration)
            {
                float scale = Mathf.Lerp(resultScale, 0f, t / shrinkDuration);

                foreach (Transform c in resultParent)
                    if (c != null)
                        c.localScale = scale * Vector3.one;

                t += Time.deltaTime;
                yield return null;
            }

            foreach (Transform c in resultParent)
                if (c != null)
                    Destroy(c.gameObject);
        }

        // =============================
        // 壊した破片のColliderをOFFにする
        // =============================
        private void DisableBreakPointColliders()
        {
            foreach (Collider col in breakPointsParent.GetComponentsInChildren<Collider>())
            {
                col.enabled = false;
            }
        }

        [ContextMenu("割れ目の調整（調整用）")]
        public void Demolish()
        {
            Enumerable.Range(0, resultParent.childCount).Select(i => resultParent.GetChild(i)).ToList().ForEach(x => DestroyImmediate(x.gameObject)); List<Transform> breakPoints = Enumerable.Range(0, breakPointsParent.childCount).Select(x => breakPointsParent.GetChild(x)).ToList();
            var watch = System.Diagnostics.Stopwatch.StartNew(); List<GameObject> res = meshDemolisher.Demolish(targetGameObject, breakPoints, interiorMaterial); 
            watch.Stop();  res.ForEach(x => x.transform.SetParent(resultParent, true));
            Enumerable.Range(0, resultParent.childCount).Select(i => resultParent.GetChild(i)).ToList().ForEach(x => x.localScale = resultScale * Vector3.one);
            targetGameObject.SetActive(false); 
        }

        [ContextMenu("割れ目の調整（実行中用）")] 
        public async void DemolishAsync() 
        {
            targetGameObject.SetActive(true);
            Enumerable.Range(0, resultParent.childCount).Select(i => resultParent.GetChild(i)).ToList().ForEach(x => DestroyImmediate(x.gameObject));
            List<Transform> breakPoints = Enumerable.Range(0, breakPointsParent.childCount).Select(x => breakPointsParent.GetChild(x)).ToList();
            var watch = System.Diagnostics.Stopwatch.StartNew(); List<GameObject> res = await meshDemolisher.DemolishAsync(targetGameObject, breakPoints, interiorMaterial); 
            watch.Stop(); res.ForEach(x => x.transform.SetParent(resultParent, true));
            Enumerable.Range(0, resultParent.childCount).Select(i => resultParent.GetChild(i)).ToList().ForEach(x => x.localScale = resultScale * Vector3.one); 
            targetGameObject.SetActive(false);
        }


        // =============================
        // 元に戻す
        // =============================
        [ContextMenu("初期の状態に戻す")]
        public void Reset()
        {
            isDemolished = false;
            requestDemolish = false;

            foreach (Transform c in resultParent)
            DestroyImmediate(c.gameObject);

            // breakPoint Collider を戻す
            foreach (Collider col in breakPointsParent.GetComponentsInChildren<Collider>())
            col.enabled = true;

        }
    }
}