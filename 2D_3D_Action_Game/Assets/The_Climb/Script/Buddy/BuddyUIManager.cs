using UnityEngine;

public class BuddyUIManager : MonoBehaviour
{
    RectTransform arrowRectTransform = null;

    [SerializeField] private Transform target;
    [SerializeField] private Camera uiCamera;
    [SerializeField] private RectTransform canvasRect;
    [SerializeField] private RectTransform buddyRect;

    private float rateScale = 4.5f;
    private float scaleMin = 0.5f;
    private float scaleMax = 1.5f;

    [SerializeField] private Vector2 edgeBuffer = new Vector2(50f, 50f);

    void Start()
    {
        arrowRectTransform = GetComponent<RectTransform>();

        arrowRectTransform.localScale = Vector3.one;
    }

    void LateUpdate()
    {
        Vector3 screenPos = uiCamera.WorldToScreenPoint(target.position);

        // ターゲットがカメラの後ろにいる場合の処理 Z < 0
        if (screenPos.z < 0.0f)
        {
            screenPos *= -1.0f;
            // 画面中心から遠ざける方向へ押し出す
            Vector3 center = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
            Vector3 diff = (screenPos - center).normalized;
            if (diff.sqrMagnitude == 0) diff = Vector3.up; 
            screenPos = center + diff * Mathf.Max(Screen.width, Screen.height) * 2f; 
        }

        bool isOnScreen =
        screenPos.z > 0 &&
        screenPos.x >= 0 && screenPos.x <= Screen.width &&
        screenPos.y >= 0 && screenPos.y <= Screen.height;

        //画面内なら非表示
        if (isOnScreen)
        {
            buddyRect.gameObject.SetActive(false);
            return;
        }

        //画面外なら表示
        buddyRect.gameObject.SetActive(true);

        //画面端クランプ
        screenPos.x = Mathf.Clamp(screenPos.x, edgeBuffer.x, Screen.width - edgeBuffer.x);
        screenPos.y = Mathf.Clamp(screenPos.y, edgeBuffer.y, Screen.height - edgeBuffer.y);

        //位置変換
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            null, // 元通りnullに戻す（OverlayCanvas用）
            out localPos
        );

        buddyRect.localPosition = localPos;
    }
}
