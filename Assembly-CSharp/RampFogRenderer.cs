using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x0200001F RID: 31
public sealed class RampFogRenderer : PostProcessEffectRenderer<RampFog>
{
	// Token: 0x0600008F RID: 143 RVA: 0x00004C44 File Offset: 0x00002E44
	public override void Render(PostProcessRenderContext context)
	{
		PropertySheet propertySheet = context.propertySheets.Get(Shader.Find("Hidden/PostProcess/RampFog"));
		propertySheet.properties.SetFloat("_FogIntensity", base.settings.fogIntensity);
		propertySheet.properties.SetFloat("_FogPower", base.settings.fogPower);
		propertySheet.properties.SetFloat("_FogZero", base.settings.fogZero);
		propertySheet.properties.SetFloat("_FogOne", base.settings.fogOne);
		propertySheet.properties.SetFloat("_FogHeightStart", base.settings.fogHeightStart);
		propertySheet.properties.SetFloat("_FogHeightEnd", base.settings.fogHeightEnd);
		propertySheet.properties.SetFloat("_FogHeightIntensity", base.settings.fogHeightIntensity);
		propertySheet.properties.SetColor("_FogColorStart", base.settings.fogColorStart);
		propertySheet.properties.SetColor("_FogColorMid", base.settings.fogColorMid);
		propertySheet.properties.SetColor("_FogColorEnd", base.settings.fogColorEnd);
		propertySheet.properties.SetFloat("_SkyboxStrength", base.settings.skyboxStrength);
		context.command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0, false, null);
	}
}
