using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000118 RID: 280
	public class DamageInfo
	{
		// Token: 0x0600051E RID: 1310 RVA: 0x00014786 File Offset: 0x00012986
		public void ModifyDamageInfo(HurtBox.DamageModifier damageModifier)
		{
			switch (damageModifier)
			{
			case HurtBox.DamageModifier.Normal:
			case HurtBox.DamageModifier.SniperTarget:
				break;
			case HurtBox.DamageModifier.Weak:
				this.damageType |= DamageType.WeakPointHit;
				return;
			case HurtBox.DamageModifier.Barrier:
				this.damageType |= DamageType.BarrierBlocked;
				break;
			default:
				return;
			}
		}

		// Token: 0x04000534 RID: 1332
		public float damage;

		// Token: 0x04000535 RID: 1333
		public bool crit;

		// Token: 0x04000536 RID: 1334
		public GameObject inflictor;

		// Token: 0x04000537 RID: 1335
		public GameObject attacker;

		// Token: 0x04000538 RID: 1336
		public Vector3 position;

		// Token: 0x04000539 RID: 1337
		public Vector3 force;

		// Token: 0x0400053A RID: 1338
		public bool rejected;

		// Token: 0x0400053B RID: 1339
		public ProcChainMask procChainMask;

		// Token: 0x0400053C RID: 1340
		public float procCoefficient = 1f;

		// Token: 0x0400053D RID: 1341
		public DamageType damageType;

		// Token: 0x0400053E RID: 1342
		public DamageColorIndex damageColorIndex;

		// Token: 0x0400053F RID: 1343
		public DotController.DotIndex dotIndex = DotController.DotIndex.None;
	}
}
