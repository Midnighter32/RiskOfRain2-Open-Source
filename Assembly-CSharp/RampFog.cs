using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x0200001E RID: 30
[PostProcess(typeof(RampFogRenderer), PostProcessEvent.BeforeTransparent, "PostProcess/RampFog", true)]
[Serializable]
public sealed class RampFog : PostProcessEffectSettings
{
	// Token: 0x0400009D RID: 157
	[Tooltip("Fog intensity.")]
	[Range(0f, 1f)]
	public FloatParameter fogIntensity = new FloatParameter
	{
		value = 0.5f
	};

	// Token: 0x0400009E RID: 158
	[Tooltip("Fog Power")]
	[Range(0f, 20f)]
	public FloatParameter fogPower = new FloatParameter
	{
		value = 1f
	};

	// Token: 0x0400009F RID: 159
	[Range(-1f, 1f)]
	[Tooltip("The zero value for the fog depth remap.")]
	public FloatParameter fogZero = new FloatParameter
	{
		value = 0f
	};

	// Token: 0x040000A0 RID: 160
	[Tooltip("The one value for the fog depth remap.")]
	[Range(-1f, 1f)]
	public FloatParameter fogOne = new FloatParameter
	{
		value = 1f
	};

	// Token: 0x040000A1 RID: 161
	[Range(-100f, 100f)]
	[Tooltip("The world position value where the height fog begins.")]
	public FloatParameter fogHeightStart = new FloatParameter
	{
		value = 0f
	};

	// Token: 0x040000A2 RID: 162
	[Tooltip("The world position value where the height fog ends.")]
	[Range(-100f, 600f)]
	public FloatParameter fogHeightEnd = new FloatParameter
	{
		value = 100f
	};

	// Token: 0x040000A3 RID: 163
	[Range(0f, 5f)]
	[Tooltip("The overall strength of the height fog.")]
	public FloatParameter fogHeightIntensity = new FloatParameter
	{
		value = 0f
	};

	// Token: 0x040000A4 RID: 164
	[Tooltip("Color of the fog at the beginning.")]
	public ColorParameter fogColorStart = new ColorParameter
	{
		value = Color.white
	};

	// Token: 0x040000A5 RID: 165
	[Tooltip("Color of the fog at the middle.")]
	public ColorParameter fogColorMid = new ColorParameter
	{
		value = Color.white
	};

	// Token: 0x040000A6 RID: 166
	[Tooltip("Color of the fog at the end.")]
	public ColorParameter fogColorEnd = new ColorParameter
	{
		value = Color.white
	};

	// Token: 0x040000A7 RID: 167
	[Tooltip("How much of the skybox will leak through?")]
	[Range(0f, 1f)]
	public FloatParameter skyboxStrength = new FloatParameter
	{
		value = 0f
	};
}
