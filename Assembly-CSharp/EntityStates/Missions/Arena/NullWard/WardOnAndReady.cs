using System;
using RoR2;

namespace EntityStates.Missions.Arena.NullWard
{
	// Token: 0x020007BD RID: 1981
	public class WardOnAndReady : NullWardBaseState
	{
		// Token: 0x06002D40 RID: 11584 RVA: 0x000BF048 File Offset: 0x000BD248
		public override void OnEnter()
		{
			base.OnEnter();
			this.buffWard.Networkradius = NullWardBaseState.wardWaitingRadius;
			this.purchaseInteraction.SetAvailable(true);
			this.childLocator.FindChild("WardOnEffect").gameObject.SetActive(true);
			this.buffWard.enabled = true;
			Util.PlaySound(WardOnAndReady.soundLoopStartEvent, base.gameObject);
		}

		// Token: 0x06002D41 RID: 11585 RVA: 0x000BF0AF File Offset: 0x000BD2AF
		public override void OnExit()
		{
			Util.PlaySound(WardOnAndReady.soundLoopEndEvent, base.gameObject);
			base.OnExit();
		}

		// Token: 0x04002981 RID: 10625
		public static string soundLoopStartEvent;

		// Token: 0x04002982 RID: 10626
		public static string soundLoopEndEvent;
	}
}
