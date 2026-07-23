using UnityEngine;
using SQLite4Unity3d;
using System.IO;
using System.Linq;
using System;
public class EnemyDataBase : MonoBehaviour
{
    public  SQLiteConnection _connection;
    [HideInInspector]
    public  string SaveDbPath;
    [HideInInspector]
    public  string TempDbPath;
    private bool IsSaveConfirmed = false;


    /// <summary>
    /// データベースの保存場所
    /// </summary>
    private void Awake()
    {
        TempDbPath = Path.Combine(Application.persistentDataPath, "EnemykillData_temp.db");
        SaveDbPath = Path.Combine(Application.persistentDataPath, "EnemyKillData.db");

        //新しいデータを作成
        _connection = new SQLiteConnection(TempDbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);

        // テーブル作成（存在しなければ作成）
        _connection.CreateTable<EnemyKillData>();

    }

 
    ///敵撃破データを追加又は更新
    ///</summray>
    public void AddOrUpdateKillData(EnemyStats stats, string areaName)
    {

        if (_connection == null)
        {
            Debug.Log("初期化されてません");
            return;
        }

        if(stats.Period != "過去")
        {
            Debug.Log($"{stats.EnemyName}は{stats.Period}の敵のため記録しません");
            return;
        }

        //既存データ検索
        var existingDate = _connection.Table<EnemyKillData>()
        .FirstOrDefault(e => e.EnemyID == stats.ID && e.AreaName == areaName);

        string now = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

        //1度倒したことがある敵
        if (existingDate != null)
        {
            existingDate.KillCount += 1;
            existingDate.LastKillTime = now;
            _connection.Update(existingDate);
            Debug.Log($"【撃破記録】{existingDate.EnemyName} を倒した！ " +
                  $"場所: {existingDate.AreaName} / 累計: {existingDate.KillCount} 回 / 最終: {existingDate.LastKillTime}");
        }

        //初めて倒した敵
        else
        {
            var newDate = new EnemyKillData(stats, areaName, 1, now);
            _connection.Insert(newDate);
            Debug.Log($"【撃破記録】{newDate.EnemyName} を倒した！ " +
                 $"場所: {newDate.AreaName} / 累計: {newDate.KillCount} 回 / 最終: {newDate.LastKillTime}");
        }
    }

    ///<summy>
    ///保存ボタンを押して時に呼ぶ関数
    ///</summy>
    public void ConfiremSave()
    {
        _connection.Close();
        File.Copy(TempDbPath, SaveDbPath, true);
        Debug.Log("データを保存します");

        IsSaveConfirmed = true;
    }

    ///<summy>
    ///ゲーム終了時に呼び出される
    ///</summy>
    public void OnApplicationQuit()
    {
        _connection?.Close();

        if (!IsSaveConfirmed && File.Exists(TempDbPath))
        {
            File.Delete(TempDbPath);
            Debug.LogWarning("データを破棄しました");
        }
    }
}
