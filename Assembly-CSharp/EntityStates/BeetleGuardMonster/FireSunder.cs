using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.BeetleGuardMonster
{
	// Token: 0x020008F3 RID: 2291
	public class FireSunder : BaseState
	{
		// Token: 0x06003336 RID: 13110 RVA: 0x000DE1F4 File Offset: 0x000DC3F4
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

		// Token: 0x06003337 RID: 13111 RVA: 0x000DE308 File Offset: 0x000DC508
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

		// Token: 0x06003338 RID: 13112 RVA: 0x000DE350 File Offset: 0x000DC550
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

		// Token: 0x06003339 RID: 13113 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x040032AB RID: 12971
		public static float baseDuration = 3.5f;

		// Token: 0x040032AC RID: 12972
		public static float damageCoefficient = 4f;

		// Token: 0x040032AD RID: 12973
		public static float forceMagnitude = 16f;

		// Token: 0x040032AE RID: 12974
		public static string initialAttackSoundString;

		// Token: 0x040032AF RID: 12975
		public static GameObject chargeEffectPrefab;

		// Token: 0x040032B0 RID: 12976
		public static GameObject projectilePrefab;

		// Token: 0x040032B1 RID: 12977
		public static GameObject hitEffectPrefab;

		// Token: 0x040032B2 RID: 12978
		private Animator modelAnimator;

		// Token: 0x040032B3 RID: 12979
		private Transform modelTransform;

		// Token: 0x040032B4 RID: 12980
		private bool hasAttacked;

		// Token: 0x040032B5 RID: 12981
		private float duration;

		// Token: 0x040032B6 RID: 12982
		private GameObject rightHandChargeEffect;

		// Token: 0x040032B7 RID: 12983
		private ChildLocator modelChildLocator;

		// Token: 0x040032B8 RID: 12984
		private Transform handRTransform;
	}
}
