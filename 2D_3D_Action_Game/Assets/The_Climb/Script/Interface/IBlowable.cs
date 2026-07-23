using UnityEngine;

//  吹き飛ばしインターフェース
public interface IBlowable
{
    //  吹っ飛ばす関数
    void Blow(Rigidbody blowTagert, float Direction);
}