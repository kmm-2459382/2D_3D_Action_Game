using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class EnemyGeneration : MonoBehaviour
{
    [Header("敵のデータリスト")]
    [SerializeField] private List<EnemyStats> EnemyStatsList = new List<EnemyStats>();

    [Header("敵の出現場所の設定")]
    [SerializeField] private List<Transform> EnemySpotList = new List<Transform>();

    [Header("フィールド上の敵のリスト")]
    private List<GameObject> FieldEnemies = new List<GameObject>();

    //出現調整するデータ
    private Dictionary<string, float> EnemyWeights = new();

    //現代か過去かの判定
    bool Past = false;

    void Start()
    {
        CreateEnemy();
    }

    ///<summy>
    ///撃破率で出現する敵の数を変更
    ///</summy>
    public void AbjustSpawanByKillRatio(Dictionary<string,float> KillRatios)
    {
        if(KillRatios == null || KillRatios.Count == 0)
        {
            Debug.Log("データがない為通常生成を行います");
            Past = true;
            CreateEnemy();
            return;
        }

        Past = false;
        //初期化
        EnemyWeights.Clear();
        foreach( var EnemyList in EnemyStatsList)
        {
            float Ratio = KillRatios.ContainsKey(EnemyList.EnemyName) ? KillRatios[EnemyList.EnemyName] : 0f;
            EnemyWeights[EnemyList.EnemyName] = Mathf.Clamp01(1f - (Ratio / 100f));
        }

        CreateEnemy();
    }


    ///<summy>
    ///敵の生成
    ///</summy>>
    void CreateEnemy()
    {
        if (FieldEnemies.Count > 0)
        {
            Debug.LogWarning("すでに敵が存在するため新規生成をスキップします");
            return;
        }

        foreach (Transform spot in EnemySpotList)
        {
            EnemyStats enemyData = GetWeightedRandomEnemy();
            GameObject obj = Instantiate(enemyData.EnemyPrefab, spot.position, Quaternion.identity);
            obj.name = enemyData.EnemyName;

            Enemy enemyBase = obj.GetComponent<Enemy>();
            if (enemyBase != null)
            {
                enemyBase.SetUp(enemyData, this);
                FieldEnemies.Add(obj);
            }
        }
    }

    //出撃調整
    private EnemyStats GetWeightedRandomEnemy()
    {
        if (EnemyWeights.Count == 0 || Past == true)
            return EnemyStatsList[Random.Range(0, EnemyStatsList.Count)];

        float totalWeight = EnemyWeights.Values.Sum();
        float random = Random.value * totalWeight;
        float current = 0;

        foreach (var enemy in EnemyStatsList)
        {
            float w = EnemyWeights[enemy.EnemyName];
            current += w;
            if (random <= current)
                return enemy;
        }

        return EnemyStatsList.Last(); // 保険
    }


    ///<summy>
    ///時間の切り替えで敵を削除
    ///</summy>
public void ClearAllEnemy()
    {
        foreach (GameObject AllEnemy in FieldEnemies)
        {
            if (AllEnemy != null)
            {
                Destroy(AllEnemy);
            }
        }
        FieldEnemies.Clear();
        Debug.Log("敵を全部消しました");
    }

    /// <summary> 
    /// 敵削除時に呼ばれる
    /// </summary>
    public void RemoveEnemy(GameObject enemy)
    {
      if (FieldEnemies.Contains(enemy))
      {
            FieldEnemies.Remove(enemy);
            Debug.Log($"敵 {enemy.name} を削除しました。現在の敵数: {FieldEnemies.Count}");
      }
    }
}

  

