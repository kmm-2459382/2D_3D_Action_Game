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

    [Header("Performance")]
    [Tooltip("チェックの実行間隔（秒）。0.05〜0.1くらいがおすすめ")]
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
                _controller.excludeLayers &= ~groundLayer;
            }
            else
            {
                _controller.excludeLayers |= groundLayer;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = _isInAppearArea ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position + sensorOffset, sensorRadius);
    }
}