using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.RoboBallBoss.Weapon
{
	// Token: 0x0200079E RID: 1950
	public class FireEyeBlast : BaseState
	{
		// Token: 0x06002CAB RID: 11435 RVA: 0x000BC6F0 File Offset: 0x000BA8F0
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = this.baseDuration / this.attackSpeedStat;
			this.fireDuration = this.baseFireDuration / this.attackSpeedStat;
			base.PlayAnimation("Gesture, Additive", "FireEyeBlast", "FireEyeBlast.playbackRate", this.duration);
			Util.PlaySound(FireEyeBlast.attackString, base.gameObject);
			if (base.isAuthority)
			{
				base.healthComponent.TakeDamageForce(base.GetAimRay().direction * FireEyeBlast.selfForce, false, false);
			}
			if (UnityEngine.Random.value <= 0.5f)
			{
				this.projectileSpreadIsYaw = true;
			}
		}

		// Token: 0x06002CAC RID: 11436 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06002CAD RID: 11437 RVA: 0x000BC798 File Offset: 0x000BA998
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority)
			{
				int num = Mathf.FloorToInt(base.fixedAge / this.fireDuration * (float)this.projectileCount);
				if (this.projectilesFired <= num && this.projectilesFired < this.projectileCount)
				{
					if (FireEyeBlast.muzzleflashEffectPrefab)
					{
						EffectManager.SimpleMuzzleFlash(FireEyeBlast.muzzleflashEffectPrefab, base.gameObject, FireEyeBlast.muzzleString, false);
					}
					Ray aimRay = base.GetAimRay();
					float speedOverride = this.projectileSpeed;
					int num2 = Mathf.FloorToInt((float)this.projectilesFired - (float)(this.projectileCount - 1) / 2f);
					float bonusYaw = 0f;
					float bonusPitch = 0f;
					if (this.projectileSpreadIsYaw)
					{
						bonusYaw = (float)num2 / (float)(this.projectileCount - 1) * this.totalYawSpread;
					}
					else
					{
						bonusPitch = (float)num2 / (float)(this.projectileCount - 1) * this.totalYawSpread;
					}
					Vector3 forward = Util.ApplySpread(aimRay.direction, 0f, 0f, 1f, 1f, bonusYaw, bonusPitch);
					ProjectileManager.instance.FireProjectile(this.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(forward), base.gameObject, this.damageStat * this.damageCoefficient, FireEyeBlast.force, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, speedOverride);
					this.projectilesFired++;
				}
			}
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06002CAE RID: 11438 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x040028C9 RID: 10441
		[SerializeField]
		public GameObject projectilePrefab;

		// Token: 0x040028CA RID: 10442
		public static GameObject muzzleflashEffectPrefab;

		// Token: 0x040028CB RID: 10443
		[SerializeField]
		public int projectileCount = 3;

		// Token: 0x040028CC RID: 10444
		[SerializeField]
		public float totalYawSpread = 5f;

		// Token: 0x040028CD RID: 10445
		[SerializeField]
		public float baseDuration = 2f;

		// Token: 0x040028CE RID: 10446
		[SerializeField]
		public float baseFireDuration = 0.2f;

		// Token: 0x040028CF RID: 10447
		[SerializeField]
		public float damageCoefficient = 1.2f;

		// Token: 0x040028D0 RID: 10448
		[SerializeField]
		public float projectileSpeed;

		// Token: 0x040028D1 RID: 10449
		public static float force = 20f;

		// Token: 0x040028D2 RID: 10450
		public static float selfForce;

		// Token: 0x040028D3 RID: 10451
		public static string attackString;

		// Token: 0x040028D4 RID: 10452
		public static string muzzleString;

		// Token: 0x040028D5 RID: 10453
		private float duration;

		// Token: 0x040028D6 RID: 10454
		private float fireDuration;

		// Token: 0x040028D7 RID: 10455
		private int projectilesFired;

		// Token: 0x040028D8 RID: 10456
		private bool projectileSpreadIsYaw;
	}
}
