//using UnityEngine;

//public class SpikeDamage : MonoBehaviour
//{
//    public int damage = 1;

//    [Header("反応させるレイヤー（複数選択可）")]
//    public LayerMask targetMask;

//    private void OnTriggerEnter(Collider other)
//    {
//        // 1. まずタグが Player かチェック
//        if (!other.CompareTag("Player")) return;

//        // 2. 接触したオブジェクトのレイヤーが targetMask に含まれているか判定
//        // (1 << other.gameObject.layer) でレイヤー番号をビットに変換し、AND演算を行います
//        if (((1 << other.gameObject.layer) & targetMask) != 0)
//        {
//            PlayerHealth health = other.GetComponent<PlayerHealth>();
//            if (health != null)
//            {
//                health.TakeDamage(damage);
//            }
//        }
//    }
//}