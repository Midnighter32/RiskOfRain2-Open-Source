using System;

namespace EntityStates.Merc
{
	// Token: 0x0200010B RID: 267
	public class WhirlwindAir : WhirlwindBase
	{
		// Token: 0x06000527 RID: 1319 RVA: 0x00016C0A File Offset: 0x00014E0A
		protected override void PlayAnim()
		{
			base.PlayCrossfade("FullBody, Override", "WhirlwindAir", "Whirlwind.playbackRate", this.duration, 0.1f);
		}

		// Token: 0x06000528 RID: 1320 RVA: 0x00016C2C File Offset: 0x00014E2C
		public override void OnExit()
		{
			base.OnExit();
			base.PlayAnimation("FullBody, Override", "WhirlwindAirExit");
		}
	}
}
