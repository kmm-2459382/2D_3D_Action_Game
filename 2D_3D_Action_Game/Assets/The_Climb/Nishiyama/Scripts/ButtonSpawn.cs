using UnityEngine;
using System.Collections;

public class ButtonGimmick : MonoBehaviour
{
    [Header("押せるボタンのTransform")]
    [SerializeField] private Transform buttonTransform;
    [SerializeField] private float pressDepth = 0.1f;
    [SerializeField] private float pressSpeed = 5f;

    [Header("出現させたいオブジェクト（複数対応）")]
    [SerializeField] private GameObject[] targetObjects;

    [Header("ボタンが沈んで戻るまで & オブジェクトの出現時間")]
    [SerializeField] private float activeTime = 3f;

    private bool isActivated = false;
    private bool isPressAnimating = false;
    private Vector3 originalLocalPos;

    private void Start()
    {
        if (buttonTransform != null)
            originalLocalPos = buttonTransform.localPosition;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (!isActivated)
            StartCoroutine(ActivateGimmick());
    }

    private IEnumerator ActivateGimmick()
    {
        isActivated = true;

        // 同時に処理開始
        StartCoroutine(PressButtonAnimation());

        // オブジェクト出現
        foreach (var obj in targetObjects)
            if (obj != null) obj.SetActive(true);

        // activeTime 秒待つ（ボタンが沈んで戻る時間と共通）
        yield return new WaitForSeconds(activeTime);

        // 消える
        foreach (var obj in targetObjects)
            if (obj != null) obj.SetActive(false);

        isActivated = false;
    }

    public void ForceResetButton()
    {
        StopAllCoroutines(); // アニメーション中断

        if (buttonTransform != null)
            buttonTransform.localPosition = originalLocalPos;

        isPressAnimating = false;
        isActivated = false;

        // すべてのターゲット非表示
        foreach (var obj in targetObjects)
            if (obj != null) obj.SetActive(false);
    }

    private IEnumerator PressButtonAnimation()
    {
        if (isPressAnimating || buttonTransform == null) yield break;
        isPressAnimating = true;

        Vector3 originalPos = buttonTransform.localPosition;
        Vector3 pressedPos = originalPos - new Vector3(0, pressDepth, 0);

        // 押し込む
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * pressSpeed;
            buttonTransform.localPosition = Vector3.Lerp(originalPos, pressedPos, t);
            yield return null;
        }

        // activeTime 秒押されたままにする
        yield return new WaitForSeconds(activeTime);

        // 元に戻す
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * pressSpeed;
            buttonTransform.localPosition = Vector3.Lerp(pressedPos, originalPos, t);
            yield return null;
        }



        isPressAnimating = false;
    }
}
