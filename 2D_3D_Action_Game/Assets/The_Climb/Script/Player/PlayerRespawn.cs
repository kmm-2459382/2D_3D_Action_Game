using UnityEngine;
using TheClimb.Astral;
using System.Collections.Generic;

public class PlayerRespawn : MonoBehaviour
{
    Rigidbody rb;

    private Vector3 lastSavePos; //リスポーン位置

    [SerializeField] List<ImpactBallRespornData> impactBallRespornDatas;    //  衝撃球のリセットに必要なデータ(簡易実装の為後からリファクタ)

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        lastSavePos = gameObject.transform.position;
    }

    void Update()
    {
        //落ちた判定(今は簡易)
        if (transform.position.y < -4.3f)
        {
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;  // 速度リセット
                rb.MovePosition(lastSavePos);      // 物理的にワープ
            }
            else
            {
                transform.position = lastSavePos;  // Rigidbodyがないならこっち
            }
        } 
    }

    private void OnTriggerEnter(Collider other)
    {
        //リスポーンエリア通過でリスポーン地点更新
        if (other.CompareTag("RespawnArea"))
        {
            lastSavePos = other.transform.position;
        }
    }
}
