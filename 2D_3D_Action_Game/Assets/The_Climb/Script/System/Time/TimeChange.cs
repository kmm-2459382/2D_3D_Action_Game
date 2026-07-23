//using NUnit.Framework.Constraints;
//using System.Runtime.CompilerServices;
//using Unity.VisualScripting;
//using UnityEngine;

//public class TimeChange : MonoBehaviour    //    天体の制御のために生成じゃなくて直置きにさせてください。(松山)
//{
//    [Header("ステージセレクト")]
//    [SerializeField] private GameObject[] MapPrefabs;
//    private GameObject[] MapInstance;

//    [Header("プレイヤーの参照")]
//    [SerializeField] private Transform Player;

//    [Header("フェード制御")]
//    [SerializeField] private ScreenFader fader;

//    [Header("クールダウン時間(秒)")]
//    [SerializeField] private float switchCooldown = 2f;

//    public KeyBind KeyBind;                   //プレイヤーのキーを取得

//    private int CurrentActiveIndex = 0;
//    //private SafeSpawner spawner;


//    void Start()
//    {
//        KeyBind = GameObject.Find("KeyManager").GetComponent<KeyBind>();
//        //spawner = Player.GetComponent<SafeSpawner>();

//        MapInstance = new GameObject[MapPrefabs.Length];
//        for (int i = 0; i < MapPrefabs.Length; i++)
//        {
//            MapInstance[i] = Instantiate(MapPrefabs[i], Vector3.zero, Quaternion.identity);
//            MapInstance[i].SetActive(false);
//        }

//        MapInstance[CurrentActiveIndex].SetActive(true);
//    }


//    //void Update()
//    //{
//    //    if (!fader.IsFading && Input.GetKeyDown(KeyBind.timeSwitch))
//    //    {
//    //        fader.FadeAndDo(SwitchToNextMap);
//    //    }
//    //}

//    private void SwitchToNextMap()
//    {
//        //シーン内にいるすべての敵を消す
//        EnemyGeneration[] EnemyDelete = Object.FindObjectsByType<EnemyGeneration>(FindObjectsSortMode.InstanceID);
//        foreach (EnemyGeneration Generator in EnemyDelete)
//        {
//            Generator.ClearAllEnemy();
//        }


//        //現代のマップを非表示
//        MapInstance[CurrentActiveIndex].SetActive(false);
        
//        //次のマップへ切り替え
//        CurrentActiveIndex++;
//        if (CurrentActiveIndex >= MapInstance.Length)
//            CurrentActiveIndex = 0;

//        //次のマップの生成
//        MapInstance[CurrentActiveIndex].SetActive(true);

//        //新しいマップの敵生成andマップが現代のマップの時だけ出撃調整する
//        if(CurrentActiveIndex == 0)
//        {
//            var Analyzer = FindFirstObjectByType<EnemyKillAnalyzer>();
//            if(Analyzer != null)
//            {
//                var KillRatios = Analyzer.GetKillRatio();
//                var Generator = FindFirstObjectByType<EnemyGeneration>();
//                if(Generator != null)
//                {
//                    Generator.AbjustSpawanByKillRatio(KillRatios);
//                    Debug.Log("現代のマップなので出撃調整を行いました");
//                }
//            }
//        }

//        //新しいマップの敵生成and通常生成を行う
//        else if(CurrentActiveIndex == 1)
//        {
//            var Generator = FindFirstObjectByType<EnemyGeneration>();
//            if(Generator != null)
//            {
//                Generator.AbjustSpawanByKillRatio(null);
//                Debug.Log("通常マップの生成を行いました");
//            }
//        }

//        else
//        {
//            Debug.LogError("EnemyGenerationがHierarchyに存在しません");
//        }
//    }
//}