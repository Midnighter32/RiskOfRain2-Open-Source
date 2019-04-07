using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.ClayBoss
{
	// Token: 0x020001B8 RID: 440
	internal class FireTarball : BaseState
	{
		// Token: 0x0600089C RID: 2204 RVA: 0x0002B228 File Offset: 0x00029428
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
				EffectManager.instance.SimpleMuzzleFlash(FireTarball.effectPrefab, base.gameObject, targetMuzzle, false);
			}
			if (base.isAuthority)
			{
				Vector3 forward = Vector3.ProjectOnPlane(this.aimRay.direction, Vector3.up);
				ProjectileManager.instance.FireProjectile(FireTarball.projectilePrefab, this.aimRay.origin, Util.QuaternionSafeLookRotation(forward), base.gameObject, this.damageStat * FireTarball.damageCoefficient, 0f, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
			}
			base.characterBody.AddSpreadBloom(FireTarball.spreadBloomValue);
		}

		// Token: 0x0600089D RID: 2205 RVA: 0x0002B380 File Offset: 0x00029580
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

		// Token: 0x0600089E RID: 2206 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x0600089F RID: 2207 RVA: 0x0002B3F0 File Offset: 0x000295F0
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

		// Token: 0x060008A0 RID: 2208 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04000B81 RID: 2945
		public static GameObject effectPrefab;

		// Token: 0x04000B82 RID: 2946
		public static GameObject projectilePrefab;

		// Token: 0x04000B83 RID: 2947
		public static int tarballCountMax = 3;

		// Token: 0x04000B84 RID: 2948
		public static float damageCoefficient;

		// Token: 0x04000B85 RID: 2949
		public static float baseTimeBetweenShots = 1f;

		// Token: 0x04000B86 RID: 2950
		public static float cooldownDuration = 2f;

		// Token: 0x04000B87 RID: 2951
		public static float recoilAmplitude = 1f;

		// Token: 0x04000B88 RID: 2952
		public static string attackSoundString;

		// Token: 0x04000B89 RID: 2953
		public static float spreadBloomValue = 0.3f;

		// Token: 0x04000B8A RID: 2954
		private int tarballCount;

		// Token: 0x04000B8B RID: 2955
		private Ray aimRay;

		// Token: 0x04000B8C RID: 2956
		private Transform modelTransform;

		// Token: 0x04000B8D RID: 2957
		private float duration;

		// Token: 0x04000B8E RID: 2958
		private float fireTimer;

		// Token: 0x04000B8F RID: 2959
		private float timeBetweenShots;
	}
}
