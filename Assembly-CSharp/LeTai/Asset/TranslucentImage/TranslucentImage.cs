using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LeTai.Asset.TranslucentImage
{
	// Token: 0x020006BE RID: 1726
	public class TranslucentImage : Image, IMeshModifier
	{
		// Token: 0x06002651 RID: 9809 RVA: 0x000B0BC8 File Offset: 0x000AEDC8
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

		// Token: 0x06002652 RID: 9810 RVA: 0x000B0C40 File Offset: 0x000AEE40
		private void PrepShader()
		{
			this.correctShader = Shader.Find("UI/TranslucentImage");
			TranslucentImage._vibrancyPropId = Shader.PropertyToID("_Vibrancy");
			TranslucentImage._brightnessPropId = Shader.PropertyToID("_Brightness");
			TranslucentImage._flattenPropId = Shader.PropertyToID("_Flatten");
			TranslucentImage._blurTexPropId = Shader.PropertyToID("_BlurTex");
			TranslucentImage._cropRegionPropId = Shader.PropertyToID("_CropRegion");
		}

		// Token: 0x06002653 RID: 9811 RVA: 0x000B0CA8 File Offset: 0x000AEEA8
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

		// Token: 0x06002654 RID: 9812 RVA: 0x000B0DB4 File Offset: 0x000AEFB4
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

		// Token: 0x06002655 RID: 9813 RVA: 0x000B0E1C File Offset: 0x000AF01C
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

		// Token: 0x06002656 RID: 9814 RVA: 0x000B0E78 File Offset: 0x000AF078
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

		// Token: 0x06002657 RID: 9815 RVA: 0x000B0ED7 File Offset: 0x000AF0D7
		protected override void OnEnable()
		{
			base.OnEnable();
			this.SetVerticesDirty();
		}

		// Token: 0x06002658 RID: 9816 RVA: 0x000B0EE5 File Offset: 0x000AF0E5
		protected override void OnDisable()
		{
			this.SetVerticesDirty();
			base.OnDisable();
		}

		// Token: 0x06002659 RID: 9817 RVA: 0x000B0EF3 File Offset: 0x000AF0F3
		protected override void OnDidApplyAnimationProperties()
		{
			this.SetVerticesDirty();
			base.OnDidApplyAnimationProperties();
		}

		// Token: 0x0600265A RID: 9818 RVA: 0x000B0F04 File Offset: 0x000AF104
		public virtual void ModifyMesh(Mesh mesh)
		{
			using (VertexHelper vertexHelper = new VertexHelper(mesh))
			{
				this.ModifyMesh(vertexHelper);
				vertexHelper.FillMesh(mesh);
			}
		}

		// Token: 0x04002887 RID: 10375
		public TranslucentImageSource source;

		// Token: 0x04002888 RID: 10376
		[Tooltip("(De)Saturate them image, 1 is normal, 0 is black and white, below zero make the image negative")]
		[Range(-1f, 3f)]
		private float vibrancy = 1f;

		// Token: 0x04002889 RID: 10377
		[Tooltip("Brighten/darken them image")]
		[Range(-1f, 1f)]
		private float brightness;

		// Token: 0x0400288A RID: 10378
		[Range(0f, 1f)]
		[Tooltip("Flatten the color behind to help keep contrast on varying background")]
		private float flatten;

		// Token: 0x0400288B RID: 10379
		private Shader correctShader;

		// Token: 0x0400288C RID: 10380
		private static int _vibrancyPropId;

		// Token: 0x0400288D RID: 10381
		private static int _brightnessPropId;

		// Token: 0x0400288E RID: 10382
		private static int _flattenPropId;

		// Token: 0x0400288F RID: 10383
		private static int _blurTexPropId;

		// Token: 0x04002890 RID: 10384
		private static int _cropRegionPropId;

		// Token: 0x04002891 RID: 10385
		private float oldVibrancy;

		// Token: 0x04002892 RID: 10386
		private float oldBrightness;

		// Token: 0x04002893 RID: 10387
		private float oldFlatten;

		// Token: 0x04002894 RID: 10388
		[Tooltip("Blend between the sprite and background blur")]
		[Range(0f, 1f)]
		public float spriteBlending = 0.65f;
	}
}
