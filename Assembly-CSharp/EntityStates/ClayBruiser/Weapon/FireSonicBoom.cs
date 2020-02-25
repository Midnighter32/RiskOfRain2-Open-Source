using System;
using EntityStates.Treebot.Weapon;
using RoR2;

namespace EntityStates.ClayBruiser.Weapon
{
	// Token: 0x020008CC RID: 2252
	public class FireSonicBoom : FireSonicBoom
	{
		// Token: 0x0600327D RID: 12925 RVA: 0x000DA649 File Offset: 0x000D8849
		public override void OnEnter()
		{
			base.OnEnter();
			base.GetModelAnimator().SetBool("WeaponIsReady", true);
		}

		// Token: 0x0600327E RID: 12926 RVA: 0x000DA662 File Offset: 0x000D8862
		protected override void AddDebuff(CharacterBody body)
		{
			body.AddTimedBuff(BuffIndex.ClayGoo, this.slowDuration);
		}

		// Token: 0x0600327F RID: 12927 RVA: 0x000DA672 File Offset: 0x000D8872
		public override void OnExit()
		{
			if (!this.outer.destroying)
			{
				base.GetModelAnimator().SetBool("WeaponIsReady", false);
			}
			base.OnExit();
		}
	}
}
