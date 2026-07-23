using UnityEngine;

public class BeltConveyor : MonoBehaviour
{
    //ベルトが動く方向
    public enum BeltDirection
    {
        Right,
        Left,
    }

    public float BeltPower = 100f;   //ベルトがプレイヤーに与える力
    public BeltDirection beltDirection = BeltDirection.Right;  //インスペクターで選択可能なベルトの方向

    //選択された方向に基づいて実際のベクトルを返す
    private Vector3 Direction => (beltDirection == BeltDirection.Right) ? Vector3.right : Vector3.left;


    //プレイヤーがベルトに乗っている間、毎フレーム通知を送る
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var BeltReceiver = other.GetComponent<IConveyorReceiver>();
            if(BeltReceiver != null)
            {
                //プレイヤーに「ベルトに乗っている」と通知し、ベルトの速度を渡す
                BeltReceiver.SetOnBelt(true, Direction.normalized * BeltPower);
            }
        }
    }

    //プレイヤーがベルトから離れた時に通知
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var BeltReceiver = other.GetComponent<IConveyorReceiver>();
            if(BeltReceiver != null)
            {
                //ベルトの影響を解除
                BeltReceiver.SetOnBelt(false, Vector3.zero);
            }
        }
    }
}
