using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Toolbot
{
	// Token: 0x0200075E RID: 1886
	public class RecoverAimStunDrone : BaseState
	{
		// Token: 0x06002B97 RID: 11159 RVA: 0x000B8094 File Offset: 0x000B6294
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = RecoverAimStunDrone.baseDuration / this.attackSpeedStat;
			base.PlayAnimation("Gesture, Additive", "FireBomb", "FireBomb.playbackRate", this.duration);
			Util.PlaySound(RecoverAimStunDrone.fireSoundString, base.gameObject);
			if (RecoverAimStunDrone.muzzleEffectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(RecoverAimStunDrone.muzzleEffectPrefab, base.gameObject, "MuzzleNailgun", false);
			}
		}

		// Token: 0x06002B98 RID: 11160 RVA: 0x000B8107 File Offset: 0x000B6307
		public override void OnExit()
		{
			base.OnExit();
			base.PlayCrossfade("Stance, Override", "Empty", 0.1f);
		}

		// Token: 0x06002B99 RID: 11161 RVA: 0x000B8124 File Offset: 0x000B6324
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority && base.fixedAge >= this.duration)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x06002B9A RID: 11162 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x040027AD RID: 10157
		public static GameObject muzzleEffectPrefab;

		// Token: 0x040027AE RID: 10158
		public static string fireSoundString;

		// Token: 0x040027AF RID: 10159
		public static float baseDuration;

		// Token: 0x040027B0 RID: 10160
		private float duration;
	}
}
