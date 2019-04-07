using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.BeetleGuardMonster
{
	// Token: 0x020001D8 RID: 472
	public class FireSunder : BaseState
	{
		// Token: 0x06000936 RID: 2358 RVA: 0x0002E588 File Offset: 0x0002C788
		public override void OnEnter()
		{
			base.OnEnter();
			this.modelAnimator = base.GetModelAnimator();
			this.modelTransform = base.GetModelTransform();
			Util.PlaySound(FireSunder.initialAttackSoundString, base.gameObject);
			this.duration = FireSunder.baseDuration / this.attackSpeedStat;
			base.PlayCrossfade("Body", "FireSunder", "FireSunder.playbackRate", this.duration, 0.2f);
			if (base.characterBody)
			{
				base.characterBody.SetAimTimer(this.duration + 2f);
			}
			if (this.modelTransform)
			{
				AimAnimator component = this.modelTransform.GetComponent<AimAnimator>();
				if (component)
				{
					component.enabled = true;
				}
				this.modelChildLocator = this.modelTransform.GetComponent<ChildLocator>();
				if (this.modelChildLocator)
				{
					GameObject original = FireSunder.chargeEffectPrefab;
					this.handRTransform = this.modelChildLocator.FindChild("HandR");
					if (this.handRTransform)
					{
						this.rightHandChargeEffect = UnityEngine.Object.Instantiate<GameObject>(original, this.handRTransform);
					}
				}
			}
		}

		// Token: 0x06000937 RID: 2359 RVA: 0x0002E69C File Offset: 0x0002C89C
		public override void OnExit()
		{
			EntityState.Destroy(this.rightHandChargeEffect);
			if (this.modelTransform)
			{
				AimAnimator component = this.modelTransform.GetComponent<AimAnimator>();
				if (component)
				{
					component.enabled = true;
				}
			}
			base.OnExit();
		}

		// Token: 0x06000938 RID: 2360 RVA: 0x0002E6E4 File Offset: 0x0002C8E4
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (this.modelAnimator && this.modelAnimator.GetFloat("FireSunder.activate") > 0.5f && !this.hasAttacked)
			{
				if (base.isAuthority && this.modelTransform)
				{
					Ray aimRay = base.GetAimRay();
					aimRay.origin = this.handRTransform.position;
					ProjectileManager.instance.FireProjectile(FireSunder.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * FireSunder.damageCoefficient, FireSunder.forceMagnitude, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
				}
				this.hasAttacked = true;
				EntityState.Destroy(this.rightHandChargeEffect);
			}
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06000939 RID: 2361 RVA: 0x0000B306 File Offset: 0x00009506
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04000C83 RID: 3203
		public static float baseDuration = 3.5f;

		// Token: 0x04000C84 RID: 3204
		public static float damageCoefficient = 4f;

		// Token: 0x04000C85 RID: 3205
		public static float forceMagnitude = 16f;

		// Token: 0x04000C86 RID: 3206
		public static string initialAttackSoundString;

		// Token: 0x04000C87 RID: 3207
		public static GameObject chargeEffectPrefab;

		// Token: 0x04000C88 RID: 3208
		public static GameObject projectilePrefab;

		// Token: 0x04000C89 RID: 3209
		public static GameObject hitEffectPrefab;

		// Token: 0x04000C8A RID: 3210
		private Animator modelAnimator;

		// Token: 0x04000C8B RID: 3211
		private Transform modelTransform;

		// Token: 0x04000C8C RID: 3212
		private bool hasAttacked;

		// Token: 0x04000C8D RID: 3213
		private float duration;

		// Token: 0x04000C8E RID: 3214
		private GameObject rightHandChargeEffect;

		// Token: 0x04000C8F RID: 3215
		private ChildLocator modelChildLocator;

		// Token: 0x04000C90 RID: 3216
		private Transform handRTransform;
	}
}
