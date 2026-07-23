using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "SceneLibrary", menuName = "Stage/Scene Library")]
public class SceneLibrary : ScriptableObject
{
#if UNITY_EDITOR
    public List<SceneAsset> sceneAssets = new List<SceneAsset>();
#endif
}
