using System;
using RoR2;

namespace EntityStates.Croco
{
	// Token: 0x020008AB RID: 2219
	public class ChainableLeap : BaseLeap
	{
		// Token: 0x17000460 RID: 1120
		// (get) Token: 0x060031BC RID: 12732 RVA: 0x000D6615 File Offset: 0x000D4815
		protected override DamageType blastDamageType
		{
			get
			{
				return DamageType.Stun1s;
			}
		}

		// Token: 0x060031BD RID: 12733 RVA: 0x000D661C File Offset: 0x000D481C
		protected override void DoImpactAuthority()
		{
			base.DoImpactAuthority();
			BlastAttack.Result result = base.DetonateAuthority();
			base.skillLocator.utility.RunRecharge((float)result.hitCount * ChainableLeap.refundPerHit);
		}

		// Token: 0x0400304D RID: 12365
		public static float refundPerHit;
	}
}
