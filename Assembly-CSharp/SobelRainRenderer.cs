using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000023 RID: 35
public sealed class SobelRainRenderer : PostProcessEffectRenderer<SobelRain>
{
	// Token: 0x06000095 RID: 149 RVA: 0x00004F38 File Offset: 0x00003138
	public override void Render(PostProcessRenderContext context)
	{
		PropertySheet propertySheet = context.propertySheets.Get(Shader.Find("Hidden/PostProcess/SobelRain"));
		propertySheet.properties.SetFloat("_RainIntensity", base.settings.rainInensity);
		propertySheet.properties.SetFloat("_OutlineScale", base.settings.outlineScale);
		propertySheet.properties.SetFloat("_RainDensity", base.settings.rainDensity);
		propertySheet.properties.SetTexture("_RainTexture", base.settings.rainTexture);
		propertySheet.properties.SetColor("_RainColor", base.settings.rainColor);
		context.command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0, false, null);
	}
}
