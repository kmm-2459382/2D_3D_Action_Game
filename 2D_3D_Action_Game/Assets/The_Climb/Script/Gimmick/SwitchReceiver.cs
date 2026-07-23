using UnityEngine;

public class SwitchReceiver : MonoBehaviour
{
    [Header("スイッチ未作動時に表示するオブジェクト")]
    [SerializeField] private GameObject InactiveObject;

    [Header("スイッチ作動後に表示するオブジェクト")]
    [SerializeField] private GameObject ActiveObject;

    void Awake()
    {
        // スイッチ作動後用のオブジェクトを非表示にしておく
        if (ActiveObject != null)
        {
            ActiveObject.SetActive(false);
        }
    }

    // スイッチが押されたときに呼ばれる
    // 表示を「未作動→作動」に切り替える
    public void Activate()
    {
        // 未作動用のオブジェクトを非表示
        if (InactiveObject != null)
        {
            InactiveObject.SetActive(false);
        }

        // 作動後用のオブジェクトを表示
        if (ActiveObject != null)
        {
            ActiveObject.SetActive(true);
        }
    }
}
