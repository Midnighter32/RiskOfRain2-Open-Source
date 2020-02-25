using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000020 RID: 32
[PostProcess(typeof(SobelOutlineRenderer), PostProcessEvent.BeforeTransparent, "PostProcess/SobelOutline", true)]
[Serializable]
public sealed class SobelOutline : PostProcessEffectSettings
{
	// Token: 0x040000A8 RID: 168
	[Range(0f, 5f)]
	[Tooltip("The intensity of the outline.")]
	public FloatParameter outlineIntensity = new FloatParameter
	{
		value = 0.5f
	};

	// Token: 0x040000A9 RID: 169
	[Range(0f, 10f)]
	[Tooltip("The falloff of the outline.")]
	public FloatParameter outlineScale = new FloatParameter
	{
		value = 1f
	};
}
