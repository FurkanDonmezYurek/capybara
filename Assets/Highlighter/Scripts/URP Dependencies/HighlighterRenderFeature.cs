using UnityEngine.Rendering.Universal;

namespace Highlighter
{
    public class HighlighterRenderFeature : ScriptableRendererFeature
    {
        private HighlighterRenderPass urpRenderPass;
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        public override void Create()
        {
            urpRenderPass = new HighlighterRenderPass(this);
            urpRenderPass.renderPassEvent = renderPassEvent;
        }
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) => renderer.EnqueuePass(urpRenderPass);
        

        private void OnValidate()
        {
            if (urpRenderPass != null)
                urpRenderPass.renderPassEvent = renderPassEvent;
        }
    }
}