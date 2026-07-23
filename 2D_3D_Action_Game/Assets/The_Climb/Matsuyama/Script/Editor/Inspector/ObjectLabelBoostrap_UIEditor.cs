using UnityEditor;

namespace TheClimb.Core
{
    [CustomEditor(typeof(ObjectLabelBootstrap))]
    public class ObjectLabelBoostrap_UIEditor : Editor
    {
        public override void OnInspectorGUI()    //  GUI編集
        {
            serializedObject.Update();

            EditorGUILayout.HelpBox("ラベルのScriptableObject", MessageType.Info);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_labelConfig"));

            EditorGUILayout.HelpBox("ラベルを表示する対象", MessageType.Info);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_labelTargetTF"));
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.HelpBox("ラベル対象のControlelr", MessageType.Info);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_itemController"));
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.HelpBox("ラベル本体", MessageType.Info);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_label"));
            serializedObject.ApplyModifiedProperties();
        }
    }
}