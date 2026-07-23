using UnityEngine;

public class Switch : MonoBehaviour
{
    [Header("下がる見た目（孫オブジェクト）")]
    public Transform visualObject;

    [Header("下がるコライダー（孫オブジェクト）")]
    public Transform pressDownCollider;

    [Header("スイッチテキスト")]
    public GameObject switchText;

    [Header("押下量")]
    public float pressDownDistance = 2.0f;

    [Header("押下後の挙動")]
    public bool destroyOnPressed = false; // ★ 追加

    private Vector3 visualInitialLocalPos;
    private Vector3 colliderInitialLocalPos;
    private bool isPressed = false;

    public bool IsPressed => isPressed;

    void Start()
    {
        // ★ 必ず localPosition（親基準）
        if (visualObject != null)
            visualInitialLocalPos = visualObject.localPosition;

        if (pressDownCollider != null)
            colliderInitialLocalPos = pressDownCollider.localPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isPressed) return;
        if (!other.CompareTag("Player")) return;

        // 上から踏んだ判定
        if (other.transform.position.y < transform.position.y) return;

        Press();
    }

    void Press()
    {
        Debug.Log("Pressed!");
        isPressed = true;

        // ▼ 孫オブジェクトだけを下げる
        if (visualObject != null)
            visualObject.localPosition =
                visualInitialLocalPos + Vector3.down * pressDownDistance;

        if (pressDownCollider != null)
            pressDownCollider.localPosition =
                colliderInitialLocalPos + Vector3.down * pressDownDistance;

        if (switchText != null)
            switchText.SetActive(false);

        // ★ 押したら削除する設定
        if (destroyOnPressed)
        {
            Destroy(gameObject);
        }
    }

    // PlayerRespawnUmeda から呼ばれる
    public void ForceReset()
    {
        isPressed = false;

        // ▲ 初期位置に完全復帰
        if (visualObject != null)
            visualObject.localPosition = visualInitialLocalPos;

        if (pressDownCollider != null)
            pressDownCollider.localPosition = colliderInitialLocalPos;

        if (switchText != null)
            switchText.SetActive(true);
    }
}
