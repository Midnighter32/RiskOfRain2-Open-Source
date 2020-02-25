using System;
using RoR2;

namespace EntityStates.TimedChest
{
	// Token: 0x02000774 RID: 1908
	public class Opening : BaseState
	{
		// Token: 0x1400008C RID: 140
		// (add) Token: 0x06002BEE RID: 11246 RVA: 0x000B9A38 File Offset: 0x000B7C38
		// (remove) Token: 0x06002BEF RID: 11247 RVA: 0x000B9A6C File Offset: 0x000B7C6C
		public static event Action onOpened;

		// Token: 0x06002BF0 RID: 11248 RVA: 0x000B9AA0 File Offset: 0x000B7CA0
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

		// Token: 0x06002BF1 RID: 11249 RVA: 0x000B9B00 File Offset: 0x000B7D00
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

		// Token: 0x0400280D RID: 10253
		public static float delayUntilUnlockAchievement;

		// Token: 0x0400280E RID: 10254
		private bool hasGrantedAchievement;
	}
}
