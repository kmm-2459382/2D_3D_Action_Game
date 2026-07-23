//using UnityEngine;
//using UnityEngine.Rendering;
//using UnityEngine.Rendering.Universal;

//public class DarkToCam : ScriptableRendererFeature
//{
//    class DarknessOverlayPass : ScriptableRenderPass
//    {
//        Material material;
//        RTHandle cameraTargetHandle;

//        float progress = 0f;

//        public DarknessOverlayPass(Material mat)
//        {
//            material = mat;
//            renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
//        }

//        public void Setup(RTHandle handle)
//        {
//            cameraTargetHandle = handle;
//        }

//        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
//        {
//            var cameraTarget = renderingData.cameraData.renderer.cameraColorTargetHandle;


//            CommandBuffer cmd = CommandBufferPool.Get("DarknessOverlayPass");
//            // 必要なセット
//            cmd.SetGlobalFloat("_Progress", progress);
//            cmd.SetGlobalTexture("_MainTex", cameraTarget); // ★ これを追加！

//            // Blit (source -> destination using material)
//            cmd.Blit(cameraTarget, cameraTarget, material);

//            context.ExecuteCommandBuffer(cmd);
//            CommandBufferPool.Release(cmd);
//        }
//    }

//    DarknessOverlayPass pass;
//    public Material overlayMaterial;

//    public override void Create()
//    {
//        pass = new DarknessOverlayPass(overlayMaterial);
//    }

//    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
//    {
//        renderer.EnqueuePass(pass);
//    }
//}