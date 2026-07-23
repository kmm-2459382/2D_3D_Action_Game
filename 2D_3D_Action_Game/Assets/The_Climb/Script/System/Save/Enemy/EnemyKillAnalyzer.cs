using UnityEngine;
using SQLite4Unity3d;
using System.Linq;
using System.IO;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.Overlays;
#endif

public class EnemyKillAnalyzer : MonoBehaviour
{
    private EnemyDataBase dbManager;
    private SQLiteConnection Connection;

    private void Awake()
    {
        dbManager = FindFirstObjectByType<EnemyDataBase>();
        if(dbManager == null)
        {
            Debug.LogError("EnemyDataBaseが見つけられません");
            return;
        }

        Connection = dbManager._connection;
    }


    /// <summary>
    /// 敵の撃破データの割合を計算して表示
    /// </summary>
    [ContextMenu("Show Kill Ratio")]
    public Dictionary<string, float> GetKillRatio()
    {
        if(Connection == null)　return　null;

        string TempPath = dbManager.TempDbPath;
        string SavePath = dbManager.SaveDbPath;

        List<EnemyKillData> TempData = new();
        List<EnemyKillData> SaveData = new();

        if (File.Exists(TempPath))
        {
            using (var TempConn = new SQLiteConnection(TempPath, SQLiteOpenFlags.ReadWrite))
            {
                TempData = TempConn.Table<EnemyKillData>().ToList();
            }
        }

  
        if (File.Exists(SavePath))
        {
            using (var SaveConn = new SQLiteConnection(SavePath, SQLiteOpenFlags.ReadWrite))
            {
                SaveData = SaveConn.Table<EnemyKillData>().ToList();
            }
        }

        // 両方を統合
        var allData = TempData.Concat(SaveData)
            .GroupBy(e => e.EnemyName)
            .Select(g => new EnemyKillData
            {
                EnemyName = g.Key,
                KillCount = g.Sum(x => x.KillCount)
            })
            .ToList();

        if (allData.Count == 0) return null;
        

        // 合計撃破数
        int totalKills = allData.Sum(e => e.KillCount);

        // 割合を計算してログ出力
        var Result = allData
            .OrderByDescending(e => e.KillCount)
            .Select(e => new
            {
                e.EnemyName,
                e.KillCount,
                Ratio = (float)e.KillCount / totalKills * 100f
            }).ToList();
        Debug.Log($"📊 撃破データ分析結果（合計 {totalKills} 体）");
        foreach (var r in Result)
        {
            Debug.Log($"・{r.EnemyName}：{r.KillCount}体 ({r.Ratio:F1}%)");
        }

        //敵の名前、やられた回数
        var ResultList = Result.ToDictionary(r => r.EnemyName, r => r.Ratio);
        return ResultList;
     
    }
}