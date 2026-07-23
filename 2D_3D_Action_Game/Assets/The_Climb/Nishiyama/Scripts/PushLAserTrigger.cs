using UnityEngine;

public class PushLaserTrigger : MonoBehaviour
{
    [Header("起動する迫ってくるレーザー（複数可）")]
    public LaserKill[] lasers;

    private bool used = false; // 一度使ったかどうか



    private void OnTriggerEnter(Collider other)
    {
        if (used) return;

        if (other.CompareTag("Player"))
        {
            used = true;
            Debug.Log("【PushLaserTrigger】{gameObject.name} を通過");

            if (lasers == null || lasers.Length == 0)
            {
                Debug.LogWarning("【PushLaserTrigger】Laser が設定されていません！");
                return;
            }

            foreach (var laser in lasers)
            {
                if (laser != null)
                {
                    laser.AppearAndStartPush();
                }
                else
                {
                    Debug.LogWarning("【PushLaserTrigger】Laser に null が含まれています");
                }
            }
        }
    }

    public void ResetTrigger()
    {
        used = false;
    }
}
