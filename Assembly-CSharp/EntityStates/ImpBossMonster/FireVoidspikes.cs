using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.ImpBossMonster
{
	// Token: 0x0200013F RID: 319
	public class FireVoidspikes : BaseState
	{
		// Token: 0x0600061C RID: 1564 RVA: 0x0001C564 File Offset: 0x0001A764
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

		// Token: 0x0600061D RID: 1565 RVA: 0x0001C726 File Offset: 0x0001A926
		public override void OnExit()
		{
			base.characterMotor.walkSpeedPenaltyCoefficient = 1f;
			base.OnExit();
		}

		// Token: 0x0600061E RID: 1566 RVA: 0x0001C740 File Offset: 0x0001A940
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (NetworkServer.active && this.modelAnimator && this.slashCount <= 0)
			{
				if (this.modelAnimator.GetFloat("HandR.hitBoxActive") > 0.1f)
				{
					EffectManager.instance.SimpleMuzzleFlash(FireVoidspikes.swipeEffectPrefab, base.gameObject, "FireVoidspikesR", true);
					this.slashCount++;
					if (this.modelTransform)
					{
						this.attack.hitBoxGroup = Array.Find<HitBoxGroup>(this.modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "HandR");
					}
					base.characterBody.healthComponent.TakeDamageForce(base.characterDirection.forward * FireVoidspikes.selfForce, true);
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
					EffectManager.instance.SimpleMuzzleFlash(FireVoidspikes.swipeEffectPrefab, base.gameObject, "FireVoidspikesL", true);
					this.slashCount++;
					if (this.modelTransform)
					{
						this.attack.hitBoxGroup = Array.Find<HitBoxGroup>(this.modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "HandL");
					}
					base.characterBody.healthComponent.TakeDamageForce(base.characterDirection.forward * FireVoidspikes.selfForce, true);
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

		// Token: 0x0600061F RID: 1567 RVA: 0x0001C9EC File Offset: 0x0001ABEC
		private void FireSpike(Ray aimRay, float bonusPitch, float bonusYaw)
		{
			Vector3 forward = Util.ApplySpread(aimRay.direction, 0f, 0f, 1f, 1f, bonusYaw, bonusPitch);
			ProjectileManager.instance.FireProjectile(FireVoidspikes.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(forward), base.gameObject, this.damageStat * FireVoidspikes.projectileDamageCoefficient, 0f, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
		}

		// Token: 0x06000620 RID: 1568 RVA: 0x0000B306 File Offset: 0x00009506
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04000726 RID: 1830
		public static float baseDuration = 3.5f;

		// Token: 0x04000727 RID: 1831
		public static float damageCoefficient = 4f;

		// Token: 0x04000728 RID: 1832
		public static float procCoefficient;

		// Token: 0x04000729 RID: 1833
		public static float selfForce;

		// Token: 0x0400072A RID: 1834
		public static float forceMagnitude = 16f;

		// Token: 0x0400072B RID: 1835
		public static GameObject hitEffectPrefab;

		// Token: 0x0400072C RID: 1836
		public static GameObject swipeEffectPrefab;

		// Token: 0x0400072D RID: 1837
		public static string enterSoundString;

		// Token: 0x0400072E RID: 1838
		public static string attackSoundString;

		// Token: 0x0400072F RID: 1839
		public static float walkSpeedPenaltyCoefficient;

		// Token: 0x04000730 RID: 1840
		public static int projectileCount;

		// Token: 0x04000731 RID: 1841
		public static float projectileYawSpread;

		// Token: 0x04000732 RID: 1842
		public static float projectileDamageCoefficient;

		// Token: 0x04000733 RID: 1843
		public static GameObject projectilePrefab;

		// Token: 0x04000734 RID: 1844
		private OverlapAttack attack;

		// Token: 0x04000735 RID: 1845
		private Animator modelAnimator;

		// Token: 0x04000736 RID: 1846
		private float duration;

		// Token: 0x04000737 RID: 1847
		private int slashCount;

		// Token: 0x04000738 RID: 1848
		private Transform modelTransform;
	}
}
