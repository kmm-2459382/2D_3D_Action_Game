using UnityEngine;

public class GimmickSwitch : MonoBehaviour
{
    [SerializeField] private Switch SwitchSource; // 押下状態を監視するスイッチ

    [Header("作動させるギミック")]
    [SerializeField] private SwitchReceiver[] Receivers; // 作動対象のギミック

    private bool IsActivated = false; // 押されたかどうか

    private void Update()
    {
        // すでに作動済みなら何もしない
        if (IsActivated) return;

        // スイッチが存在し、押されたら作動
        if (SwitchSource != null && SwitchSource.IsPressed)
        {
            Activate();
        }
    }

    // スイッチが押された時の処理
    private void Activate()
    {
        // 再実行防止
        IsActivated = true;
        
        // 登録されたギミックを順番に作動
        foreach (var r in Receivers)
        {
            if (r != null)
            {
                r.Activate();
            }
        }

        // 以後の入力処理を止める（スクリプトを無効化）
        this.enabled = false;
    }
}
