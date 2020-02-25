using System;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.SurvivorPod
{
	// Token: 0x0200077D RID: 1917
	public class Release : SurvivorPodBaseState
	{
		// Token: 0x06002C17 RID: 11287 RVA: 0x000BA400 File Offset: 0x000B8600
		public override void OnEnter()
		{
			base.OnEnter();
			base.PlayAnimation("Base", "Release");
			Transform modelTransform = base.GetModelTransform();
			if (modelTransform)
			{
				ChildLocator component = modelTransform.GetComponent<ChildLocator>();
				component.FindChild("Door").gameObject.SetActive(false);
				component.FindChild("ReleaseExhaustFX").gameObject.SetActive(true);
			}
			if (!base.survivorPodController)
			{
				return;
			}
			if (NetworkServer.active && base.vehicleSeat && base.vehicleSeat.currentPassengerBody)
			{
				base.vehicleSeat.EjectPassenger(base.vehicleSeat.currentPassengerBody.gameObject);
			}
		}

		// Token: 0x06002C18 RID: 11288 RVA: 0x000BA4B4 File Offset: 0x000B86B4
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (NetworkServer.active && (!base.vehicleSeat || !base.vehicleSeat.currentPassengerBody))
			{
				this.outer.SetNextState(new ReleaseFinished());
			}
		}

		// Token: 0x04002826 RID: 10278
		public static float ejectionSpeed = 20f;
	}
}
