using System;

namespace EntityStates.Merc
{
	// Token: 0x020007C9 RID: 1993
	public class WhirlwindAir : WhirlwindBase
	{
		// Token: 0x06002D7C RID: 11644 RVA: 0x000C0F55 File Offset: 0x000BF155
		protected override void PlayAnim()
		{
			base.PlayCrossfade("FullBody, Override", "WhirlwindAir", "Whirlwind.playbackRate", this.duration, 0.1f);
		}

		// Token: 0x06002D7D RID: 11645 RVA: 0x000C0F77 File Offset: 0x000BF177
		public override void OnExit()
		{
			base.OnExit();
			base.PlayAnimation("FullBody, Override", "WhirlwindAirExit");
		}
	}
}
