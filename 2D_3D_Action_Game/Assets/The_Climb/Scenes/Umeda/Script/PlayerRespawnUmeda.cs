using UnityEngine;
using System;
using System.Collections.Generic;

public class PlayerRespawnUmeda : MonoBehaviour
{
    private CharacterController characterController;

    [Header("リスポーンポイントリスト")]
    public List<Transform> respawnPoints = new List<Transform>(); // 手動設定用リスト

    [Header("現在のリスポーンインデックス")]
    [SerializeField] private int currentIndex = 0;

    private Transform currentRespawnPoint;

    // チェックポイントの見た目制御用（同じ順番で登録）
    [Header("対応するチェックポイント見た目リスト")]
    public List<CheckpointVisual> checkpointVisuals = new List<CheckpointVisual>();

    [Header("リスポーン判定設定（チェックポイントからの高さ制限）")]
    public float maxHeightFromCheckpoint = 30f;   // チェックポイントよりこれ以上「上」に行ったらリスポーン
    public float maxFallFromCheckpoint = 20f;     // チェックポイントよりこれ以上「下」に落ちたらリスポーン

    public static Action OnPlayerRespawn;

    [Header("リスポーン時にリセットするスイッチ")]
    public List<Switch> switchesToReset = new List<Switch>();

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        // 初回起動時にリスポーンポイントが設定されていれば0番目をセット
        if (respawnPoints.Count > 0)
        {
            SetRespawnPoint(0);
        }
        else
        {
            Debug.LogWarning("⚠️ リスポーンポイントが未設定です。");
        }
    }

    void Update()
    {
        if (currentRespawnPoint == null && respawnPoints.Count > 0) return;

        if (currentRespawnPoint != null)
        {
            // 🔸 チェックポイントとプレイヤーの「Y座標の差分」を計算
            // プレイヤーが上ならプラス、下ならマイナスになる
            float diffY = transform.position.y - currentRespawnPoint.position.y;

            // 🔸 上方向の制限チェック（例: 30mより上に行ったら）
            bool isTooHigh = diffY > maxHeightFromCheckpoint;

            // 🔸 下方向の制限チェック（例: -20mより下に落ちたら）
            bool isTooLow = diffY < -maxFallFromCheckpoint;

            // どちらかに引っかかったらリスポーン
            if (isTooHigh || isTooLow)
            {
                Debug.Log($"制限エリア外に出ました (DiffY: {diffY:F2}) -> Respawn");
                Respawn();
            }
        }
    }

    public void Respawn()
    {
        if (currentRespawnPoint == null)
        {
            Debug.LogWarning("⚠️ 現在有効なリスポーンポイントがないため、リスポーンできません。");
            return;
        }

        // CharacterControllerを使用している場合の安全なワープ処理
        if (characterController != null)
        {
            characterController.enabled = false; // 一度無効化して位置を強制転送
            transform.position = currentRespawnPoint.position;
            transform.rotation = currentRespawnPoint.rotation; // 向きもリセット

            // 落下時に床の除外レイヤーが設定されたままの場合はクリア
            characterController.excludeLayers = 0;

            characterController.enabled = true;  // 再有効化
        }
        else
        {
            transform.position = currentRespawnPoint.position;
            transform.rotation = currentRespawnPoint.rotation;
        }

        // MarioStyleController の速度・慣性をリセット
        var marioController = GetComponent<StarterAssets.MarioStyleController>();
        if (marioController != null)
        {
            marioController.ResetVelocity();
        }

        // 外部コンポーネント（存在する場合のみ安全にリセット処理を呼び出し）
        var playerMove = GetComponent<PlayerMove>();
        if (playerMove != null)
        {
            playerMove.ResetGravity();
        }

        // 相棒システムなど（存在すれば安全に処理）
        var playerState = GameObject.Find("PlayerModel")?.GetComponent<PlayerState>();
        if (playerState != null)
        {
            playerState.carryingBuddy = true;
            playerState.sanityLevel = 100;
            playerState.erosionLevel = 0;
        }

        var buddyCarry = GameObject.Find("PlayerModel")?.GetComponent<BuddyCarry>();
        if (buddyCarry != null && buddyCarry.buddyPos != null)
        {
            buddyCarry.buddyPos.constraintActive = true;
            if (buddyCarry.buddyController != null)
            {
                buddyCarry.buddyController.moving = false;
            }
        }

        // リスポーン通知イベントを発火
        OnPlayerRespawn?.Invoke();

        // 登録されたスイッチの強制リセット
        foreach (var sw in switchesToReset)
        {
            if (sw != null)
                sw.ForceReset();
        }

        // LightDarkWorld（存在する場合のみ）
        var lightDarkWorld = FindAnyObjectByType<LightDarkWorld>();
        if (lightDarkWorld != null)
        {
            lightDarkWorld.ResetToDarkState();
        }

        Debug.Log("🔄 プレイヤーをリスポーン地点に戻しました。");
    }

    private void OnTriggerEnter(Collider other)
    {
        // Checkpointに触れたら該当Indexを探す
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
                Debug.LogWarning($"⚠️ Index {index} のリスポーンポイントが設定されていません（nullです）。");
                return;
            }

            currentIndex = index;
            currentRespawnPoint = respawnPoints[index];
            UpdateCheckpointVisual(index);
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