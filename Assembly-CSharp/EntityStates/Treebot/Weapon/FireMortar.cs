using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Treebot.Weapon
{
	// Token: 0x02000751 RID: 1873
	public class FireMortar : BaseState
	{
		// Token: 0x06002B60 RID: 11104 RVA: 0x000B6C90 File Offset: 0x000B4E90
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FireMortar.baseDuration / this.attackSpeedStat;
			base.PlayAnimation("Gesture, Additive", "FireBomb", "FireBomb.playbackRate", this.duration);
			Util.PlaySound(FireMortar.fireSoundString, base.gameObject);
			if (FireMortar.muzzleEffectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(FireMortar.muzzleEffectPrefab, base.gameObject, "MuzzleNailgun", false);
			}
		}

		// Token: 0x06002B61 RID: 11105 RVA: 0x000B6D03 File Offset: 0x000B4F03
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority && base.fixedAge >= this.duration)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x06002B62 RID: 11106 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04002755 RID: 10069
		public static GameObject muzzleEffectPrefab;

		// Token: 0x04002756 RID: 10070
		public static string fireSoundString;

		// Token: 0x04002757 RID: 10071
		public static float baseDuration;

		// Token: 0x04002758 RID: 10072
		private float duration;
	}
}
