using UnityEngine;

public class RespawnOnTouch : MonoBehaviour
{
    PlayerBarrier playerBarrier;

    void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerBarrier = player.GetComponentInChildren<PlayerBarrier>();
        }
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.collider.CompareTag("Player"))
    //    {
    //        PlayerRespawnUmeda respawn = collision.collider.GetComponent<PlayerRespawnUmeda>();

    //        if (playerBarrier == null)
    //        {
    //            respawn.Respawn();
    //            return;
    //        }
    //        else if (!playerBarrier.TryBlockAttack())
    //        {
    //            if (respawn != null)
    //            {
    //                respawn.Respawn();
    //            }
    //        }
    //    }
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerRespawnUmeda respawn = other.GetComponent<PlayerRespawnUmeda>();
            if (playerBarrier == null)
            {
                respawn.Respawn();
                return;
            }
            else if (!playerBarrier.TryBlockAttack())
            {
                if (respawn != null)
                {
                    respawn.Respawn();
                }
            }
        }
    }
}
