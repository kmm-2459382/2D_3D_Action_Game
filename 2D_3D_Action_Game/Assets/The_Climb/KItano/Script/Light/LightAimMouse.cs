using UnityEngine;

public class LightAimToMouse : MonoBehaviour
{
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        // マウス位置をワールド座標に変換
        Vector3 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

        // Z座標をライトと合わせる（2D時は z を固定）
        mousePos.z = transform.position.z;

        // プレイヤー（ライト基点）→ マウス の方向ベクトル
        Vector3 direction = mousePos - transform.position;

        // 角度（2D用にZ軸回転のみ）
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // ライトをその方向に回転（Z軸のみ）
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
