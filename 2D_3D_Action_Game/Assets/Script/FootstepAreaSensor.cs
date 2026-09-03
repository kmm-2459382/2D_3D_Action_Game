using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FootstepAreaSensor : MonoBehaviour
{
    [Header("Layer Settings")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask triggerAreaLayer;

    [Header("Sensor Settings")]
    [SerializeField] private float sensorRadius = 0.15f;
    [SerializeField] private float checkInterval = 0.05f;

    [Tooltip("複数の足元センサー位置（例: 前後左右など）")]
    [SerializeField]
    private Vector3[] sensorOffsets = new Vector3[]
    {
        new Vector3(0.2f, 0.05f, 0.2f),   // 右前
        new Vector3(-0.2f, 0.05f, 0.2f),  // 左前
        new Vector3(0.2f, 0.05f, -0.2f),  // 右後ろ
        new Vector3(-0.2f, 0.05f, -0.2f)  // 左後ろ
    };

    [Tooltip("チェックを入れると『すべてのセンサーがエリア内』でないと落ちます。チェックを外すと『1つでも触れていれば安全』になります。")]
    [SerializeField] private bool requireAllSensors = false;

    private CharacterController _controller;
    private bool _isInAppearArea = false;
    private float _timer = 0f;

    void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= checkInterval)
        {
            _timer = 0f;
            CheckFootArea();
        }
    }

    private void CheckFootArea()
    {
        if (sensorOffsets == null || sensorOffsets.Length == 0) return;

        bool isTouchingTrigger = requireAllSensors ? true : false;

        for (int i = 0; i < sensorOffsets.Length; i++)
        {
            // キャラクターの向き（回転）に合わせてオフセットを変換
            Vector3 sensorWorldPosition = transform.TransformPoint(sensorOffsets[i]);

            bool hit = Physics.CheckSphere(
                sensorWorldPosition,
                sensorRadius,
                triggerAreaLayer,
                QueryTriggerInteraction.Collide
            );

            if (requireAllSensors)
            {
                // 1つでも外れたらNG
                if (!hit)
                {
                    isTouchingTrigger = false;
                    break;
                }
            }
            else
            {
                // 1つでも触れていればOK
                if (hit)
                {
                    isTouchingTrigger = true;
                    break;
                }
            }
        }

        if (isTouchingTrigger != _isInAppearArea)
        {
            _isInAppearArea = isTouchingTrigger;

            if (_isInAppearArea)
            {
                // エリア内：地面に乗れる
                _controller.excludeLayers &= ~groundLayer;
            }
            else
            {
                // エリア外：地面をすり抜けて落下
                _controller.excludeLayers |= groundLayer;
                _controller.Move(Vector3.down * 0.1f);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (sensorOffsets == null) return;

        Gizmos.color = _isInAppearArea ? Color.green : Color.red;

        foreach (var offset in sensorOffsets)
        {
            Vector3 worldPos = transform.TransformPoint(offset);
            Gizmos.DrawWireSphere(worldPos, sensorRadius);
        }
    }
}