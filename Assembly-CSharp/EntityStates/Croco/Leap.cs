using System;
using RoR2;

namespace EntityStates.Croco
{
	// Token: 0x020008AA RID: 2218
	public class Leap : BaseLeap
	{
		// Token: 0x1700045F RID: 1119
		// (get) Token: 0x060031B9 RID: 12729 RVA: 0x000D65F1 File Offset: 0x000D47F1
		protected override DamageType blastDamageType
		{
			get
			{
				return DamageType.Stun1s | DamageType.PoisonOnHit;
			}
		}

		// Token: 0x060031BA RID: 12730 RVA: 0x000D65F8 File Offset: 0x000D47F8
		protected override void DoImpactAuthority()
		{
			base.DoImpactAuthority();
			base.DetonateAuthority();
			base.DropAcidPoolAuthority();
		}
	}
}
