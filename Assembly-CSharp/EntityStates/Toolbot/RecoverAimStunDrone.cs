using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Toolbot
{
	// Token: 0x020000DA RID: 218
	public class RecoverAimStunDrone : BaseState
	{
		// Token: 0x06000449 RID: 1097 RVA: 0x00011C60 File Offset: 0x0000FE60
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = RecoverAimStunDrone.baseDuration / this.attackSpeedStat;
			base.PlayAnimation("Gesture, Additive", "FireBomb", "FireBomb.playbackRate", this.duration);
			Util.PlaySound(RecoverAimStunDrone.fireSoundString, base.gameObject);
			if (RecoverAimStunDrone.muzzleEffectPrefab)
			{
				EffectManager.instance.SimpleMuzzleFlash(RecoverAimStunDrone.muzzleEffectPrefab, base.gameObject, "MuzzleNailgun", false);
			}
		}

		// Token: 0x0600044A RID: 1098 RVA: 0x00011CD8 File Offset: 0x0000FED8
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority && base.fixedAge >= this.duration)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x0600044B RID: 1099 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04000407 RID: 1031
		public static GameObject muzzleEffectPrefab;

		// Token: 0x04000408 RID: 1032
		public static string fireSoundString;

		// Token: 0x04000409 RID: 1033
		public static float baseDuration;

		// Token: 0x0400040A RID: 1034
		private float duration;
	}
}
