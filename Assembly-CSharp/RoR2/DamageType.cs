using System;

namespace RoR2
{
	// Token: 0x02000115 RID: 277
	[Flags]
	public enum DamageType : uint
	{
		// Token: 0x04000517 RID: 1303
		Generic = 0U,
		// Token: 0x04000518 RID: 1304
		NonLethal = 1U,
		// Token: 0x04000519 RID: 1305
		BypassArmor = 2U,
		// Token: 0x0400051A RID: 1306
		ResetCooldownsOnKill = 4U,
		// Token: 0x0400051B RID: 1307
		SlowOnHit = 8U,
		// Token: 0x0400051C RID: 1308
		WeakPointHit = 16U,
		// Token: 0x0400051D RID: 1309
		Stun1s = 32U,
		// Token: 0x0400051E RID: 1310
		BarrierBlocked = 64U,
		// Token: 0x0400051F RID: 1311
		IgniteOnHit = 128U,
		// Token: 0x04000520 RID: 1312
		Freeze2s = 256U,
		// Token: 0x04000521 RID: 1313
		ClayGoo = 512U,
		// Token: 0x04000522 RID: 1314
		BleedOnHit = 1024U,
		// Token: 0x04000523 RID: 1315
		Silent = 2048U,
		// Token: 0x04000524 RID: 1316
		PoisonOnHit = 4096U,
		// Token: 0x04000525 RID: 1317
		PercentIgniteOnHit = 8192U,
		// Token: 0x04000526 RID: 1318
		WeakOnHit = 16384U,
		// Token: 0x04000527 RID: 1319
		Nullify = 32768U,
		// Token: 0x04000528 RID: 1320
		VoidDeath = 65536U,
		// Token: 0x04000529 RID: 1321
		AOE = 131072U
	}
}
