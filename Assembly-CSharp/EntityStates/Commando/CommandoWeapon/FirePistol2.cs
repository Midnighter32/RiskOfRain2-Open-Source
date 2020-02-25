using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Commando.CommandoWeapon
{
	// Token: 0x020008BD RID: 2237
	public class FirePistol2 : BaseState
	{
		// Token: 0x0600322B RID: 12843 RVA: 0x000D8BE0 File Offset: 0x000D6DE0
		private void FireBullet(string targetMuzzle)
		{
			Util.PlaySound(FirePistol2.firePistolSoundString, base.gameObject);
			if (FirePistol2.muzzleEffectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(FirePistol2.muzzleEffectPrefab, base.gameObject, targetMuzzle, false);
			}
			base.AddRecoil(-0.4f * FirePistol2.recoilAmplitude, -0.8f * FirePistol2.recoilAmplitude, -0.3f * FirePistol2.recoilAmplitude, 0.3f * FirePistol2.recoilAmplitude);
			if (base.isAuthority)
			{
				new BulletAttack
				{
					owner = base.gameObject,
					weapon = base.gameObject,
					origin = this.aimRay.origin,
					aimVector = this.aimRay.direction,
					minSpread = 0f,
					maxSpread = base.characterBody.spreadBloomAngle,
					damage = FirePistol2.damageCoefficient * this.damageStat,
					force = FirePistol2.force,
					tracerEffectPrefab = FirePistol2.tracerEffectPrefab,
					muzzleName = targetMuzzle,
					hitEffectPrefab = FirePistol2.hitEffectPrefab,
					isCrit = Util.CheckRoll(this.critStat, base.characterBody.master),
					radius = 0.1f,
					smartCollision = true
				}.Fire();
			}
			base.characterBody.AddSpreadBloom(FirePistol2.spreadBloomValue);
		}

		// Token: 0x0600322C RID: 12844 RVA: 0x000D8D34 File Offset: 0x000D6F34
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FirePistol2.baseDuration / this.attackSpeedStat;
			this.aimRay = base.GetAimRay();
			base.StartAimMode(this.aimRay, 3f, false);
			if (this.remainingShots % 2 == 0)
			{
				base.PlayAnimation("Gesture Additive, Left", "FirePistol, Left");
				this.FireBullet("MuzzleLeft");
				return;
			}
			base.PlayAnimation("Gesture Additive, Right", "FirePistol, Right");
			this.FireBullet("MuzzleRight");
		}

		// Token: 0x0600322D RID: 12845 RVA: 0x000D8DB8 File Offset: 0x000D6FB8
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge < this.duration || !base.isAuthority)
			{
				return;
			}
			this.remainingShots--;
			if (this.remainingShots == 0)
			{
				this.outer.SetNextStateToMain();
				return;
			}
			FirePistol2 firePistol = new FirePistol2();
			firePistol.remainingShots = this.remainingShots;
			this.outer.SetNextState(firePistol);
		}

		// Token: 0x0600322E RID: 12846 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x040030F6 RID: 12534
		public static GameObject muzzleEffectPrefab;

		// Token: 0x040030F7 RID: 12535
		public static GameObject hitEffectPrefab;

		// Token: 0x040030F8 RID: 12536
		public static GameObject tracerEffectPrefab;

		// Token: 0x040030F9 RID: 12537
		public static float damageCoefficient;

		// Token: 0x040030FA RID: 12538
		public static float force;

		// Token: 0x040030FB RID: 12539
		public static float baseDuration = 2f;

		// Token: 0x040030FC RID: 12540
		public static string firePistolSoundString;

		// Token: 0x040030FD RID: 12541
		public static float recoilAmplitude = 1f;

		// Token: 0x040030FE RID: 12542
		public static float spreadBloomValue = 0.3f;

		// Token: 0x040030FF RID: 12543
		public static float commandoBoostBuffCoefficient = 0.4f;

		// Token: 0x04003100 RID: 12544
		public int remainingShots = 2;

		// Token: 0x04003101 RID: 12545
		private Ray aimRay;

		// Token: 0x04003102 RID: 12546
		private float duration;
	}
}
