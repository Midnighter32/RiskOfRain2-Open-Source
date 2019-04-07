using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000383 RID: 899
	public class ParticleSystemRandomColor : MonoBehaviour
	{
		// Token: 0x060012B9 RID: 4793 RVA: 0x0005BD44 File Offset: 0x00059F44
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

		// Token: 0x060012BA RID: 4794 RVA: 0x0005BDA4 File Offset: 0x00059FA4
		[AssetCheck(typeof(ParticleSystemRandomColor))]
		private static void CheckParticleSystemRandomColor(ProjectIssueChecker projectIssueChecker, UnityEngine.Object asset)
		{
			ParticleSystemRandomColor particleSystemRandomColor = (ParticleSystemRandomColor)asset;
			for (int i = 0; i < particleSystemRandomColor.particleSystems.Length; i++)
			{
				if (!particleSystemRandomColor.particleSystems[i])
				{
					projectIssueChecker.LogErrorFormat(asset, "Null particle system in slot {0}", new object[]
					{
						i
					});
				}
			}
		}

		// Token: 0x04001689 RID: 5769
		public Color[] colors;

		// Token: 0x0400168A RID: 5770
		public ParticleSystem[] particleSystems;
	}
}
