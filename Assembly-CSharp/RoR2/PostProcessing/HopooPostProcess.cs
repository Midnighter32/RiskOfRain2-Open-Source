using System;
using UnityEngine;

namespace RoR2.PostProcessing
{
	// Token: 0x02000564 RID: 1380
	[ExecuteInEditMode]
	public class HopooPostProcess : MonoBehaviour
	{
		// Token: 0x06001ECE RID: 7886 RVA: 0x00004507 File Offset: 0x00002707
		private void Start()
		{
		}

		// Token: 0x06001ECF RID: 7887 RVA: 0x00091751 File Offset: 0x0008F951
		[ImageEffectOpaque]
		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			Graphics.Blit(source, destination, this.mat);
		}

		// Token: 0x0400217B RID: 8571
		public Material mat;
	}
}
