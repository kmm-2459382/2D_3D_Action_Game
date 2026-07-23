using SQLite4Unity3d;

public class EnemyKillData
{
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }               //← DB用の一意ID（自動採番）


    public int EnemyID         { get; set; }  //エネミーのID
    public string EnemyName　  { get; set; }  //エネミーの名前
    public string EnemyPeriod  { get; set; }  //エネミーの時代
    public string AreaName     { get; set; }　//エリアの名前
    public int KillCount       { get; set; }　//やられた回数
    public string LastKillTime { get; set; }  //時間

    public EnemyKillData() { }
    public EnemyKillData(EnemyStats stats, string areaName, int killCount, string lastKillTime)
    {
        this.EnemyID = stats.ID;
        this.EnemyName = stats.EnemyName;
        this.EnemyPeriod = stats.Period;
        this.AreaName = areaName;
        this.KillCount = killCount;
        this.LastKillTime = lastKillTime;
    }
}
