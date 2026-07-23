//using UnityEngine;
//using UnityEngine.Rendering;
//using UnityEngine.Rendering.Universal;

////  画面フェードアウト処理
//public class CustomFadeFeature : ScriptableRendererFeature
//{
//    //  フェードのための設定クラス
//    [System.Serializable]
//    public class FadeSettings
//    {
//        public Material fadeMaterial;  // フェードアウトするマテリアル
//    }

//    public FadeSettings settings = new FadeSettings();    //  設定クラス

//    //  レンダーパスクラス
//    class CustomFadePass : ScriptableRenderPass
//    {
//        private Material fadeMaterial;    //  フェードするマテリアル
//        private RTHandle source;    //  描画対象のテクスチャ
//        private string ProfilerTag = "Custom Fade Pass";    //  描画処理パス

//        //  パスのコンストラクタ
//        public CustomFadePass(Material material)
//        {
//            this.fadeMaterial =  new Material(material);
//            renderPassEvent = RenderPassEvent.AfterRendering;
//        }
//        //  マテリアルにフェードちを代入
//        public void SetFadeValue(float fadeValue)
//        {
//            fadeMaterial.SetFloat("_Fade", fadeValue);
//        }

//        //  描画セットアップ関数
//        //public void Setup(RTHandle cameraColorTarget)W
//        //{
//        //    this.source = cameraColorTarget;
//        //}

//        //  描画実行関数
//        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
//        {
//            if (fadeMaterial == null) return;

//            CommandBuffer cmd = CommandBufferPool.Get(ProfilerTag);    //  コマンドバッファの取得

//            RTHandle cameraTarget = renderingData.cameraData.renderer.cameraColorTargetHandle;

//            Blitter.BlitCameraTexture(cmd, cameraTarget, cameraTarget, fadeMaterial, 0);
//            context.ExecuteCommandBuffer(cmd);
//            CommandBufferPool.Release(cmd);
//        }
//    }

//    CustomFadePass fadePass;    //  フェード処理パス

//    //  初期化関数
//    public override void Create()
//    {
//        fadePass = new CustomFadePass(settings.fadeMaterial);
//    }

//    //  パス登録関数
//    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
//    {
//        if (settings.fadeMaterial == null) return;
//    }
//}
////fadePass.Setup(renderer.cameraColorTargetHandle);  // RTHandleで受け渡し
////renderer.EnqueuePass(fadePass);
////Debug.Log(21);
