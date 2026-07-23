using ModestTree;
using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    private Vector3 ShakeOffset = Vector3.zero; // 現在の振動量
    private Coroutine ShakeCoroutine;           // 実行中の振動処理

    private void Awake()
    {
        // シングルトン登録
        Instance = this;
    }

    private void LateUpdate()
    {
        // PlayerCameraFollowerが位置を確定した後に、振動分だけ位置を加算する
        transform.position += ShakeOffset;
    }

    // 振動開始
    public void Shake(float Duration = 0.2f, float Magnitude = 0.3f)
    {
        if (ShakeCoroutine != null)
        {
            StopCoroutine(ShakeCoroutine);
        }

        // 振動処理を開始
        ShakeCoroutine = StartCoroutine(ShakeRoutine(Duration, Magnitude));
    }

    // 振動処理
    private IEnumerator ShakeRoutine(float Duration, float Magnitude)
    {
        // 経過時間
        float Elapsed = 0f;

        // 指定時間が経過するまで繰り返す
        while (Elapsed < Duration)
        {
            float X = Random.Range(-1f, 1f) * Magnitude;
            float Y = Random.Range(-1f, 1f) * Magnitude;

            ShakeOffset = new Vector3(X, Y, 0);

            Elapsed += Time.deltaTime;
            yield return null;
        }

        // 終了時にオフセットをリセット
        ShakeOffset = Vector3.zero;
    }
}