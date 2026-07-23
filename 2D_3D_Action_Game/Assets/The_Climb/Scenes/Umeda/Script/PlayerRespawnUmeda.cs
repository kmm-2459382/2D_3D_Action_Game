using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;
using TheClimb.Astral;

public class PlayerRespawnUmeda : MonoBehaviour
{
    private Rigidbody rb;
    private PlayerMove playerMove;

    [Header("リスポーンポイントリスト")]
    public List<Transform> respawnPoints = new List<Transform>(); // 手動設定用リスト

    [Header("現在のリスポーンインデックス")]
    [SerializeField] private int currentIndex = 0;

    private Transform currentRespawnPoint;

    // チェックポイントの見た目制御用（同じ順番で登録）
    [Header("対応するチェックポイント見た目リスト")]
    public List<CheckpointVisual> checkpointVisuals = new List<CheckpointVisual>();

    [Header("リスポーン判定設定（チェックポイントからの相対距離）")]
    public float maxHeightFromCheckpoint = 30f; // 上方向の制限（これ以上離れるとリスポーン）
    public float fallDistanceFromCheckpoint = 20f; // 下方向の制限（これ以上落ちるとリスポーン）

    private bool buddyStage = false;
    private GameObject player;
    private PlayerState playerState;
    private BuddyCarry buddyCarry;

    public static Action OnPlayerRespawn;

    [Header("リスポーン時にリセットするスイッチ")]
    public List<Switch> switchesToReset = new List<Switch>();

    //[Header("衝撃球のリセットために必要なデータ")]
    //[SerializeField] List<ImpactBallRespornData> impactBallRespornDatas;    //  衝撃球のリセットに必要なデータ(簡易実装の為後からリファクタ)
    //[Header("今のステージが天体かどうか")]
    //[SerializeField] bool isAstralStage;    //  現在が宇宙ステージかを確認するフラグ(簡易実装のため後からリファクタ)


    // ---------------------------------------------------------
    // シーンリロードを跨いで値を保持するためのstatic変数
    // ---------------------------------------------------------
    private static int lastCheckpointIndex = -1;
    private static List<string> persistentButtonIDs = new List<string>();
    private static int kitanoSkillUseCount = -1; // Kitanoスキルの使用回数保持用

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerMove = GetComponent<PlayerMove>();

        //相棒ステージか判定
        if (SceneManager.GetActiveScene().name == "Nakamura")
        {
            buddyStage = true;
            player = GameObject.Find("PlayerModel");
            playerState = player.GetComponent<PlayerState>();
            buddyCarry = player.GetComponent<BuddyCarry>();

            // リロード直後かどうか判定：lastCheckpointIndexが有効なら復帰処理を行う
            if (lastCheckpointIndex >= 0)
            {
                StartCoroutine(RestoreStateDelay());
            }
        }
        
        // 初回起動時など（Nakamura以外、またはリロード直後でない場合）
        if (lastCheckpointIndex < 0 && respawnPoints.Count > 0)
        {
            SetRespawnPoint(0);
        }
        else if (respawnPoints.Count == 0)
        {
            Debug.LogWarning("⚠️ リスポーンポイントが未設定です。");
        }
    }

    // 1フレーム待ってから復元処理を行う（PressureButtonのStart待機用）
    private IEnumerator RestoreStateDelay()
    {
        yield return null; // 1フレーム待機

        // チェックポイント復帰
        if (lastCheckpointIndex < respawnPoints.Count)
        {
            SetRespawnPoint(lastCheckpointIndex);
            if (respawnPoints[lastCheckpointIndex] != null)
            {
                // 位置を強制同期（Rigidbodyなどがある場合も考慮）
                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.MovePosition(respawnPoints[lastCheckpointIndex].position);
                }
                transform.position = respawnPoints[lastCheckpointIndex].position;
            }
        }

        // PressureButtonの状態復帰
        if (persistentButtonIDs != null && persistentButtonIDs.Count > 0)
        {
            // シーン内の全PressureButtonを探す
            PressureButton[] allButtons = FindObjectsByType<PressureButton>(FindObjectsSortMode.None);
            foreach (var btn in allButtons)
            {
                // IDが一致、かつ keepStateAfterRespawn が true の場合
                if (btn.keepStateAfterRespawn && persistentButtonIDs.Contains(btn.buttonID))
                {
                    btn.SetPressedInstant();
                }
            }
        }

        // Kitanoスキルの使用回数復帰
        if (kitanoSkillUseCount >= 0)
        {
            TempDisableColliders kitanoSkill = FindAnyObjectByType<TempDisableColliders>();
            if (kitanoSkill != null)
            {
                kitanoSkill.CurrentUseCount = kitanoSkillUseCount;
            }
        }
    }

    void Update()
    {
        if (currentRespawnPoint == null && respawnPoints.Count > 0) return;

        // チェックポイント未設定時の安全策（Startで設定されるはずだが念のため）
        if (currentRespawnPoint == null && respawnPoints.Count > 0)
        {
             // 何もしない、あるいは0番目をセットするなど
        }
        else if (currentRespawnPoint != null)
        {
            // 現在のチェックポイント（currentRespawnPoint）とのY座標差分を計算
            float diffY = transform.position.y - currentRespawnPoint.position.y;

            // 上に行き過ぎた場合 OR 下に落ちすぎた場合
            if (diffY > maxHeightFromCheckpoint || this.transform.position.y < -fallDistanceFromCheckpoint)
            {
                Debug.Log($"制限エリア外に出ました (DiffY: {diffY:F2}) -> Respawn");
                Respawn();
            }
        }

        // デバッグ用：Rキーでリスポーン
        // playerStateがnullの場合のエラー回避
        if (playerState != null && (Input.GetKeyDown(KeyCode.P) || playerState.sanityLevel <= 0))
        {
            Respawn();
        }
    }

    public void Respawn()
    {
        // Nakamuraシーンの場合のみ、シーンリロードによる初期化を行う
        if (SceneManager.GetActiveScene().name == "Nakamura")
        {
            // 1. 維持したいPressureButtonの状態を保存
            persistentButtonIDs.Clear();
            PressureButton[] allButtons = FindObjectsByType<PressureButton>(FindObjectsSortMode.None);
            foreach (var btn in allButtons)
            {
                // 「リスポーン後も維持」フラグがあり、かつ現在押されている(pressCount > 0)場合
                if (btn.keepStateAfterRespawn && btn.pressCount > 0)
                {
                    if (!string.IsNullOrEmpty(btn.buttonID))
                    {
                        persistentButtonIDs.Add(btn.buttonID);
                    }
                    else
                    {
                        Debug.LogWarning($"⚠️ PressureButton '{btn.name}' は keepStateAfterRespawn=true ですが、buttonID が空です。状態は保存されません。");
                    }
                }
            }

            // Kitanoスキルの使用回数を保存
            TempDisableColliders kitanoSkill = FindAnyObjectByType<TempDisableColliders>();
            if (kitanoSkill != null)
            {
                kitanoSkillUseCount = kitanoSkill.CurrentUseCount;
            }

            // 2. 現在のチェックポイントIndexを保存
            lastCheckpointIndex = currentIndex;

            // 3. シーンリロード (すべて初期化される)
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        }
        else
        {
            //if(isAstralStage)    //  天体ステージだったら衝撃球を生成する(簡易実装の為後からリファクタ)
            //{ RespownImpactBall(); }

            // 他のシーンでは旧来のリスポーン処理（位置だけ戻す等）
            RespawnNormal();
        }
    }

    //void RespownImpactBall()    //  衝撃球再生成(簡易実装の為後からリファクタ)
    //{
    //    if (impactBallRespornDatas != null && isAstralStage)
    //    {
    //        foreach (var ball in impactBallRespornDatas)
    //        {
    //            if (ball == null || ball.ImpactBall == null)
    //            {
    //                Debug.Log("ImpactBallDatasにnullがある");
    //                continue;
    //            }

    //            Instantiate(ball.ImpactBall, ball.GenratePosition, Quaternion.identity);
    //        }
    //    }
    //}
    // 元々のリスポーン処理（Nakamura以外用）
    private void RespawnNormal()
    {
        if (currentRespawnPoint == null) return;

        // 重力リセット
        if (playerMove != null)
        {
            playerMove.ResetGravity();
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.MovePosition(currentRespawnPoint.position);
        }
        else
        {
            transform.position = currentRespawnPoint.position;
        }

        if (buddyStage)
        {
            // 相棒をおんぶ状態に戻す
            playerState.carryingBuddy = true;
            buddyCarry.buddyPos.constraintActive = true;
            buddyCarry.buddyController.moving = false;

            // 正気度と浸食度をリセット
            playerState.sanityLevel = 100;
            playerState.erosionLevel = 0;

            // リスポーンを通知
            OnPlayerRespawn?.Invoke();
        }

        // 全スイッチを強制リセット
        foreach (var sw in switchesToReset)
        {
            if (sw != null)
                sw.ForceReset();
        }

        // 白黒床の判定などをDark状態（リスポーン初期状態）にリセット
        var lightDarkWorld = FindAnyObjectByType<LightDarkWorld>();
        if (lightDarkWorld != null)
        {
            lightDarkWorld.ResetToDarkState();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Checkpointタグに触れたら該当Indexを探す
        for (int i = 0; i < respawnPoints.Count; i++)
        {
            if (respawnPoints[i] != null && other.transform == respawnPoints[i])
            {
                SetRespawnPoint(i);
                break;
            }
        }
    }

    public void SetRespawnPoint(int index)
    {
        if (index >= 0 && index < respawnPoints.Count)
        {
            if (respawnPoints[index] == null)
            {
                Debug.LogWarning($"⚠️ Index {index} のリスポーンポイントが設定されていません（nullです）。Inspectorを確認してください。");
                return;
            }

            currentIndex = index;
            currentRespawnPoint = respawnPoints[index];
            UpdateCheckpointVisual(index);
            //Debug.Log($"✅ リスポーン地点を更新しました → {respawnPoints[index].name}");
        }
    }

    void UpdateCheckpointVisual(int activeIndex)
    {
        for (int i = 0; i < checkpointVisuals.Count; i++)
        {
            if (checkpointVisuals[i] != null)
                checkpointVisuals[i].SetActiveState(i == activeIndex);
        }
    }

    public Transform GetCurrentRespawnPoint()
    {
        return currentRespawnPoint;
    }
}
