using UnityEngine;

//ベルトコンベアの影響を受けるオブジェクトに実装させるインターフェース
public interface IConveyorReceiver
{
    //ベルトに乗っているかどうかの判定とベルトの速度を渡す
    void SetOnBelt(bool OnBelt, Vector3 BeltVelocity);
}
