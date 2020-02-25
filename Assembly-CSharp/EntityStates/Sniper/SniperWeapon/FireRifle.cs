using System;
using EntityStates.Sniper.Scope;
using RoR2;
using UnityEngine;

namespace EntityStates.Sniper.SniperWeapon
{
	// Token: 0x02000787 RID: 1927
	public class FireRifle : BaseState
	{
		// Token: 0x06002C3F RID: 11327 RVA: 0x000BAAE4 File Offset: 0x000B8CE4
		public override void OnEnter()
		{
			base.OnEnter();
			float num = 0f;
			if (base.skillLocator)
			{
				GenericSkill secondary = base.skillLocator.secondary;
				if (secondary)
				{
					EntityStateMachine stateMachine = secondary.stateMachine;
					if (stateMachine)
					{
						ScopeSniper scopeSniper = stateMachine.state as ScopeSniper;
						if (scopeSniper != null)
						{
							num = scopeSniper.charge;
							scopeSniper.charge = 0f;
						}
					}
				}
			}
			base.AddRecoil(-1f * FireRifle.recoilAmplitude, -2f * FireRifle.recoilAmplitude, -0.5f * FireRifle.recoilAmplitude, 0.5f * FireRifle.recoilAmplitude);
			this.duration = FireRifle.baseDuration / this.attackSpeedStat;
			Ray aimRay = base.GetAimRay();
			base.StartAimMode(aimRay, 2f, false);
			Util.PlaySound(FireRifle.attackSoundString, base.gameObject);
			base.PlayAnimation("Gesture", "FireShotgun", "FireShotgun.playbackRate", this.duration * 1.1f);
			string muzzleName = "MuzzleShotgun";
			if (FireRifle.effectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(FireRifle.effectPrefab, base.gameObject, muzzleName, false);
			}
			if (base.isAuthority)
			{
				BulletAttack bulletAttack = new BulletAttack();
				bulletAttack.owner = base.gameObject;
				bulletAttack.weapon = base.gameObject;
				bulletAttack.origin = aimRay.origin;
				bulletAttack.aimVector = aimRay.direction;
				bulletAttack.minSpread = 0f;
				bulletAttack.maxSpread = base.characterBody.spreadBloomAngle;
				bulletAttack.bulletCount = (uint)((FireRifle.bulletCount > 0) ? FireRifle.bulletCount : 0);
				bulletAttack.procCoefficient = 1f / (float)FireRifle.bulletCount;
				bulletAttack.damage = Mathf.LerpUnclamped(FireRifle.minChargeDamageCoefficient, FireRifle.maxChargeDamageCoefficient, num) * this.damageStat / (float)FireRifle.bulletCount;
				bulletAttack.force = Mathf.LerpUnclamped(FireRifle.minChargeForce, FireRifle.maxChargeForce, num);
				bulletAttack.falloffModel = BulletAttack.FalloffModel.None;
				bulletAttack.tracerEffectPrefab = FireRifle.tracerEffectPrefab;
				bulletAttack.muzzleName = muzzleName;
				bulletAttack.hitEffectPrefab = FireRifle.hitEffectPrefab;
				bulletAttack.isCrit = Util.CheckRoll(this.critStat, base.characterBody.master);
				if (num == 1f)
				{
					bulletAttack.stopperMask = LayerIndex.world.mask;
				}
				bulletAttack.HitEffectNormal = false;
				bulletAttack.radius = 0f;
				bulletAttack.sniper = true;
				bulletAttack.Fire();
			}
			base.characterBody.AddSpreadBloom(FireRifle.spreadBloomValue);
		}

		// Token: 0x06002C40 RID: 11328 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06002C41 RID: 11329 RVA: 0x000BAD64 File Offset: 0x000B8F64
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.inputBank)
			{
				this.inputReleased |= !base.inputBank.skill1.down;
			}
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06002C42 RID: 11330 RVA: 0x000BADC6 File Offset: 0x000B8FC6
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			if (this.inputReleased && base.fixedAge >= FireRifle.interruptInterval / this.attackSpeedStat)
			{
				return InterruptPriority.Any;
			}
			return InterruptPriority.Skill;
		}

		// Token: 0x0400283C RID: 10300
		public static GameObject effectPrefab;

		// Token: 0x0400283D RID: 10301
		public static GameObject hitEffectPrefab;

		// Token: 0x0400283E RID: 10302
		public static GameObject tracerEffectPrefab;

		// Token: 0x0400283F RID: 10303
		public static float minChargeDamageCoefficient;

		// Token: 0x04002840 RID: 10304
		public static float maxChargeDamageCoefficient;

		// Token: 0x04002841 RID: 10305
		public static float minChargeForce;

		// Token: 0x04002842 RID: 10306
		public static float maxChargeForce;

		// Token: 0x04002843 RID: 10307
		public static int bulletCount;

		// Token: 0x04002844 RID: 10308
		public static float baseDuration = 2f;

		// Token: 0x04002845 RID: 10309
		public static string attackSoundString;

		// Token: 0x04002846 RID: 10310
		public static float recoilAmplitude;

		// Token: 0x04002847 RID: 10311
		public static float spreadBloomValue = 0.3f;

		// Token: 0x04002848 RID: 10312
		public static float interruptInterval = 0.2f;

		// Token: 0x04002849 RID: 10313
		private float duration;

		// Token: 0x0400284A RID: 10314
		private bool inputReleased;
	}
}
