using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class GravityFlipManager : MonoBehaviour
{
    [Header("操作設定")]
    [SerializeField] private InputActionReference flipAction; // ← Input Systemの "Wキー" アクションを指定

    [Header("対象プレイヤー")]
    [SerializeField] private PlayerMove player; // ← PlayerMoveを持つプレイヤーを指定

    [Header("クールタイム設定")]
    [SerializeField] private float flipCooldown = 10f; // クールタイム秒数（仮）
    private bool canFlip = true; // クールタイム中でなければtrue

    [Header("UI（任意）")]
    [SerializeField] private UnityEngine.UI.Image cooldownFillImage; // クールタイム表示UI（任意）

    private void OnEnable()
    {
        if (flipAction != null)
        {
            flipAction.action.performed += OnFlipPressed;
            flipAction.action.Enable();
        }
    }

    private void Update()
    {
        if (Keyboard.current == null)
        {
            Debug.Log("Keyboard.current が NULL！ → Input System が動いていません");
            return;
        }

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            Debug.Log("Eキー押された！");
        }
    }



    private void OnDisable()
    {
        if (flipAction != null)
        {
            flipAction.action.performed -= OnFlipPressed;
            flipAction.action.Disable();
        }
    }

    private void OnFlipPressed(InputAction.CallbackContext context)
    {
        if (PlayerPrefs.GetInt("NisiyamaAbi") == 1)
        {
            // プレイヤー未指定・クールタイム中は無視
            if (player == null || !canFlip)
                return;

            // 空中では反転不可
            if (!player.State.isGrounded)
            {
                Debug.Log("反転できません（空中）");
                return;
            }

            // 反転実行
            player.ToggleUpsideDown();

            // クールタイム開始(反転直後から開始)
            StartCoroutine(FlipCooldownCoroutine());
        }
    }

    // メンバ変数としてタイマーを保持
    private float currentCooldownTimer = 0f;

    private IEnumerator FlipCooldownCoroutine()
    {
        canFlip = false;
        currentCooldownTimer = 0f;

        // クールタイム中はゲージを更新
        while (currentCooldownTimer < flipCooldown)
        {
            currentCooldownTimer += Time.deltaTime;

            // UI表示がある場合(互換性のため残す)
            if (cooldownFillImage != null)
            {
                cooldownFillImage.fillAmount = 1f - (currentCooldownTimer / flipCooldown);
            }

            yield return null;
        }

        canFlip = true;
        currentCooldownTimer = 0f;

        if (cooldownFillImage != null)
        {
            cooldownFillImage.fillAmount = 0f;
        }

        Debug.Log("反転クールタイム終了");
    }

    /// <summary>
    /// クールタイム進行状況を取得(0.0~1.0、1.0が満タン)
    /// </summary>
    public float GetCooldownProgress()
    {
        if (canFlip)
        {
            return 0f; // クールタイム終了、ゲージなし
        }

        // タイマーから計算
        float progress = 1f - (currentCooldownTimer / flipCooldown);
        return Mathf.Clamp01(progress);
    }

}
