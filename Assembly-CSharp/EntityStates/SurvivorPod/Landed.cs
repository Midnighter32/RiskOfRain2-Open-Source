using System;
using RoR2;
using UnityEngine;

namespace EntityStates.SurvivorPod
{
	// Token: 0x0200077B RID: 1915
	public class Landed : SurvivorPodBaseState
	{
		// Token: 0x06002C0F RID: 11279 RVA: 0x000BA284 File Offset: 0x000B8484
		public override void OnEnter()
		{
			base.OnEnter();
			base.PlayAnimation("Base", "Idle");
			Util.PlaySound("Play_UI_podSteamLoop", base.gameObject);
			base.survivorPodController.exitAllowed = true;
			base.vehicleSeat.handleVehicleExitRequestServer.AddCallback(new CallbackCheck<bool, GameObject>.CallbackDelegate(this.HandleVehicleExitRequest));
		}

		// Token: 0x06002C10 RID: 11280 RVA: 0x000BA2E0 File Offset: 0x000B84E0
		private void HandleVehicleExitRequest(GameObject gameObject, ref bool? result)
		{
			base.survivorPodController.exitAllowed = false;
			this.outer.SetNextState(new PreRelease());
			result = new bool?(true);
		}

		// Token: 0x06002C11 RID: 11281 RVA: 0x000BA30A File Offset: 0x000B850A
		public override void OnExit()
		{
			base.vehicleSeat.handleVehicleExitRequestServer.RemoveCallback(new CallbackCheck<bool, GameObject>.CallbackDelegate(this.HandleVehicleExitRequest));
			base.survivorPodController.exitAllowed = false;
			Util.PlaySound("Stop_UI_podSteamLoop", base.gameObject);
		}
	}
}
