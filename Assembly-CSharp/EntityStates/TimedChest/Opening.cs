using System;
using RoR2;

namespace EntityStates.TimedChest
{
	// Token: 0x020000EC RID: 236
	public class Opening : BaseState
	{
		// Token: 0x14000002 RID: 2
		// (add) Token: 0x0600048B RID: 1163 RVA: 0x00012FB0 File Offset: 0x000111B0
		// (remove) Token: 0x0600048C RID: 1164 RVA: 0x00012FE4 File Offset: 0x000111E4
		public static event Action onOpened;

		// Token: 0x0600048D RID: 1165 RVA: 0x00013018 File Offset: 0x00011218
		public override void OnEnter()
		{
			base.OnEnter();
			base.PlayAnimation("Body", "Opening");
			TimedChestController component = base.GetComponent<TimedChestController>();
			if (component)
			{
				component.purchased = true;
			}
			if (base.sfxLocator)
			{
				Util.PlaySound(base.sfxLocator.openSound, base.gameObject);
			}
		}

		// Token: 0x0600048E RID: 1166 RVA: 0x00013078 File Offset: 0x00011278
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= Opening.delayUntilUnlockAchievement && !this.hasGrantedAchievement)
			{
				Action action = Opening.onOpened;
				if (action != null)
				{
					action();
				}
				this.hasGrantedAchievement = true;
				GenericPickupController componentInChildren = base.gameObject.GetComponentInChildren<GenericPickupController>();
				if (componentInChildren)
				{
					componentInChildren.enabled = true;
				}
			}
		}

		// Token: 0x04000456 RID: 1110
		public static float delayUntilUnlockAchievement;

		// Token: 0x04000457 RID: 1111
		private bool hasGrantedAchievement;
	}
}
