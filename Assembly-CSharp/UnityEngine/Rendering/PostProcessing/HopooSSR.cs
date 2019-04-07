using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x020001E9 RID: 489
	[PostProcess(typeof(HopooSSRRenderer), PostProcessEvent.BeforeTransparent, "PostProcess/Hopoo SSR", true)]
	[Serializable]
	public sealed class HopooSSR : PostProcessEffectSettings
	{
		// Token: 0x06000985 RID: 2437 RVA: 0x0002FF98 File Offset: 0x0002E198
		public override bool IsEnabledAndSupported(PostProcessRenderContext context)
		{
			return this.enabled && context.camera.actualRenderingPath == RenderingPath.DeferredShading && SystemInfo.supportsMotionVectors && SystemInfo.supportsComputeShaders && SystemInfo.copyTextureSupport > CopyTextureSupport.None && context.resources.shaders.screenSpaceReflections && context.resources.shaders.screenSpaceReflections.isSupported && context.resources.computeShaders.gaussianDownsample;
		}

		// Token: 0x04000CDF RID: 3295
		[Tooltip("Choose a quality preset, or use \"Custom\" to fine tune it. Don't use a preset higher than \"Medium\" if you care about performances on consoles.")]
		public ScreenSpaceReflectionPresetParameter preset = new ScreenSpaceReflectionPresetParameter
		{
			value = ScreenSpaceReflectionPreset.Medium
		};

		// Token: 0x04000CE0 RID: 3296
		[Range(0f, 256f)]
		[Tooltip("Maximum iteration count.")]
		public IntParameter maximumIterationCount = new IntParameter
		{
			value = 16
		};

		// Token: 0x04000CE1 RID: 3297
		[Tooltip("Changes the size of the SSR buffer. Downsample it to maximize performances or supersample it to get slow but higher quality results.")]
		public ScreenSpaceReflectionResolutionParameter resolution = new ScreenSpaceReflectionResolutionParameter
		{
			value = ScreenSpaceReflectionResolution.Downsampled
		};

		// Token: 0x04000CE2 RID: 3298
		[Range(1f, 64f)]
		[Tooltip("Ray thickness. Lower values are more expensive but allow the effect to detect smaller details.")]
		public FloatParameter thickness = new FloatParameter
		{
			value = 8f
		};

		// Token: 0x04000CE3 RID: 3299
		[Tooltip("Maximum distance to traverse after which it will stop drawing reflections.")]
		public FloatParameter maximumMarchDistance = new FloatParameter
		{
			value = 100f
		};

		// Token: 0x04000CE4 RID: 3300
		[Tooltip("Fades reflections close to the near planes.")]
		[Range(0f, 1f)]
		public FloatParameter distanceFade = new FloatParameter
		{
			value = 0.5f
		};

		// Token: 0x04000CE5 RID: 3301
		[Range(0f, 1f)]
		[Tooltip("Fades reflections close to the screen edges.")]
		public FloatParameter vignette = new FloatParameter
		{
			value = 0.5f
		};
	}
}
