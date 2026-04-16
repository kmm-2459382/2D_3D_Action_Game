using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputFilter : MonoBehaviour
{
    public InputActionAsset inputAsset;

    // 文字列の名前だと重複の恐れがあるため、内部的なIDで管理します
    public List<string> disabledBindingIds = new List<string>();

    private void Start()
    {
        ApplyFilter();
    }

    public void ApplyFilter()
    {
        if (inputAsset == null) return;

        foreach (var map in inputAsset.actionMaps)
        {
            foreach (var action in map.actions)
            {
                for (int i = 0; i < action.bindings.Count; i++)
                {
                    var binding = action.bindings[i];
                    if (disabledBindingIds.Contains(binding.id.ToString()))
                    {
                        // バインディングを実質的に無効化（空のパスで上書き）
                        action.ApplyBindingOverride(i, new InputBinding { overridePath = "" });
                    }
                    else
                    {
                        // 上書きを解除して元に戻す
                        action.RemoveBindingOverride(i);
                    }
                }
            }
        }
    }
}