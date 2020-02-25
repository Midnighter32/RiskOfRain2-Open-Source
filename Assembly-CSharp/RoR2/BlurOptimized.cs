using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000113 RID: 275
	[RequireComponent(typeof(Camera))]
	public class BlurOptimized : MonoBehaviour
	{
		// Token: 0x06000519 RID: 1305 RVA: 0x000144E6 File Offset: 0x000126E6
		public void Start()
		{
			this.blurMaterial = new Material(Shader.Find("Hidden/FastBlur"));
			base.enabled = false;
		}

		// Token: 0x0600051A RID: 1306 RVA: 0x00014504 File Offset: 0x00012704
		public void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			float num = 1f / (1f * (float)(1 << this.downsample));
			this.blurMaterial.SetVector("_Parameter", new Vector4(this.blurSize * num, -this.blurSize * num, 0f, 0f));
			source.filterMode = FilterMode.Bilinear;
			int width = source.width >> this.downsample;
			int height = source.height >> this.downsample;
			RenderTexture renderTexture = RenderTexture.GetTemporary(width, height, 0, source.format);
			renderTexture.filterMode = FilterMode.Bilinear;
			Graphics.Blit(source, renderTexture, this.blurMaterial, 0);
			int num2 = (this.blurType == BlurOptimized.BlurType.StandardGauss) ? 0 : 2;
			for (int i = 0; i < this.blurIterations; i++)
			{
				float num3 = (float)i * 1f;
				this.blurMaterial.SetVector("_Parameter", new Vector4(this.blurSize * num + num3, -this.blurSize * num - num3, 0f, 0f));
				RenderTexture temporary = RenderTexture.GetTemporary(width, height, 0, source.format);
				temporary.filterMode = FilterMode.Bilinear;
				Graphics.Blit(renderTexture, temporary, this.blurMaterial, 1 + num2);
				RenderTexture.ReleaseTemporary(renderTexture);
				renderTexture = temporary;
				temporary = RenderTexture.GetTemporary(width, height, 0, source.format);
				temporary.filterMode = FilterMode.Bilinear;
				Graphics.Blit(renderTexture, temporary, this.blurMaterial, 2 + num2);
				RenderTexture.ReleaseTemporary(renderTexture);
				renderTexture = temporary;
			}
			Graphics.Blit(renderTexture, destination);
			RenderTexture.ReleaseTemporary(renderTexture);
		}

		// Token: 0x0400050E RID: 1294
		[Range(0f, 2f)]
		public int downsample = 1;

		// Token: 0x0400050F RID: 1295
		[Range(0f, 10f)]
		public float blurSize = 3f;

		// Token: 0x04000510 RID: 1296
		[Range(1f, 4f)]
		public int blurIterations = 2;

		// Token: 0x04000511 RID: 1297
		public BlurOptimized.BlurType blurType;

		// Token: 0x04000512 RID: 1298
		[HideInInspector]
		public Material blurMaterial;

		// Token: 0x02000114 RID: 276
		public enum BlurType
		{
			// Token: 0x04000514 RID: 1300
			StandardGauss,
			// Token: 0x04000515 RID: 1301
			SgxGauss
		}
	}
}
