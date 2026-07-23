using System.Collections;
using UnityEngine;

public class SceneReadyManager : MonoBehaviour
{
    public static bool SceneReady = false;

    void Awake()
    {
        SceneReady = false;
    }

    void Start()
    {
        // 1フレーム後にシーン準備完了
        StartCoroutine(Ready());
    }

    IEnumerator Ready()
    {
        yield return null;
        SceneReady = true;
    }
}
