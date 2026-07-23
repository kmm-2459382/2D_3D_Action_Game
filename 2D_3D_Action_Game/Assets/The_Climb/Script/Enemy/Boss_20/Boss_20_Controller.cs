using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem.Utilities;
using System;
using System.Net.Http.Headers;



public class Boss_20_Controller : MonoBehaviour
{
    public Boss_20_StatusObjectScript status;   //Assetファイル
    public GameObject Bullet_Prefab;            //弾のPrehab
    public Transform Bullet_Position;　　　　　 //弾の発射位置
    public Transform Player;

    private int Enemy_Left_Max;                 //敵の移動は左の範囲
    private int Enemy_Right_Max;　　　　　　　　//敵の移動は右の範囲
    private float Enemy_Vertical;                 //敵の縦移動
    private int Boss_Move_Direction;　　　　　  //敵の動く方向
    private float Bullet_Timer;　　　　　　　　 //弾を発射するまでの時間
    private float rest_Timer;　　　　　　　　　 //休憩時間
    private float boss_Speed;　　　　　　　　　 //ボスの速さ
    private bool isResting = false;　　　　　　 //ボスの動くかどうかの判定
    private Rigidbody rb;
    private float Wave = 5.0f;                  //揺れ動く回数
    

    //初期化処理
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Initialize();

    }
    void Update()
    {
        HandleShooting();
    }

    void FixedUpdate()
    {
        Move();
    }

    //ボスの初期状態の設定
    void Initialize()
    {
        Bullet_Timer = status.Attack;
        Boss_Move_Direction = status.LEFT;
        rest_Timer = status.Rest;
        boss_Speed = status.Speed;
        Enemy_Left_Max = status.LEFT_Max;
        Enemy_Right_Max = status.RIGHT_Max; 
        Enemy_Vertical = status.Vertical;

    }

    //ボスの動き
    void Move()
    {
        if (!isResting)
        {
            Vector3 waveMotion = new Vector3(0f, Mathf.Sin(Time.time * Wave) * Enemy_Vertical, 0f);

            Vector3 newPosition = rb.position + new Vector3(boss_Speed * Boss_Move_Direction * Time.fixedDeltaTime, 0f) + waveMotion;
            EnemyMovementRange(ref newPosition);
            rb.MovePosition(newPosition);
            rest_Timer -= Time.fixedDeltaTime;
        }

        if (rest_Timer <= 0f && !isResting)
        {
            isResting = true;
            StartCoroutine(RestAndResume());
        }
    }

   

    //ボスの休憩
    IEnumerator RestAndResume()
    {
        Debug.Log("減速開始");
        float decelerationRate = 2f;

        while (boss_Speed > 0f)
        {
            boss_Speed = Mathf.MoveTowards(boss_Speed, 0f, decelerationRate * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }

        boss_Speed = 0f;
        Debug.Log("停止完了");
        yield return new WaitForSeconds(3f);
        Debug.Log("休憩終了");
        boss_Speed = status.Speed;
        rest_Timer = status.Rest;
        isResting = false;
    }

    //動く方向の指定
     void EnemyMovementRange(ref Vector3 nextPosition)
    {
        if(nextPosition.x <= Enemy_Left_Max)
        {
                      
            Boss_Move_Direction = status.RIGHT;
        }
        
        else if(nextPosition.x >= Enemy_Right_Max)
        {
           
            Boss_Move_Direction = status.LEFT;
        }
    }

    //弾の発射タイミング
    void HandleShooting()
    {
        Bullet_Timer -= Time.deltaTime;
        if (Bullet_Timer <= 0f)
        {
            Bullet();
            Bullet_Timer = status.Attack;
        }
    }

    //発射　　　　　　　　　　
    void Bullet()
    {
        if (!isResting)
        {
          GameObject player = GameObject.FindWithTag("Player");
          if (player == null) return;

          float spacing = 0.5f; // 横の間隔（好みに応じて調整）

          Vector3 basePosition = Bullet_Position.position;
          Quaternion rotation = Bullet_Position.rotation;

          // -1.5, -0.5, +0.5, +1.5 にオフセット（左右2個ずつ均等に）
          float[] offsets = new float[] { -1.5f, 1.5f };

          foreach (float offset in offsets)
          {
            Vector3 spawnPos = basePosition + Bullet_Position.right * offset * spacing;
            GameObject bullet = Instantiate(Bullet_Prefab, spawnPos, rotation);

            BossBullet bulletScript = bullet.GetComponent<BossBullet>();
            if (bulletScript != null)
            {
                bulletScript.status = this.status;
                bulletScript.Initialize(Player);
            }
          }
        }
       

    }

}
