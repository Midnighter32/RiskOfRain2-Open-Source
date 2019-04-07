using System;
using RoR2;

namespace EntityStates.SurvivorPod
{
	// Token: 0x020000EE RID: 238
	internal class Landed : SurvivorPodBaseState
	{
		// Token: 0x06000494 RID: 1172 RVA: 0x00013218 File Offset: 0x00011418
		public override void OnEnter()
		{
			base.OnEnter();
			base.PlayAnimation("Base", "Idle");
			Util.PlaySound("Play_UI_podSteamLoop", base.gameObject);
			if (base.survivorPodController && base.survivorPodController.characterBodyObject)
			{
				this.survivorInteractionDriver = base.survivorPodController.characterBodyObject.GetComponent<InteractionDriver>();
			}
			if (this.survivorInteractionDriver)
			{
				this.survivorInteractionDriver.interactableOverride = base.survivorPodController.gameObject;
			}
		}

		// Token: 0x06000495 RID: 1173 RVA: 0x000132A4 File Offset: 0x000114A4
		public override void OnExit()
		{
			Util.PlaySound("Stop_UI_podSteamLoop", base.gameObject);
			if (this.survivorInteractionDriver)
			{
				this.survivorInteractionDriver.interactableOverride = null;
			}
		}

		// Token: 0x04000459 RID: 1113
		private InteractionDriver survivorInteractionDriver;
	}
}
