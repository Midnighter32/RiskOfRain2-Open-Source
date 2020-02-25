using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.HAND.Weapon
{
	// Token: 0x02000849 RID: 2121
	public class Slam : BaseState
	{
		// Token: 0x06002FFD RID: 12285 RVA: 0x000CDB9C File Offset: 0x000CBD9C
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

		// Token: 0x06002FFE RID: 12286 RVA: 0x000CDD08 File Offset: 0x000CBF08
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (NetworkServer.active && this.modelAnimator && this.modelAnimator.GetFloat("Hammer.hitBoxActive") > 0.5f)
			{
				if (!this.hasSwung)
				{
					Ray aimRay = base.GetAimRay();
					EffectManager.SimpleMuzzleFlash(Slam.swingEffectPrefab, base.gameObject, "SwingCenter", true);
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

		// Token: 0x06002FFF RID: 12287 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04002DD1 RID: 11729
		public static float baseDuration = 3.5f;

		// Token: 0x04002DD2 RID: 11730
		public static float returnToIdlePercentage;

		// Token: 0x04002DD3 RID: 11731
		public static float impactDamageCoefficient = 2f;

		// Token: 0x04002DD4 RID: 11732
		public static float earthquakeDamageCoefficient = 2f;

		// Token: 0x04002DD5 RID: 11733
		public static float forceMagnitude = 16f;

		// Token: 0x04002DD6 RID: 11734
		public static float radius = 3f;

		// Token: 0x04002DD7 RID: 11735
		public static GameObject hitEffectPrefab;

		// Token: 0x04002DD8 RID: 11736
		public static GameObject swingEffectPrefab;

		// Token: 0x04002DD9 RID: 11737
		public static GameObject projectilePrefab;

		// Token: 0x04002DDA RID: 11738
		private Transform hammerChildTransform;

		// Token: 0x04002DDB RID: 11739
		private OverlapAttack attack;

		// Token: 0x04002DDC RID: 11740
		private Animator modelAnimator;

		// Token: 0x04002DDD RID: 11741
		private float duration;

		// Token: 0x04002DDE RID: 11742
		private bool hasSwung;
	}
}
