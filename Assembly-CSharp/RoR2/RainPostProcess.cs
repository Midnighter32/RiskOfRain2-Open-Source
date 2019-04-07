using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003A3 RID: 931
	[ExecuteInEditMode]
	public class RainPostProcess : MonoBehaviour
	{
		// Token: 0x060013BB RID: 5051 RVA: 0x00002058 File Offset: 0x00000258
		private void Start()
		{
			base.GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
		}

		// Token: 0x060013BC RID: 5052 RVA: 0x00004507 File Offset: 0x00002707
		private void Update()
		{
		}

		// Token: 0x060013BD RID: 5053 RVA: 0x0006084A File Offset: 0x0005EA4A
		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			Graphics.Blit(source, destination, this.mat);
		}

		// Token: 0x0400176C RID: 5996
		public Material mat;

		// Token: 0x0400176D RID: 5997
		private RenderTexture renderTex;
	}
}
