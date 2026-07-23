using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Kitanoスキル用UI
/// ・使用回数を弾アイコン（3つ）で表示
/// ・使い切ったらグレー半透明オーバーレイを表示
/// </summary>
public class KitanoAmmoUI : MonoBehaviour
{
    [Header("弾アイコン（左から順）")]
    [SerializeField] private Image[] ammoIcons;

    [Header("グレー半透明オーバーレイ")]
    [SerializeField] private Image grayOverlay;

    [Header("Kitanoスキル参照")]
    [SerializeField] private TempDisableColliders kitanoSkill;

    [Header("スキル管理（任意）")]
    [SerializeField] private PlayerAbilityManager abilityManager;

    private int lastRemainingCount = -1;
    private bool lastDepletedState = false;

    void Start()
    {
        if (grayOverlay != null)
            grayOverlay.gameObject.SetActive(false);
    }

    void Update()
    {
        if (kitanoSkill == null) return;

        // Kitanoスキル以外ならUI非表示（必要なければ消してOK）
        if (abilityManager != null && abilityManager.CurrentSkillNumber != 2)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

        int remaining = kitanoSkill.GetRemainingUseCount();
        bool isDepleted = remaining <= 0;

        // 弾数が変わったときだけ更新
        if (remaining != lastRemainingCount)
        {
            UpdateAmmoIcons(remaining);
            lastRemainingCount = remaining;
        }

        // 使い切り状態が変わったときだけ更新
        if (isDepleted != lastDepletedState)
        {
            SetGrayOverlay(isDepleted);
            lastDepletedState = isDepleted;
        }
    }

    /// <summary>
    /// 弾アイコン表示更新
    /// </summary>
    private void UpdateAmmoIcons(int remaining)
    {
        for (int i = 0; i < ammoIcons.Length; i++)
        {
            if (ammoIcons[i] == null) continue;

            // 残り回数分だけ表示
            ammoIcons[i].enabled = i < remaining;
        }
    }

    /// <summary>
    /// グレーオーバーレイ切り替え
    /// </summary>
    private void SetGrayOverlay(bool active)
    {
        if (grayOverlay != null)
        {
            grayOverlay.gameObject.SetActive(active);
        }
    }
}
