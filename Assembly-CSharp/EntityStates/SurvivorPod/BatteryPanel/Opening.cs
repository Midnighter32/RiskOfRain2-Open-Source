using System;
using RoR2;

namespace EntityStates.SurvivorPod.BatteryPanel
{
	// Token: 0x02000782 RID: 1922
	public class Opening : BaseBatteryPanelState
	{
		// Token: 0x06002C29 RID: 11305 RVA: 0x000BA700 File Offset: 0x000B8900
		public override void OnEnter()
		{
			base.OnEnter();
			base.PlayPodAnimation("Additive", "OpenPanel", "OpenPanel.playbackRate", Opening.duration);
			Util.PlaySound(Opening.openSoundString, base.gameObject);
		}

		// Token: 0x06002C2A RID: 11306 RVA: 0x000BA733 File Offset: 0x000B8933
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= Opening.duration && base.isAuthority)
			{
				this.outer.SetNextState(new Opened());
			}
		}

		// Token: 0x06002C2B RID: 11307 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x0400282C RID: 10284
		public static float duration;

		// Token: 0x0400282D RID: 10285
		public static string openSoundString;
	}
}
