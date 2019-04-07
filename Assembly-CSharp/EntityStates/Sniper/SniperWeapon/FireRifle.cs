using System;
using EntityStates.Sniper.Scope;
using RoR2;
using UnityEngine;

namespace EntityStates.Sniper.SniperWeapon
{
	// Token: 0x020000F5 RID: 245
	internal class FireRifle : BaseState
	{
		// Token: 0x060004AF RID: 1199 RVA: 0x000137A4 File Offset: 0x000119A4
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
				EffectManager.instance.SimpleMuzzleFlash(FireRifle.effectPrefab, base.gameObject, muzzleName, false);
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

		// Token: 0x060004B0 RID: 1200 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x060004B1 RID: 1201 RVA: 0x00013A2C File Offset: 0x00011C2C
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

		// Token: 0x060004B2 RID: 1202 RVA: 0x00013A8E File Offset: 0x00011C8E
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			if (this.inputReleased && base.fixedAge >= FireRifle.interruptInterval / this.attackSpeedStat)
			{
				return InterruptPriority.Any;
			}
			return InterruptPriority.Skill;
		}

		// Token: 0x04000469 RID: 1129
		public static GameObject effectPrefab;

		// Token: 0x0400046A RID: 1130
		public static GameObject hitEffectPrefab;

		// Token: 0x0400046B RID: 1131
		public static GameObject tracerEffectPrefab;

		// Token: 0x0400046C RID: 1132
		public static float minChargeDamageCoefficient;

		// Token: 0x0400046D RID: 1133
		public static float maxChargeDamageCoefficient;

		// Token: 0x0400046E RID: 1134
		public static float minChargeForce;

		// Token: 0x0400046F RID: 1135
		public static float maxChargeForce;

		// Token: 0x04000470 RID: 1136
		public static int bulletCount;

		// Token: 0x04000471 RID: 1137
		public static float baseDuration = 2f;

		// Token: 0x04000472 RID: 1138
		public static string attackSoundString;

		// Token: 0x04000473 RID: 1139
		public static float recoilAmplitude;

		// Token: 0x04000474 RID: 1140
		public static float spreadBloomValue = 0.3f;

		// Token: 0x04000475 RID: 1141
		public static float interruptInterval = 0.2f;

		// Token: 0x04000476 RID: 1142
		private float duration;

		// Token: 0x04000477 RID: 1143
		private bool inputReleased;
	}
}
