using System;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Missions.Arena.NullWard
{
	// Token: 0x020007C0 RID: 1984
	public class Complete : NullWardBaseState
	{
		// Token: 0x06002D4F RID: 11599 RVA: 0x000BF350 File Offset: 0x000BD550
		public override void OnEnter()
		{
			base.OnEnter();
			this.buffWard.Networkradius = NullWardBaseState.wardRadiusOn;
			this.purchaseInteraction.SetAvailable(false);
			this.childLocator.FindChild("CompleteEffect").gameObject.SetActive(true);
			if (NetworkServer.active)
			{
				base.arenaMissionController.EndRound();
			}
		}

		// Token: 0x06002D50 RID: 11600 RVA: 0x000BF3AC File Offset: 0x000BD5AC
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.buffWard.Networkradius = Mathf.Lerp(NullWardBaseState.wardRadiusOn, NullWardBaseState.wardRadiusOff, base.fixedAge / Complete.duration);
			if (base.fixedAge >= Complete.duration && base.isAuthority)
			{
				this.outer.SetNextState(new Off());
			}
		}

		// Token: 0x04002989 RID: 10633
		public static float duration;

		// Token: 0x0400298A RID: 10634
		public static string soundEntryEvent;
	}
}
