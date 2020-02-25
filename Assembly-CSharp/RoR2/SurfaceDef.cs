using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000112 RID: 274
	[CreateAssetMenu(menuName = "RoR2/SurfaceDef")]
	public class SurfaceDef : ScriptableObject
	{
		// Token: 0x06000517 RID: 1303 RVA: 0x000144BE File Offset: 0x000126BE
		private void OnValidate()
		{
			if (!Application.isPlaying && this.surfaceDefIndex != SurfaceDefIndex.Invalid)
			{
				this.surfaceDefIndex = SurfaceDefIndex.Invalid;
			}
		}

		// Token: 0x04000508 RID: 1288
		[HideInInspector]
		public SurfaceDefIndex surfaceDefIndex = SurfaceDefIndex.Invalid;

		// Token: 0x04000509 RID: 1289
		public Color approximateColor;

		// Token: 0x0400050A RID: 1290
		public GameObject impactEffectPrefab;

		// Token: 0x0400050B RID: 1291
		public GameObject footstepEffectPrefab;

		// Token: 0x0400050C RID: 1292
		public string impactSoundString;

		// Token: 0x0400050D RID: 1293
		public string materialSwitchString;
	}
}
