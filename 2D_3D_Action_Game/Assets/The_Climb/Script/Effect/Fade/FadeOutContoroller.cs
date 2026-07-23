//using UnityEngine;
//using System.Collections;

////  フェードアウト進行度コントロール
//public class FadeController : MonoBehaviour
//{
//    CustomFadeFeature customFadeFeature;
//    public Material fadeMaterial;    //  フェードアウトするマテリアル
//    private Material runtimeMaterial;    //  実行時のマテリアル
//    Coroutine fade;    //  フェードコルーチン

//    [SerializeField] float FadeDuration;    //  フェードする

//    void Awake()
//    {
//        // 共有マテリアルを汚さないためインスタンス化
//        runtimeMaterial = new Material(fadeMaterial);

//        Initialize();
//    }
//    void Start()
//    {
//        fade = StartCoroutine(Fade(1f, 0f));
//    }
//    //  初期化関数
//    void Initialize()
//    {
//        FadeDuration = 1.0f;  
//    }
//    //  フェードルーチン
//    public IEnumerator Fade(float from, float to)
//    {
//        float FadingTime = 0f;    //  時間カウント
//        while (FadingTime < FadeDuration)
//        {
//            FadingTime += Time.deltaTime;
//            float EasedT = Mathf.SmoothStep(0f, 1f, FadingTime / FadeDuration);
//            customFadeFeature.settings.fadeMaterial.SetFloat("_Fade", EasedT);
//            yield return null;
//            Debug.Log(runtimeMaterial.GetFloat("_Fade"));
//        }
//        runtimeMaterial.SetFloat("_Fade", to);
//    }
//}