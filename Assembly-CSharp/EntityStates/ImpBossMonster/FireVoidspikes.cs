using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.ImpBossMonster
{
	// Token: 0x0200081B RID: 2075
	public class FireVoidspikes : BaseState
	{
		// Token: 0x06002F09 RID: 12041 RVA: 0x000C8908 File Offset: 0x000C6B08
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FireVoidspikes.baseDuration / this.attackSpeedStat;
			this.modelAnimator = base.GetModelAnimator();
			this.modelTransform = base.GetModelTransform();
			base.characterMotor.walkSpeedPenaltyCoefficient = FireVoidspikes.walkSpeedPenaltyCoefficient;
			this.attack = new OverlapAttack();
			this.attack.attacker = base.gameObject;
			this.attack.inflictor = base.gameObject;
			this.attack.teamIndex = TeamComponent.GetObjectTeam(this.attack.attacker);
			this.attack.damage = FireVoidspikes.damageCoefficient * this.damageStat;
			this.attack.hitEffectPrefab = FireVoidspikes.hitEffectPrefab;
			this.attack.isCrit = Util.CheckRoll(this.critStat, base.characterBody.master);
			this.attack.procCoefficient = FireVoidspikes.procCoefficient;
			this.attack.damageType = DamageType.BleedOnHit;
			Util.PlaySound(FireVoidspikes.enterSoundString, base.gameObject);
			if (this.modelAnimator)
			{
				if (Util.CheckRoll(50f, 0f, null))
				{
					base.PlayAnimation("Gesture, Additive", "FireVoidspikesR", "FireVoidspikes.playbackRate", this.duration);
					base.PlayAnimation("Gesture, Override", "FireVoidspikesR", "FireVoidspikes.playbackRate", this.duration);
				}
				else
				{
					base.PlayAnimation("Gesture, Additive", "FireVoidspikesL", "FireVoidspikes.playbackRate", this.duration);
					base.PlayAnimation("Gesture, Override", "FireVoidspikesL", "FireVoidspikes.playbackRate", this.duration);
				}
			}
			if (base.characterBody)
			{
				base.characterBody.SetAimTimer(this.duration + 3f);
			}
		}

		// Token: 0x06002F0A RID: 12042 RVA: 0x000C8ACA File Offset: 0x000C6CCA
		public override void OnExit()
		{
			base.characterMotor.walkSpeedPenaltyCoefficient = 1f;
			base.OnExit();
		}

		// Token: 0x06002F0B RID: 12043 RVA: 0x000C8AE4 File Offset: 0x000C6CE4
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (NetworkServer.active && this.modelAnimator && this.slashCount <= 0)
			{
				if (this.modelAnimator.GetFloat("HandR.hitBoxActive") > 0.1f)
				{
					EffectManager.SimpleMuzzleFlash(FireVoidspikes.swipeEffectPrefab, base.gameObject, "FireVoidspikesR", true);
					this.slashCount++;
					if (this.modelTransform)
					{
						this.attack.hitBoxGroup = Array.Find<HitBoxGroup>(this.modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "HandR");
					}
					base.characterBody.healthComponent.TakeDamageForce(base.characterDirection.forward * FireVoidspikes.selfForce, true, false);
					this.attack.forceVector = base.characterDirection.forward * FireVoidspikes.forceMagnitude;
					this.attack.Fire(null);
					Util.PlaySound(FireVoidspikes.attackSoundString, base.gameObject);
					Ray aimRay = base.GetAimRay();
					for (int i = 0; i < FireVoidspikes.projectileCount; i++)
					{
						this.FireSpike(aimRay, 0f, ((float)FireVoidspikes.projectileCount / 2f - (float)i) * FireVoidspikes.projectileYawSpread);
					}
				}
				if (this.modelAnimator.GetFloat("HandL.hitBoxActive") > 0.1f)
				{
					EffectManager.SimpleMuzzleFlash(FireVoidspikes.swipeEffectPrefab, base.gameObject, "FireVoidspikesL", true);
					this.slashCount++;
					if (this.modelTransform)
					{
						this.attack.hitBoxGroup = Array.Find<HitBoxGroup>(this.modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "HandL");
					}
					base.characterBody.healthComponent.TakeDamageForce(base.characterDirection.forward * FireVoidspikes.selfForce, true, false);
					this.attack.forceVector = base.characterDirection.forward * FireVoidspikes.forceMagnitude;
					this.attack.Fire(null);
					Util.PlaySound(FireVoidspikes.attackSoundString, base.gameObject);
					Ray aimRay2 = base.GetAimRay();
					for (int j = 0; j < FireVoidspikes.projectileCount; j++)
					{
						this.FireSpike(aimRay2, 0f, ((float)FireVoidspikes.projectileCount / 2f - (float)j) * FireVoidspikes.projectileYawSpread);
					}
				}
			}
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06002F0C RID: 12044 RVA: 0x000C8D88 File Offset: 0x000C6F88
		private void FireSpike(Ray aimRay, float bonusPitch, float bonusYaw)
		{
			Vector3 forward = Util.ApplySpread(aimRay.direction, 0f, 0f, 1f, 1f, bonusYaw, bonusPitch);
			ProjectileManager.instance.FireProjectile(FireVoidspikes.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(forward), base.gameObject, this.damageStat * FireVoidspikes.projectileDamageCoefficient, 0f, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
		}

		// Token: 0x06002F0D RID: 12045 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04002C66 RID: 11366
		public static float baseDuration = 3.5f;

		// Token: 0x04002C67 RID: 11367
		public static float damageCoefficient = 4f;

		// Token: 0x04002C68 RID: 11368
		public static float procCoefficient;

		// Token: 0x04002C69 RID: 11369
		public static float selfForce;

		// Token: 0x04002C6A RID: 11370
		public static float forceMagnitude = 16f;

		// Token: 0x04002C6B RID: 11371
		public static GameObject hitEffectPrefab;

		// Token: 0x04002C6C RID: 11372
		public static GameObject swipeEffectPrefab;

		// Token: 0x04002C6D RID: 11373
		public static string enterSoundString;

		// Token: 0x04002C6E RID: 11374
		public static string attackSoundString;

		// Token: 0x04002C6F RID: 11375
		public static float walkSpeedPenaltyCoefficient;

		// Token: 0x04002C70 RID: 11376
		public static int projectileCount;

		// Token: 0x04002C71 RID: 11377
		public static float projectileYawSpread;

		// Token: 0x04002C72 RID: 11378
		public static float projectileDamageCoefficient;

		// Token: 0x04002C73 RID: 11379
		public static GameObject projectilePrefab;

		// Token: 0x04002C74 RID: 11380
		private OverlapAttack attack;

		// Token: 0x04002C75 RID: 11381
		private Animator modelAnimator;

		// Token: 0x04002C76 RID: 11382
		private float duration;

		// Token: 0x04002C77 RID: 11383
		private int slashCount;

		// Token: 0x04002C78 RID: 11384
		private Transform modelTransform;
	}
}
