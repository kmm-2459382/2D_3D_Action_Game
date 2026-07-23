using UnityEngine;

public class Bomb_Aim : MonoBehaviour
{
    [SerializeField] private Camera aimCamera;
    [SerializeField] private Transform player;
    [SerializeField] private Transform p_leftPos;
    [SerializeField] private Transform p_rightPos;
    [SerializeField] private Transform p_DownPos;
    [SerializeField] private Transform p_UpPos;
    [SerializeField] private float last_Direction = 0;
    [SerializeField] PlayerState playerState;
    public Transform GetSpawnPos()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = aimCamera.WorldToScreenPoint(player.position).z;
        Vector3 worldMousePos = aimCamera.ScreenToWorldPoint(mousePos);

        Vector3 diff = worldMousePos - player.position;

        if (!playerState.isGrounded && diff.y < 0)
            return p_DownPos;

        // 左右判定
        if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
            return (diff.x < 0 ? p_leftPos : p_rightPos);
        // 上方向だけ判定
        if (diff.y > 0)
            return p_UpPos;

        return p_UpPos;
    }


        public Vector3 GetShootDirection()
    {
        Transform pos = GetSpawnPos();

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = aimCamera.WorldToScreenPoint(pos.position).z;
        Vector3 worldPos = aimCamera.ScreenToWorldPoint(mousePos);
        Vector3 dir = (worldPos - player.position).normalized;

        pos.rotation = Quaternion.LookRotation(dir);

        return dir;
    }
}