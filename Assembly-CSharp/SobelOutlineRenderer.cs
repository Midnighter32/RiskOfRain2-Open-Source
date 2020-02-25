using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000021 RID: 33
public sealed class SobelOutlineRenderer : PostProcessEffectRenderer<SobelOutline>
{
	// Token: 0x06000092 RID: 146 RVA: 0x00004E28 File Offset: 0x00003028
	public override void Render(PostProcessRenderContext context)
	{
		PropertySheet propertySheet = context.propertySheets.Get(Shader.Find("Hidden/PostProcess/SobelOutline"));
		propertySheet.properties.SetFloat("_OutlineIntensity", base.settings.outlineIntensity);
		propertySheet.properties.SetFloat("_OutlineScale", base.settings.outlineScale);
		context.command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0, false, null);
	}
}
