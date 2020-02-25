using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LeTai.Asset.TranslucentImage
{
	// Token: 0x02000909 RID: 2313
	public class TranslucentImage : Image, IMeshModifier
	{
		// Token: 0x0600339B RID: 13211 RVA: 0x000E0008 File Offset: 0x000DE208
		protected override void Start()
		{
			base.Start();
			this.PrepShader();
			this.oldVibrancy = this.vibrancy;
			this.oldBrightness = this.brightness;
			this.oldFlatten = this.flatten;
			this.source = (this.source ? this.source : UnityEngine.Object.FindObjectOfType<TranslucentImageSource>());
			this.material.SetTexture(TranslucentImage._blurTexPropId, this.source.BlurredScreen);
		}

		// Token: 0x0600339C RID: 13212 RVA: 0x000E0080 File Offset: 0x000DE280
		private void PrepShader()
		{
			this.correctShader = Shader.Find("UI/TranslucentImage");
			TranslucentImage._vibrancyPropId = Shader.PropertyToID("_Vibrancy");
			TranslucentImage._brightnessPropId = Shader.PropertyToID("_Brightness");
			TranslucentImage._flattenPropId = Shader.PropertyToID("_Flatten");
			TranslucentImage._blurTexPropId = Shader.PropertyToID("_BlurTex");
			TranslucentImage._cropRegionPropId = Shader.PropertyToID("_CropRegion");
		}

		// Token: 0x0600339D RID: 13213 RVA: 0x000E00E8 File Offset: 0x000DE2E8
		private void LateUpdate()
		{
			this.source = (this.source ? this.source : UnityEngine.Object.FindObjectOfType<TranslucentImageSource>());
			if (!this.source)
			{
				Debug.LogError("Source missing. Add TranslucentImageSource component to your main camera, then drag the camera to Source slot");
				return;
			}
			if (!this.IsActive() || !this.source.BlurredScreen)
			{
				return;
			}
			if (!this.material || this.material.shader != this.correctShader)
			{
				Debug.LogError("Material using \"UI/TranslucentImage\" is required");
			}
			this.materialForRendering.SetTexture(TranslucentImage._blurTexPropId, this.source.BlurredScreen);
			this.materialForRendering.SetVector(TranslucentImage._cropRegionPropId, new Vector4(this.source.BlurRegion.xMin, this.source.BlurRegion.yMin, this.source.BlurRegion.xMax, this.source.BlurRegion.yMax));
		}

		// Token: 0x0600339E RID: 13214 RVA: 0x000E01F4 File Offset: 0x000DE3F4
		private void Update()
		{
			if (TranslucentImage._vibrancyPropId == 0 || TranslucentImage._brightnessPropId == 0 || TranslucentImage._flattenPropId == 0)
			{
				return;
			}
			this.SyncMaterialProperty(TranslucentImage._vibrancyPropId, ref this.vibrancy, ref this.oldVibrancy);
			this.SyncMaterialProperty(TranslucentImage._brightnessPropId, ref this.brightness, ref this.oldBrightness);
			this.SyncMaterialProperty(TranslucentImage._flattenPropId, ref this.flatten, ref this.oldFlatten);
		}

		// Token: 0x0600339F RID: 13215 RVA: 0x000E025C File Offset: 0x000DE45C
		private void SyncMaterialProperty(int propId, ref float value, ref float oldValue)
		{
			float @float = this.materialForRendering.GetFloat(propId);
			if (!Mathf.Approximately(@float, value))
			{
				if (!Mathf.Approximately(value, oldValue))
				{
					this.material.SetFloat(propId, value);
					this.materialForRendering.SetFloat(propId, value);
					this.SetMaterialDirty();
				}
				else
				{
					value = @float;
				}
			}
			oldValue = value;
		}

		// Token: 0x060033A0 RID: 13216 RVA: 0x000E02B8 File Offset: 0x000DE4B8
		public virtual void ModifyMesh(VertexHelper vh)
		{
			List<UIVertex> list = new List<UIVertex>();
			vh.GetUIVertexStream(list);
			for (int i = 0; i < list.Count; i++)
			{
				UIVertex value = list[i];
				value.uv1 = new Vector2(this.spriteBlending, 0f);
				list[i] = value;
			}
			vh.Clear();
			vh.AddUIVertexTriangleStream(list);
		}

		// Token: 0x060033A1 RID: 13217 RVA: 0x000E0317 File Offset: 0x000DE517
		protected override void OnEnable()
		{
			base.OnEnable();
			this.SetVerticesDirty();
		}

		// Token: 0x060033A2 RID: 13218 RVA: 0x000E0325 File Offset: 0x000DE525
		protected override void OnDisable()
		{
			this.SetVerticesDirty();
			base.OnDisable();
		}

		// Token: 0x060033A3 RID: 13219 RVA: 0x000E0333 File Offset: 0x000DE533
		protected override void OnDidApplyAnimationProperties()
		{
			this.SetVerticesDirty();
			base.OnDidApplyAnimationProperties();
		}

		// Token: 0x060033A4 RID: 13220 RVA: 0x000E0344 File Offset: 0x000DE544
		public virtual void ModifyMesh(Mesh mesh)
		{
			using (VertexHelper vertexHelper = new VertexHelper(mesh))
			{
				this.ModifyMesh(vertexHelper);
				vertexHelper.FillMesh(mesh);
			}
		}

		// Token: 0x04003320 RID: 13088
		public TranslucentImageSource source;

		// Token: 0x04003321 RID: 13089
		[Range(-1f, 3f)]
		[Tooltip("(De)Saturate them image, 1 is normal, 0 is black and white, below zero make the image negative")]
		private float vibrancy = 1f;

		// Token: 0x04003322 RID: 13090
		[Range(-1f, 1f)]
		[Tooltip("Brighten/darken them image")]
		private float brightness;

		// Token: 0x04003323 RID: 13091
		[Tooltip("Flatten the color behind to help keep contrast on varying background")]
		[Range(0f, 1f)]
		private float flatten;

		// Token: 0x04003324 RID: 13092
		private Shader correctShader;

		// Token: 0x04003325 RID: 13093
		private static int _vibrancyPropId;

		// Token: 0x04003326 RID: 13094
		private static int _brightnessPropId;

		// Token: 0x04003327 RID: 13095
		private static int _flattenPropId;

		// Token: 0x04003328 RID: 13096
		private static int _blurTexPropId;

		// Token: 0x04003329 RID: 13097
		private static int _cropRegionPropId;

		// Token: 0x0400332A RID: 13098
		private float oldVibrancy;

		// Token: 0x0400332B RID: 13099
		private float oldBrightness;

		// Token: 0x0400332C RID: 13100
		private float oldFlatten;

		// Token: 0x0400332D RID: 13101
		[Tooltip("Blend between the sprite and background blur")]
		[Range(0f, 1f)]
		public float spriteBlending = 0.65f;
	}
}
