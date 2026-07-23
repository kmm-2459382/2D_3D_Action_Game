using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class RandomSceneAssigner : MonoBehaviour
{
    private const string PlayerPrefsKey = "RandomSceneAssigned"; // 一度だけ実行用

    [Header("シーンリスト（ScriptableObject）")]
    public SceneLibrary sceneLibrary;

    [Header("割り当て対象の StageNode")]
    public List<StageNode> stageNodes = new List<StageNode>();

    [Header("ランダム種（固定すると毎回同じ順）")]
    public int randomSeed = 0;
    public bool useFixedSeed = false;

    private void Start()
    {
        // SceneReadyManager が true になるまで待つ
        StartCoroutine(WaitAndAssign());
    }

    private System.Collections.IEnumerator WaitAndAssign()
    {
        // SceneReadyManager の準備待ち
        while (!SceneReadyManager.SceneReady)
            yield return null;

        // 一度だけ実行
        if (PlayerPrefs.GetInt(PlayerPrefsKey, 0) == 1)
            yield break;

#if UNITY_EDITOR
        AssignRandomScenes();
#endif

        PlayerPrefs.SetInt(PlayerPrefsKey, 1);
        PlayerPrefs.Save();
    }

#if UNITY_EDITOR
    [ContextMenu("Assign Random Scenes")]
    public void AssignRandomScenes()
    {
        // ScriptableObject にシーンが存在するかチェック
        if (sceneLibrary == null || sceneLibrary.sceneAssets == null || sceneLibrary.sceneAssets.Count == 0)
        {
            Debug.LogError("SceneLibrary が空です！");
            return;
        }

        // StageNode が空なら自動取得
        if (stageNodes.Count == 0)
            stageNodes.AddRange(FindObjectsOfType<StageNode>(true));

        List<SceneAsset> pool = new List<SceneAsset>(sceneLibrary.sceneAssets);

        // ランダム初期化
        if (useFixedSeed)
            Random.InitState(randomSeed);

        // シャッフル
        for (int i = 0; i < pool.Count; i++)
        {
            int rand = Random.Range(i, pool.Count);
            (pool[i], pool[rand]) = (pool[rand], pool[i]);
        }

        // 割り当て
        for (int i = 0; i < stageNodes.Count; i++)
        {
            if (i < pool.Count)
            {
                StageNode node = stageNodes[i];
                node.sceneAsset = pool[i];
                node.RefreshSceneName();
                EditorUtility.SetDirty(node);
            }
        }

        Debug.Log("ランダムでシーンを割り当てました！");
    }
#endif
}
