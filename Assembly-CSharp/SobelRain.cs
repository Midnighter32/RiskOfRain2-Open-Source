using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000022 RID: 34
[PostProcess(typeof(SobelRainRenderer), PostProcessEvent.BeforeTransparent, "PostProcess/SobelRain", true)]
[Serializable]
public sealed class SobelRain : PostProcessEffectSettings
{
	// Token: 0x040000AA RID: 170
	[Range(0f, 100f)]
	[Tooltip("The intensity of the rain.")]
	public FloatParameter rainInensity = new FloatParameter
	{
		value = 0.5f
	};

	// Token: 0x040000AB RID: 171
	[Range(0f, 10f)]
	[Tooltip("The falloff of the outline. Higher values means it relies less on the sobel.")]
	public FloatParameter outlineScale = new FloatParameter
	{
		value = 1f
	};

	// Token: 0x040000AC RID: 172
	[Range(0f, 1f)]
	[Tooltip("The density of rain.")]
	public FloatParameter rainDensity = new FloatParameter
	{
		value = 1f
	};

	// Token: 0x040000AD RID: 173
	public TextureParameter rainTexture = new TextureParameter
	{
		value = null
	};

	// Token: 0x040000AE RID: 174
	public ColorParameter rainColor = new ColorParameter
	{
		value = Color.white
	};
}
