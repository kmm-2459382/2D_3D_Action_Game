using UnityEngine;

public class StickRotate : MonoBehaviour
{
    void Update()
    {
        
            Vector3 mousePos = Input.mousePosition;

            // 自分のz位置を基準に、カメラからの距離を指定
            mousePos.z = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);

            Vector3 worldMouse = Camera.main.ScreenToWorldPoint(mousePos);
            worldMouse.z = 0;

            Vector3 direction = worldMouse - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(0, 0, angle);
        
    }
    
}
