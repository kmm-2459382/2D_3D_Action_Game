using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class TutorialBoard : MonoBehaviour
{
    private GameObject visual;
    private GameObject tutorial1;
    private GameObject tutorial2;
    [Header("常に表示させたい場合\nalwaysAppearedをtrueに")]
    [SerializeField] private bool alwaysAppeared = false;  //常に出現しているか
    [Header("チュートリアル画像を二枚使うなら\nimageChangeをtrueに")]
    [SerializeField] private bool imageChange = false;  //チュートリアル用の画像を切り替えるか
    [Header("画像の切り替わる間隔の時間")]
    [SerializeField] private float changeCoolTime = 1f;  //チュートリアル画像の切り替え時間
    [Header("ビデオプレイヤー")]
    [SerializeField] private VideoPlayer videoPlayer;  //チュートリアル画像の切り替え時間
    private float changeTimer = 0f;  //チュートリアル画像の切り替えタイマー
    private bool displayed = false;  //今表示されているか判定
    private float currentY = 0f;  //現在のYのScale
    private float appearSpeed = 5f;  //出現・消滅スピード

    private void Awake()
    {
        if (videoPlayer != null)    //  チュートリアルビデオ再生準備
        {
            videoPlayer.playOnAwake = false;
            videoPlayer.time = 0;
            videoPlayer.Prepare();
            videoPlayer.prepareCompleted += OnPrepared;
        }
    }

    void Start()
    {
        Transform parent = transform.parent;
        visual = parent.Find("Visual").gameObject;
        tutorial1 = visual.transform.Find("Tutorial1")?.gameObject;
        tutorial2 = visual.transform.Find("Tutorial2")?.gameObject;

        //初期セッティング
        if (alwaysAppeared)
        {
            displayed = true;
            currentY = 1f;
            visual.transform.localScale = Vector3.one;
            GetComponent<Collider>().enabled = false;  //当たり判定無効
        }
        else
            visual.transform.localScale = new Vector3(1, 0, 1);
        if (tutorial1 != null && tutorial2 != null)
        {
            tutorial1.SetActive(true);
            tutorial2.SetActive(false);
        }
    }

    void Update()
    {
        //看板の画像消り替え
        if (imageChange && displayed)
        {
            changeTimer += Time.deltaTime;

            if (changeTimer >= changeCoolTime)
            {
                tutorial1.SetActive(!tutorial1.activeSelf);
                tutorial2.SetActive(!tutorial2.activeSelf);
                //タイマーリセット
                changeTimer = 0f;
            }
        }
    }

    void OnPrepared(VideoPlayer vp)    //  ビデオ再生準備ができた時に動く
    {
        vp.time = 0;
        vp.Play();
    }

    //看板出現
    private IEnumerator BoardAppear()
    {
        while (currentY < 1f)
        {
            //途中で離れれば出現は終わり
            if (!displayed)
                yield break;

            //加速成長
            currentY += Mathf.Max(0.01f, currentY) * appearSpeed * Time.deltaTime;
            currentY = Mathf.Min(currentY, 1f);

            visual.transform.localScale = new Vector3(1f, currentY, 1f);
            yield return null;
        }

        //調整用
        currentY = 1f;
        visual.transform.localScale = Vector3.one;
    }

    //看板消えてなくなれ
    private IEnumerator BoardDisappear()
    {
        while (currentY > 0f)
        {
            //途中で離れれば出現は終わり
            if (displayed)
                yield break;

            // 大きいほど速く、小さいほどゆっくり消える
            currentY -= Mathf.Max(0.01f, currentY) * appearSpeed * Time.deltaTime;
            currentY = Mathf.Max(currentY, 0f);

            visual.transform.localScale = new Vector3(1f, currentY, 1f);
            yield return null;
        }

        //調整用
        currentY = 0f;
        visual.transform.localScale = new Vector3(1f, 0f, 1f);

        //完全消滅のためリセット
        changeTimer = 0f;
        if (tutorial1 != null && tutorial2 != null)
        {
            tutorial1.SetActive(true);
            tutorial2.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            //看板出現
            displayed = true;
            StartCoroutine(BoardAppear());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            //看板消えてなくなれ
            displayed = false;
            StartCoroutine(BoardDisappear());
        }
    }
}
