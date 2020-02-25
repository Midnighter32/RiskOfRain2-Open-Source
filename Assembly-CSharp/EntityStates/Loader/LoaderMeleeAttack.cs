using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Loader
{
	// Token: 0x020007E7 RID: 2023
	public class LoaderMeleeAttack : BasicMeleeAttack
	{
		// Token: 0x06002E07 RID: 11783 RVA: 0x000C3EE0 File Offset: 0x000C20E0
		protected override void AuthorityModifyOverlapAttack(OverlapAttack overlapAttack)
		{
			base.AuthorityModifyOverlapAttack(overlapAttack);
			if (base.HasBuff(BuffIndex.LoaderOvercharged))
			{
				overlapAttack.damage *= 2f;
				overlapAttack.hitEffectPrefab = LoaderMeleeAttack.overchargeImpactEffectPrefab;
				overlapAttack.damageType |= DamageType.Stun1s;
			}
		}

		// Token: 0x06002E08 RID: 11784 RVA: 0x000C3F1F File Offset: 0x000C211F
		protected override void OnMeleeHitAuthority()
		{
			base.OnMeleeHitAuthority();
			base.healthComponent.AddBarrierAuthority(LoaderMeleeAttack.barrierPercentagePerHit * base.healthComponent.fullBarrier);
		}

		// Token: 0x04002B18 RID: 11032
		public static GameObject overchargeImpactEffectPrefab;

		// Token: 0x04002B19 RID: 11033
		public static float barrierPercentagePerHit;
	}
}
