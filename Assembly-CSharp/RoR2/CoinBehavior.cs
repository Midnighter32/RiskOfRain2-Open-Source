using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020001AB RID: 427
	[RequireComponent(typeof(EffectComponent))]
	public class CoinBehavior : MonoBehaviour
	{
		// Token: 0x06000920 RID: 2336 RVA: 0x00027618 File Offset: 0x00025818
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

		// Token: 0x04000977 RID: 2423
		public int originalCoinCount;

		// Token: 0x04000978 RID: 2424
		public CoinBehavior.CoinTier[] coinTiers;

		// Token: 0x020001AC RID: 428
		[Serializable]
		public struct CoinTier
		{
			// Token: 0x04000979 RID: 2425
			public ParticleSystem particleSystem;

			// Token: 0x0400097A RID: 2426
			public int valuePerCoin;
		}
	}
}
