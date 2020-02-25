using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002E0 RID: 736
	[ExecuteInEditMode]
	public class RainPostProcess : MonoBehaviour
	{
		// Token: 0x060010E2 RID: 4322 RVA: 0x00002058 File Offset: 0x00000258
		private void Start()
		{
			base.GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
		}

		// Token: 0x060010E3 RID: 4323 RVA: 0x0000409B File Offset: 0x0000229B
		private void Update()
		{
		}

		// Token: 0x060010E4 RID: 4324 RVA: 0x0004A22E File Offset: 0x0004842E
		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			Graphics.Blit(source, destination, this.mat);
		}

		// Token: 0x0400103B RID: 4155
		public Material mat;

		// Token: 0x0400103C RID: 4156
		private RenderTexture renderTex;
	}
}
