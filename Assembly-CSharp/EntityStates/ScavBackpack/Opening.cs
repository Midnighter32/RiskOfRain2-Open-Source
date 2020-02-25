using System;
using EntityStates.Barrel;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.ScavBackpack
{
	// Token: 0x0200078A RID: 1930
	public class Opening : EntityState
	{
		// Token: 0x06002C51 RID: 11345 RVA: 0x000BB15C File Offset: 0x000B935C
		public override void OnEnter()
		{
			base.OnEnter();
			base.PlayAnimation("Body", "Opening");
			this.timeBetweenDrops = Opening.duration / (float)Opening.maxItemDropCount;
			this.chestBehavior = base.GetComponent<ChestBehavior>();
			if (base.sfxLocator)
			{
				Util.PlaySound(base.sfxLocator.openSound, base.gameObject);
			}
		}

		// Token: 0x06002C52 RID: 11346 RVA: 0x000BB1C4 File Offset: 0x000B93C4
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (NetworkServer.active)
			{
				this.itemDropAge += Time.fixedDeltaTime;
				if (this.itemDropCount < (float)Opening.maxItemDropCount && this.itemDropAge > this.timeBetweenDrops)
				{
					this.itemDropCount += 1f;
					this.itemDropAge -= this.timeBetweenDrops;
					this.chestBehavior.RollItem();
					this.chestBehavior.ItemDrop();
				}
				if (base.fixedAge >= Opening.duration)
				{
					this.outer.SetNextState(new Opened());
					return;
				}
			}
		}

		// Token: 0x04002858 RID: 10328
		public static float duration = 1f;

		// Token: 0x04002859 RID: 10329
		public static int maxItemDropCount;

		// Token: 0x0400285A RID: 10330
		private ChestBehavior chestBehavior;

		// Token: 0x0400285B RID: 10331
		private float itemDropCount;

		// Token: 0x0400285C RID: 10332
		private float timeBetweenDrops;

		// Token: 0x0400285D RID: 10333
		private float itemDropAge;
	}
}
