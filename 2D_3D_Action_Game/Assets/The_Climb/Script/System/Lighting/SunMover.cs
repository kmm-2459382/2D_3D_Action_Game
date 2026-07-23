using UnityEngine;

//    太陽移動スクリプト
public class SunMover : MonoBehaviour
{
    Transform sunTransform;    //  太陽のTF
    void Awake()
    {
        //  初期化
        Initialize();
    }
    //  初期化
    void Initialize()
    {
        sunTransform = transform;
    }
    //  角度変更
    public void ChangeAngle(float SunNewRotX)
    {
        sunTransform.rotation = Quaternion.Euler(SunNewRotX, 0f, 0f);
    }
}