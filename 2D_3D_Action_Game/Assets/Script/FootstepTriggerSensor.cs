using UnityEngine;

public class FootstepTriggerSensor : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask triggerAreaLayer;

    private CharacterController _parentController;
    private int _triggerEnterCount = 0; // 重なるエリアのカウント

    void Start()
    {
        // 親オブジェクトにある CharacterController を取得
        _parentController = GetComponentInParent<CharacterController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // 触れたオブジェクトが LuminaAppearerTrigger レイヤーの場合
        if (((1 << other.gameObject.layer) & triggerAreaLayer) != 0)
        {
            _triggerEnterCount++;
            UpdateGroundCollision();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & triggerAreaLayer) != 0)
        {
            _triggerEnterCount = Mathf.Max(0, _triggerEnterCount - 1);
            UpdateGroundCollision();
        }
    }

    private void UpdateGroundCollision()
    {
        if (_parentController == null) return;

        if (_triggerEnterCount > 0)
        {
            // 1つでもエリアに入っていれば地面と衝突させる
            _parentController.excludeLayers &= ~groundLayer;
        }
        else
        {
            // 全てのエリアから出たら地面をすり抜けさせる
            _parentController.excludeLayers |= groundLayer;
        }
    }
}