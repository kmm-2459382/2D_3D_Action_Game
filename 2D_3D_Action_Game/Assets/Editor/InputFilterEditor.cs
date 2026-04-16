using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

[CustomEditor(typeof(InputFilter))]
public class InputFilterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        InputFilter script = (InputFilter)target;

        script.inputAsset = (InputActionAsset)EditorGUILayout.ObjectField(
            "Input Action Asset", script.inputAsset, typeof(InputActionAsset), false);

        if (script.inputAsset == null) return;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("入力を個別に無効化 (チェックでOFF):", EditorStyles.boldLabel);

        foreach (var map in script.inputAsset.actionMaps)
        {
            // --- Action Map レイヤー ---
            EditorGUILayout.LabelField($"Map: {map.name}", EditorStyles.boldLabel);

            foreach (var action in map.actions)
            {
                EditorGUI.indentLevel++;
                // --- Action レイヤー ---
                EditorGUILayout.LabelField($"Action: {action.name}", EditorStyles.miniBoldLabel);

                EditorGUI.indentLevel++;
                // --- Binding レイヤー (W, A, S, D, Spaceなど) ---
                for (int i = 0; i < action.bindings.Count; i++)
                {
                    var binding = action.bindings[i];

                    // 合成バインディング（2D Vectorなど）の親項目は飛ばす
                    if (binding.isComposite)
                    {
                        EditorGUILayout.LabelField($"> {binding.name}");
                        continue;
                    }

                    string label = string.IsNullOrEmpty(binding.name) ? binding.path : binding.name;
                    string bindingId = binding.id.ToString();

                    bool isDisabled = script.disabledBindingIds.Contains(bindingId);

                    // チェックボックスを表示
                    bool newToggle = EditorGUILayout.Toggle($"{label} ({binding.path})", isDisabled);

                    if (newToggle != isDisabled)
                    {
                        Undo.RecordObject(script, "Toggle Binding Filter");
                        if (newToggle) script.disabledBindingIds.Add(bindingId);
                        else script.disabledBindingIds.Remove(bindingId);
                        EditorUtility.SetDirty(script);
                    }
                }
                EditorGUI.indentLevel -= 2;
            }
        }

        if (GUI.changed && Application.isPlaying) script.ApplyFilter();
    }
}