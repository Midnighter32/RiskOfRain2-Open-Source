using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.HAND.Weapon
{
	// Token: 0x02000167 RID: 359
	public class Slam : BaseState
	{
		// Token: 0x060006F8 RID: 1784 RVA: 0x00021494 File Offset: 0x0001F694
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = Slam.baseDuration / this.attackSpeedStat;
			this.modelAnimator = base.GetModelAnimator();
			Transform modelTransform = base.GetModelTransform();
			this.attack = new OverlapAttack();
			this.attack.attacker = base.gameObject;
			this.attack.inflictor = base.gameObject;
			this.attack.teamIndex = TeamComponent.GetObjectTeam(this.attack.attacker);
			this.attack.damage = Slam.impactDamageCoefficient * this.damageStat;
			this.attack.hitEffectPrefab = Slam.hitEffectPrefab;
			this.attack.isCrit = Util.CheckRoll(this.critStat, base.characterBody.master);
			if (modelTransform)
			{
				this.attack.hitBoxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "Hammer");
				ChildLocator component = modelTransform.GetComponent<ChildLocator>();
				if (component)
				{
					this.hammerChildTransform = component.FindChild("SwingCenter");
				}
			}
			if (this.modelAnimator)
			{
				base.PlayAnimation("Gesture", "Slam", "Slam.playbackRate", this.duration);
			}
			if (base.characterBody)
			{
				base.characterBody.SetAimTimer(2f);
			}
		}

		// Token: 0x060006F9 RID: 1785 RVA: 0x00021600 File Offset: 0x0001F800
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (NetworkServer.active && this.modelAnimator && this.modelAnimator.GetFloat("Hammer.hitBoxActive") > 0.5f)
			{
				if (!this.hasSwung)
				{
					Ray aimRay = base.GetAimRay();
					EffectManager.instance.SimpleMuzzleFlash(Slam.swingEffectPrefab, base.gameObject, "SwingCenter", true);
					ProjectileManager.instance.FireProjectile(Slam.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * Slam.earthquakeDamageCoefficient, Slam.forceMagnitude, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
					this.hasSwung = true;
				}
				this.attack.forceVector = this.hammerChildTransform.right;
				this.attack.Fire(null);
			}
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x060006FA RID: 1786 RVA: 0x0000B306 File Offset: 0x00009506
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x0400088B RID: 2187
		public static float baseDuration = 3.5f;

		// Token: 0x0400088C RID: 2188
		public static float returnToIdlePercentage;

		// Token: 0x0400088D RID: 2189
		public static float impactDamageCoefficient = 2f;

		// Token: 0x0400088E RID: 2190
		public static float earthquakeDamageCoefficient = 2f;

		// Token: 0x0400088F RID: 2191
		public static float forceMagnitude = 16f;

		// Token: 0x04000890 RID: 2192
		public static float radius = 3f;

		// Token: 0x04000891 RID: 2193
		public static GameObject hitEffectPrefab;

		// Token: 0x04000892 RID: 2194
		public static GameObject swingEffectPrefab;

		// Token: 0x04000893 RID: 2195
		public static GameObject projectilePrefab;

		// Token: 0x04000894 RID: 2196
		private Transform hammerChildTransform;

		// Token: 0x04000895 RID: 2197
		private OverlapAttack attack;

		// Token: 0x04000896 RID: 2198
		private Animator modelAnimator;

		// Token: 0x04000897 RID: 2199
		private float duration;

		// Token: 0x04000898 RID: 2200
		private bool hasSwung;
	}
}
