using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000028 RID: 40
public sealed class SobelOutlineRenderer : PostProcessEffectRenderer<SobelOutline>
{
	// Token: 0x060000B9 RID: 185 RVA: 0x00005204 File Offset: 0x00003404
	public override void Render(PostProcessRenderContext context)
	{
		PropertySheet propertySheet = context.propertySheets.Get(Shader.Find("Hidden/PostProcess/SobelOutline"));
		propertySheet.properties.SetFloat("_OutlineIntensity", base.settings.outlineIntensity);
		propertySheet.properties.SetFloat("_OutlineScale", base.settings.outlineScale);
		context.command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0, false, null);
	}
}
