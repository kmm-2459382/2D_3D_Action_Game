using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FlexiblePathWalker : MonoBehaviour
{
    [Header("Layer Settings")]
    [Tooltip("ギミック用の床・壁のレイヤー")]
    [SerializeField] private LayerMask phantomLayer;

    [Tooltip("歩ける・触れる形状を表すトリガー（球体など）のレイヤー")]
    [SerializeField] private LayerMask triggerLayer;

    [Header("Detection Settings")]
    [Tooltip("足元の床を検知する距離（基本は キャラ身長の半分＋α）")]
    [SerializeField] private float groundCheckDistance = 0.6f;

    [Tooltip("前方の壁を検知する距離（少し余裕を持たせるとスムーズです）")]
    [SerializeField] private float wallCheckDistance = 0.4f;

    [Tooltip("その地点にトリガーがあるかを判定する球の半径")]
    [SerializeField] private float triggerCheckRadius = 0.2f;

    private CharacterController characterController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void FixedUpdate()
    {
        Vector3 moveDirection = Vector3.zero;
        if (UnityEngine.InputSystem.Keyboard.current != null)
        {
            var keyboard = UnityEngine.InputSystem.Keyboard.current;
            float x = 0;
            float z = 0;

            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) z = 1;
            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) z = -1;
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) x = -1;
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) x = 1;

            moveDirection = new Vector3(x, 0, z).normalized;
        }

        // 1. 足元（地面）のチェック
        bool onValidGround = CheckTerrain(transform.position, Vector3.down, groundCheckDistance, isWallCheck: false);

        // 2. 前方（壁）のチェック
        bool onValidWall = false;
        if (moveDirection.magnitude > 0.05f)
        {
            onValidWall = CheckTerrain(transform.position, moveDirection, wallCheckDistance, isWallCheck: true);
        }

        // 3. 【核心部】もしギミック床に触れているのに、そこにトリガーがない（False）場合、
        // CharacterControllerの「めり込み」を自ら発生させて、物理的に下へすり抜けさせます。
        if (!onValidGround && Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, groundCheckDistance, phantomLayer))
        {
            // トリガーがないギミック床の上にいる間は、強制的に下方向へキャラクターをわずかに押し下げ、
            // 床のコライダーを強制貫通（すり抜け）させます。
            characterController.Move(Vector3.down * 0.1f);
        }
    }

    /// <summary>
    /// 地面や壁を検知し、その場所にトリガーがあるかを調べる処理
    /// 戻り値: トリガー内にいて「乗ってヨシ」なら true、すり抜けるべきなら false
    /// </summary>
    private bool CheckTerrain(Vector3 origin, Vector3 direction, float distance, bool isWallCheck)
    {
        RaycastHit hit;
        bool hasHit = false;

        if (isWallCheck)
            hasHit = Physics.SphereCast(origin, characterController.radius * 0.9f, direction, out hit, distance, phantomLayer);
        else
            hasHit = Physics.Raycast(origin, direction, out hit, distance, phantomLayer);

        if (hasHit)
        {
            // 当たった場所にトリガー（球体）があるか調べる
            bool isInsideTrigger = Physics.CheckSphere(hit.point, triggerCheckRadius, triggerLayer);

            Debug.Log($"【検知】{hit.collider.name} に当たりました。トリガーの有無: {isInsideTrigger}");

            // トリガーがあれば（True）足場として認めたいので true を返す
            return isInsideTrigger;
        }

        // そもそもギミック床がない場合は、通常の床にいるとみなして制限しない（trueを返す）
        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
        Gizmos.DrawWireSphere(transform.position + Vector3.down * groundCheckDistance, triggerCheckRadius);
    }
}