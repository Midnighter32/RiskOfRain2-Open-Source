using System;

namespace RoR2
{
	// Token: 0x02000231 RID: 561
	[Flags]
	public enum DamageType : ushort
	{
		// Token: 0x04000E56 RID: 3670
		Generic = 0,
		// Token: 0x04000E57 RID: 3671
		NonLethal = 1,
		// Token: 0x04000E58 RID: 3672
		BypassArmor = 2,
		// Token: 0x04000E59 RID: 3673
		ResetCooldownsOnKill = 4,
		// Token: 0x04000E5A RID: 3674
		SlowOnHit = 8,
		// Token: 0x04000E5B RID: 3675
		WeakPointHit = 16,
		// Token: 0x04000E5C RID: 3676
		Stun1s = 32,
		// Token: 0x04000E5D RID: 3677
		BarrierBlocked = 64,
		// Token: 0x04000E5E RID: 3678
		IgniteOnHit = 128,
		// Token: 0x04000E5F RID: 3679
		Freeze2s = 256,
		// Token: 0x04000E60 RID: 3680
		ClayGoo = 512,
		// Token: 0x04000E61 RID: 3681
		BleedOnHit = 1024,
		// Token: 0x04000E62 RID: 3682
		Silent = 2048
	}
}
