using System;
using UnityEngine;

namespace RoR2.PostProcessing
{
	// Token: 0x0200052E RID: 1326
	[ExecuteInEditMode]
	public class HopooPostProcess : MonoBehaviour
	{
		// Token: 0x06001F5C RID: 8028 RVA: 0x0000409B File Offset: 0x0000229B
		private void Start()
		{
		}

		// Token: 0x06001F5D RID: 8029 RVA: 0x00088321 File Offset: 0x00086521
		[ImageEffectOpaque]
		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			Graphics.Blit(source, destination, this.mat);
		}

		// Token: 0x04001D05 RID: 7429
		public Material mat;
	}
}
