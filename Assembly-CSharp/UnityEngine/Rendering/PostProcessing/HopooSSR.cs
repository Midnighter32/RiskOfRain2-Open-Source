using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x020000AA RID: 170
	[PostProcess(typeof(HopooSSRRenderer), PostProcessEvent.BeforeTransparent, "PostProcess/Hopoo SSR", true)]
	[Serializable]
	public sealed class HopooSSR : PostProcessEffectSettings
	{
		// Token: 0x06000345 RID: 837 RVA: 0x0000CCE8 File Offset: 0x0000AEE8
		public override bool IsEnabledAndSupported(PostProcessRenderContext context)
		{
			return this.enabled && context.camera.actualRenderingPath == RenderingPath.DeferredShading && SystemInfo.supportsMotionVectors && SystemInfo.supportsComputeShaders && SystemInfo.copyTextureSupport > CopyTextureSupport.None && context.resources.shaders.screenSpaceReflections && context.resources.shaders.screenSpaceReflections.isSupported && context.resources.computeShaders.gaussianDownsample;
		}

		// Token: 0x040002F1 RID: 753
		[Tooltip("Choose a quality preset, or use \"Custom\" to fine tune it. Don't use a preset higher than \"Medium\" if you care about performances on consoles.")]
		public ScreenSpaceReflectionPresetParameter preset = new ScreenSpaceReflectionPresetParameter
		{
			value = ScreenSpaceReflectionPreset.Medium
		};

		// Token: 0x040002F2 RID: 754
		[Tooltip("Maximum iteration count.")]
		[Range(0f, 256f)]
		public IntParameter maximumIterationCount = new IntParameter
		{
			value = 16
		};

		// Token: 0x040002F3 RID: 755
		[Tooltip("Changes the size of the SSR buffer. Downsample it to maximize performances or supersample it to get slow but higher quality results.")]
		public ScreenSpaceReflectionResolutionParameter resolution = new ScreenSpaceReflectionResolutionParameter
		{
			value = ScreenSpaceReflectionResolution.Downsampled
		};

		// Token: 0x040002F4 RID: 756
		[Tooltip("Ray thickness. Lower values are more expensive but allow the effect to detect smaller details.")]
		[Range(1f, 64f)]
		public FloatParameter thickness = new FloatParameter
		{
			value = 8f
		};

		// Token: 0x040002F5 RID: 757
		[Tooltip("Maximum distance to traverse after which it will stop drawing reflections.")]
		public FloatParameter maximumMarchDistance = new FloatParameter
		{
			value = 100f
		};

		// Token: 0x040002F6 RID: 758
		[Tooltip("Fades reflections close to the near planes.")]
		[Range(0f, 1f)]
		public FloatParameter distanceFade = new FloatParameter
		{
			value = 0.5f
		};

		// Token: 0x040002F7 RID: 759
		[Tooltip("Fades reflections close to the screen edges.")]
		[Range(0f, 1f)]
		public FloatParameter vignette = new FloatParameter
		{
			value = 0.5f
		};
	}
}
