using UnityEngine;

public class CheckpointVisual : MonoBehaviour
{
    [Header("見た目切り替え用")]
    public GameObject inactiveCylinder; // 非アクティブ時（例：青）
    public GameObject activeCylinder;   // アクティブ時（例：赤や黄）

    private void Start()
    {
        SetActiveState(false);
    }

    public void SetActiveState(bool isActive)
    {
        if (inactiveCylinder != null)
            inactiveCylinder.SetActive(!isActive);

        if (activeCylinder != null)
            activeCylinder.SetActive(isActive);
    }
}