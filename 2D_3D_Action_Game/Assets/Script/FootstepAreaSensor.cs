using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FootstepAreaSensor : MonoBehaviour
{
    [Header("Layer Settings")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask triggerAreaLayer;

    [Header("Sensor Settings")]
    [SerializeField] private float sensorRadius = 0.2f;
    [SerializeField] private Vector3 sensorOffset = new Vector3(0f, 0.05f, 0f);
    [SerializeField] private float checkInterval = 0.05f;

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
        Vector3 sensorWorldPosition = transform.position + sensorOffset;

        bool isTouchingTrigger = Physics.CheckSphere(
            sensorWorldPosition,
            sensorRadius,
            triggerAreaLayer,
            QueryTriggerInteraction.Collide
        );

        if (isTouchingTrigger != _isInAppearArea)
        {
            _isInAppearArea = isTouchingTrigger;

            if (_isInAppearArea)
            {
                // エリア内：通常通り地面に乗れる
                _controller.excludeLayers &= ~groundLayer;
            }
            else
            {
                // エリア外に出た瞬間：地面をすり抜けさせる
                _controller.excludeLayers |= groundLayer;

                // ★追加: エリア外に出た瞬間に横方向の引っかかりを消すため、
                // 少しだけ強制的に下方向へ押し出す（または慣性を断ち切る）
                _controller.Move(Vector3.down * 0.1f);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = _isInAppearArea ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position + sensorOffset, sensorRadius);
    }
}