using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200022F RID: 559
	[RequireComponent(typeof(Camera))]
	public class BlurOptimized : MonoBehaviour
	{
		// Token: 0x06000AB5 RID: 2741 RVA: 0x00034DDA File Offset: 0x00032FDA
		public void Start()
		{
			this.blurMaterial = new Material(Shader.Find("Hidden/FastBlur"));
			base.enabled = false;
		}

		// Token: 0x06000AB6 RID: 2742 RVA: 0x00034DF8 File Offset: 0x00032FF8
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

		// Token: 0x04000E4D RID: 3661
		[Range(0f, 2f)]
		public int downsample = 1;

		// Token: 0x04000E4E RID: 3662
		[Range(0f, 10f)]
		public float blurSize = 3f;

		// Token: 0x04000E4F RID: 3663
		[Range(1f, 4f)]
		public int blurIterations = 2;

		// Token: 0x04000E50 RID: 3664
		public BlurOptimized.BlurType blurType;

		// Token: 0x04000E51 RID: 3665
		[HideInInspector]
		public Material blurMaterial;

		// Token: 0x02000230 RID: 560
		public enum BlurType
		{
			// Token: 0x04000E53 RID: 3667
			StandardGauss,
			// Token: 0x04000E54 RID: 3668
			SgxGauss
		}
	}
}
