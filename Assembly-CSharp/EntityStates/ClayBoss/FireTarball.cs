using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.ClayBoss
{
	// Token: 0x020008D3 RID: 2259
	public class FireTarball : BaseState
	{
		// Token: 0x0600329B RID: 12955 RVA: 0x000DAE44 File Offset: 0x000D9044
		private void FireSingleTarball(string targetMuzzle)
		{
			base.PlayCrossfade("Body", "FireTarBall", 0.1f);
			Util.PlaySound(FireTarball.attackSoundString, base.gameObject);
			this.aimRay = base.GetAimRay();
			if (this.modelTransform)
			{
				ChildLocator component = this.modelTransform.GetComponent<ChildLocator>();
				if (component)
				{
					Transform transform = component.FindChild(targetMuzzle);
					if (transform)
					{
						this.aimRay.origin = transform.position;
					}
				}
			}
			base.AddRecoil(-1f * FireTarball.recoilAmplitude, -2f * FireTarball.recoilAmplitude, -1f * FireTarball.recoilAmplitude, 1f * FireTarball.recoilAmplitude);
			if (FireTarball.effectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(FireTarball.effectPrefab, base.gameObject, targetMuzzle, false);
			}
			if (base.isAuthority)
			{
				Vector3 forward = Vector3.ProjectOnPlane(this.aimRay.direction, Vector3.up);
				ProjectileManager.instance.FireProjectile(FireTarball.projectilePrefab, this.aimRay.origin, Util.QuaternionSafeLookRotation(forward), base.gameObject, this.damageStat * FireTarball.damageCoefficient, 0f, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
			}
			base.characterBody.AddSpreadBloom(FireTarball.spreadBloomValue);
		}

		// Token: 0x0600329C RID: 12956 RVA: 0x000DAF98 File Offset: 0x000D9198
		public override void OnEnter()
		{
			base.OnEnter();
			this.timeBetweenShots = FireTarball.baseTimeBetweenShots / this.attackSpeedStat;
			this.duration = (FireTarball.baseTimeBetweenShots * (float)FireTarball.tarballCountMax + FireTarball.cooldownDuration) / this.attackSpeedStat;
			this.modelTransform = base.GetModelTransform();
			if (base.characterBody)
			{
				base.characterBody.SetAimTimer(2f);
			}
		}

		// Token: 0x0600329D RID: 12957 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x0600329E RID: 12958 RVA: 0x000DB008 File Offset: 0x000D9208
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.fireTimer -= Time.fixedDeltaTime;
			if (this.fireTimer <= 0f)
			{
				if (this.tarballCount < FireTarball.tarballCountMax)
				{
					this.fireTimer += this.timeBetweenShots;
					this.FireSingleTarball("BottomMuzzle");
					this.tarballCount++;
				}
				else
				{
					this.fireTimer += 9999f;
					base.PlayCrossfade("Body", "ExitTarBall", "ExitTarBall.playbackRate", (FireTarball.cooldownDuration - FireTarball.baseTimeBetweenShots) / this.attackSpeedStat, 0.1f);
				}
			}
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x0600329F RID: 12959 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x040031A9 RID: 12713
		public static GameObject effectPrefab;

		// Token: 0x040031AA RID: 12714
		public static GameObject projectilePrefab;

		// Token: 0x040031AB RID: 12715
		public static int tarballCountMax = 3;

		// Token: 0x040031AC RID: 12716
		public static float damageCoefficient;

		// Token: 0x040031AD RID: 12717
		public static float baseTimeBetweenShots = 1f;

		// Token: 0x040031AE RID: 12718
		public static float cooldownDuration = 2f;

		// Token: 0x040031AF RID: 12719
		public static float recoilAmplitude = 1f;

		// Token: 0x040031B0 RID: 12720
		public static string attackSoundString;

		// Token: 0x040031B1 RID: 12721
		public static float spreadBloomValue = 0.3f;

		// Token: 0x040031B2 RID: 12722
		private int tarballCount;

		// Token: 0x040031B3 RID: 12723
		private Ray aimRay;

		// Token: 0x040031B4 RID: 12724
		private Transform modelTransform;

		// Token: 0x040031B5 RID: 12725
		private float duration;

		// Token: 0x040031B6 RID: 12726
		private float fireTimer;

		// Token: 0x040031B7 RID: 12727
		private float timeBetweenShots;
	}
}
