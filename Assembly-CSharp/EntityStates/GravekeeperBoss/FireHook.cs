using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.GravekeeperBoss
{
	// Token: 0x0200084C RID: 2124
	public class FireHook : BaseState
	{
		// Token: 0x0600300A RID: 12298 RVA: 0x000CE150 File Offset: 0x000CC350
		public override void OnEnter()
		{
			base.OnEnter();
			base.fixedAge = 0f;
			this.duration = FireHook.baseDuration / this.attackSpeedStat;
			this.modelAnimator = base.GetModelAnimator();
			if (this.modelAnimator)
			{
				base.PlayCrossfade("Body", "FireHook", "FireHook.playbackRate", this.duration, 0.03f);
			}
			ChildLocator component = this.modelAnimator.GetComponent<ChildLocator>();
			if (component)
			{
				component.FindChild(FireHook.muzzleString);
			}
			Util.PlayScaledSound(FireHook.soundString, base.gameObject, this.attackSpeedStat);
			EffectManager.SimpleMuzzleFlash(FireHook.muzzleflashEffectPrefab, base.gameObject, FireHook.muzzleString, false);
			Ray aimRay = base.GetAimRay();
			if (NetworkServer.active)
			{
				this.FireSingleHook(aimRay, 0f, 0f);
				for (int i = 0; i < FireHook.projectileCount; i++)
				{
					float bonusPitch = UnityEngine.Random.Range(-FireHook.spread, FireHook.spread) / 2f;
					float bonusYaw = UnityEngine.Random.Range(-FireHook.spread, FireHook.spread) / 2f;
					this.FireSingleHook(aimRay, bonusPitch, bonusYaw);
				}
			}
		}

		// Token: 0x0600300B RID: 12299 RVA: 0x000CE270 File Offset: 0x000CC470
		private void FireSingleHook(Ray aimRay, float bonusPitch, float bonusYaw)
		{
			Vector3 forward = Util.ApplySpread(aimRay.direction, 0f, 0f, 1f, 1f, bonusYaw, bonusPitch);
			ProjectileManager.instance.FireProjectile(FireHook.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(forward), base.gameObject, this.damageStat * FireHook.projectileDamageCoefficient, FireHook.projectileForce, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
		}

		// Token: 0x0600300C RID: 12300 RVA: 0x000CE2F0 File Offset: 0x000CC4F0
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x0600300D RID: 12301 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04002DF2 RID: 11762
		public static float baseDuration = 3f;

		// Token: 0x04002DF3 RID: 11763
		public static string soundString;

		// Token: 0x04002DF4 RID: 11764
		public static string muzzleString;

		// Token: 0x04002DF5 RID: 11765
		public static float projectileDamageCoefficient;

		// Token: 0x04002DF6 RID: 11766
		public static GameObject muzzleflashEffectPrefab;

		// Token: 0x04002DF7 RID: 11767
		public static GameObject projectilePrefab;

		// Token: 0x04002DF8 RID: 11768
		public static float spread;

		// Token: 0x04002DF9 RID: 11769
		public static int projectileCount;

		// Token: 0x04002DFA RID: 11770
		public static float projectileForce;

		// Token: 0x04002DFB RID: 11771
		private float duration;

		// Token: 0x04002DFC RID: 11772
		private Animator modelAnimator;
	}
}
