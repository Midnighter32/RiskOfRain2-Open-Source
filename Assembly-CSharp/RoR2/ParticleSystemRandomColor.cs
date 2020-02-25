using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002BB RID: 699
	public class ParticleSystemRandomColor : MonoBehaviour
	{
		// Token: 0x06000FC5 RID: 4037 RVA: 0x000453A8 File Offset: 0x000435A8
		private void Awake()
		{
			if (this.colors.Length != 0)
			{
				Color color = this.colors[UnityEngine.Random.Range(0, this.colors.Length)];
				for (int i = 0; i < this.particleSystems.Length; i++)
				{
					this.particleSystems[i].main.startColor = color;
				}
			}
		}

		// Token: 0x06000FC6 RID: 4038 RVA: 0x00045408 File Offset: 0x00043608
		[AssetCheck(typeof(ParticleSystemRandomColor))]
		private static void CheckParticleSystemRandomColor(AssetCheckArgs args)
		{
			ParticleSystemRandomColor particleSystemRandomColor = (ParticleSystemRandomColor)args.asset;
			for (int i = 0; i < particleSystemRandomColor.particleSystems.Length; i++)
			{
				if (!particleSystemRandomColor.particleSystems[i])
				{
					args.LogErrorFormat(args.asset, "Null particle system in slot {0}", new object[]
					{
						i
					});
				}
			}
		}

		// Token: 0x04000F46 RID: 3910
		public Color[] colors;

		// Token: 0x04000F47 RID: 3911
		public ParticleSystem[] particleSystems;
	}
}
