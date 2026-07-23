using UnityEngine;
using System.Collections;

public class SkillPlatformSpawner : MonoBehaviour
{
    [Header("参照設定")]
    public Transform player;               // ← プレイヤーをInspectorで指定
    public GameObject platformPrefab;      // 足場のPrefab
    public GameObject triggerColliderPrefab; // コライダーPrefab（IsTrigger = true）

    [Header("スキル設定")]
    public KeyCode activateKey = KeyCode.E;
    public float skillCooldown = 3f;       // 使用間隔（秒）
    public float colliderShrinkDuration = 1.5f; // コライダーが消えるまでの時間

    [Header("生成位置設定")]
    public Vector3 spawnOffset = new Vector3(0, -0.49f, 0); // プレイヤーの足元に生成
    public float zShiftOnEnd = -2f;        // 消滅時にずらす距離（Z軸方向）

    private bool canUseSkill = true;
    private float cooldownTimer = 0f; // クールタイムの残り時間を追跡
    private PlayerMove playerMove; // プレイヤーの移動制御クラス

    void Start()
    {
        if (player != null)
        {
            playerMove = player.GetComponent<PlayerMove>(); // プレイヤーの移動制御クラスを取得
        }
    }

    /// <summary>
    /// クールタイム進行状況を取得(0.0~1.0、1.0が満タン)
    /// </summary>
    public float GetCooldownProgress()
    {
        if (canUseSkill)
        {
            return 0f; // クールタイム終了、ゲージなし
        }

        // クールタイム中の進行状況を計算(残り時間割合)
        float progress = cooldownTimer / skillCooldown;
        return Mathf.Clamp01(progress);
    }

    void Update()
    {
        if (player == null) return;

        if (Input.GetKeyDown(activateKey) && canUseSkill && PlayerPrefs.GetInt("UmedaAbi") == 1)
        {
            StartCoroutine(SpawnPlatform()); // 足場を生成する
        }
    }

    private IEnumerator SpawnPlatform()
    {
        canUseSkill = false;

        // プレイヤーの足元の座標を取得
        Vector3 currentOffset = spawnOffset;
        if (playerMove != null && playerMove.IsUpsideDown)
        {
            currentOffset.y *= -1; // 上下反転している場合はオフセットを反転
        }
        Vector3 spawnPos = player.position + currentOffset;

        // 足場とトリガーを生成
        GameObject platform = Instantiate(platformPrefab, spawnPos, Quaternion.identity); // 足場を生成
        GameObject trigger = Instantiate(triggerColliderPrefab, spawnPos, Quaternion.identity); // トリガーを生成

        // トリガーの縮小アニメーション
        float timer = 0f;
        Vector3 initialScale = trigger.transform.localScale;

        while (timer < colliderShrinkDuration)
        {
            timer += Time.deltaTime;
            float t = timer / colliderShrinkDuration;
            trigger.transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, t);
            yield return null;
        }

        // コライダーが完全に縮んだ瞬間
        // → 足場とトリガーをZ軸方向にずらす
        platform.transform.position += new Vector3(0, 0, zShiftOnEnd); // 足場をずらす
        trigger.transform.position += new Vector3(0, 0, zShiftOnEnd); // トリガーをずらす

        // 少し待ってから破壊（0.1秒で自然な消え方に）
        yield return new WaitForSeconds(0.1f);

        platform.SetActive(false); // 足場を非表示にする
        trigger.SetActive(false); // トリガーを非表示にする

        // クールタイム(タイマーを更新しながら待機)
        cooldownTimer = skillCooldown;
        while (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
            yield return null;
        }
        cooldownTimer = 0f;
        canUseSkill = true;
    }
}
