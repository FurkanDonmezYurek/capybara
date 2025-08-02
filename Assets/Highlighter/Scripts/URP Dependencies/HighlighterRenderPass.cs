using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

namespace Highlighter
{
    public class HighlighterRenderPass : ScriptableRenderPass
    {
        private HighlighterRenderFeature renderFeature;
        private RenderTargetIdentifier source;
        private RenderTargetIdentifier destination;
        private int destinationPtr = Shader.PropertyToID("_Destination");

        public HighlighterRenderPass(HighlighterRenderFeature renderFeature)
        { 
            this.renderFeature = renderFeature;
        }
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            var camDescriptor = renderingData.cameraData.cameraTargetDescriptor;
            cmd.GetTemporaryRT(destinationPtr, camDescriptor.width, camDescriptor.height, 0, FilterMode.Bilinear, camDescriptor.colorFormat);
            source = renderingData.cameraData.renderer.cameraColorTarget;
            destination = new RenderTargetIdentifier(destinationPtr);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.isSceneViewCamera)
                return;

            CommandBuffer cmd = CommandBufferPool.Get("RenderPassURP");

            if (renderFeature.isActive)
            {   
                HighlighterManager.UpdateData(renderingData.cameraData.camera);
                HighlighterManager.Render(cmd, source,ref destination);
                cmd.Blit(destination, source);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(destinationPtr);
        }
    }
}