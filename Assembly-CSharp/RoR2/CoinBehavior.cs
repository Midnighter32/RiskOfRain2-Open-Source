using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200029F RID: 671
	[RequireComponent(typeof(EffectComponent))]
	public class CoinBehavior : MonoBehaviour
	{
		// Token: 0x06000DB1 RID: 3505 RVA: 0x00043364 File Offset: 0x00041564
		private void Start()
		{
			this.originalCoinCount = (int)base.GetComponent<EffectComponent>().effectData.genericFloat;
			int i = this.originalCoinCount;
			for (int j = 0; j < this.coinTiers.Length; j++)
			{
				CoinBehavior.CoinTier coinTier = this.coinTiers[j];
				int num = 0;
				while (i >= coinTier.valuePerCoin)
				{
					i -= coinTier.valuePerCoin;
					num++;
				}
				if (num > 0)
				{
					ParticleSystem.EmissionModule emission = coinTier.particleSystem.emission;
					emission.enabled = true;
					emission.SetBursts(new ParticleSystem.Burst[]
					{
						new ParticleSystem.Burst(0f, (float)num)
					});
					coinTier.particleSystem.gameObject.SetActive(true);
				}
			}
		}

		// Token: 0x040011A5 RID: 4517
		public int originalCoinCount;

		// Token: 0x040011A6 RID: 4518
		public CoinBehavior.CoinTier[] coinTiers;

		// Token: 0x020002A0 RID: 672
		[Serializable]
		public struct CoinTier
		{
			// Token: 0x040011A7 RID: 4519
			public ParticleSystem particleSystem;

			// Token: 0x040011A8 RID: 4520
			public int valuePerCoin;
		}
	}
}
