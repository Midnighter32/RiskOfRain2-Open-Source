using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Engi.EngiWeapon
{
	// Token: 0x02000186 RID: 390
	internal class FireConcussionBlast : BaseState
	{
		// Token: 0x06000783 RID: 1923 RVA: 0x00024E78 File Offset: 0x00023078
		private void FireGrenade(string targetMuzzle)
		{
			Util.PlaySound(FireConcussionBlast.attackSoundString, base.gameObject);
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
			base.AddRecoil(-1f * FireConcussionBlast.recoilAmplitude, -2f * FireConcussionBlast.recoilAmplitude, -1f * FireConcussionBlast.recoilAmplitude, 1f * FireConcussionBlast.recoilAmplitude);
			if (FireConcussionBlast.effectPrefab)
			{
				EffectManager.instance.SimpleMuzzleFlash(FireConcussionBlast.effectPrefab, base.gameObject, targetMuzzle, false);
			}
			if (base.isAuthority)
			{
				new BulletAttack
				{
					owner = base.gameObject,
					weapon = base.gameObject,
					origin = this.aimRay.origin,
					aimVector = this.aimRay.direction,
					minSpread = FireConcussionBlast.minSpread,
					maxSpread = FireConcussionBlast.maxSpread,
					damage = FireConcussionBlast.damageCoefficient * this.damageStat,
					force = FireConcussionBlast.force,
					tracerEffectPrefab = FireConcussionBlast.tracerEffectPrefab,
					muzzleName = targetMuzzle,
					hitEffectPrefab = FireConcussionBlast.hitEffectPrefab,
					isCrit = Util.CheckRoll(this.critStat, base.characterBody.master),
					maxDistance = FireConcussionBlast.maxDistance,
					radius = FireConcussionBlast.radius,
					stopperMask = 0
				}.Fire();
			}
		}

		// Token: 0x06000784 RID: 1924 RVA: 0x00025018 File Offset: 0x00023218
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FireConcussionBlast.baseDuration / this.attackSpeedStat;
			this.modelTransform = base.GetModelTransform();
			this.aimRay = base.GetAimRay();
			base.StartAimMode(this.aimRay, 2f, false);
		}

		// Token: 0x06000785 RID: 1925 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06000786 RID: 1926 RVA: 0x00025068 File Offset: 0x00023268
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority)
			{
				this.fireTimer -= Time.fixedDeltaTime;
				float num = FireConcussionBlast.fireDuration / this.attackSpeedStat / (float)FireConcussionBlast.grenadeCountMax;
				if (this.fireTimer <= 0f && this.grenadeCount < FireConcussionBlast.grenadeCountMax)
				{
					this.fireTimer += num;
					if (this.grenadeCount % 2 == 0)
					{
						this.FireGrenade("MuzzleLeft");
						base.PlayCrossfade("Gesture, Left Cannon", "FireGrenadeLeft", 0.1f);
					}
					else
					{
						this.FireGrenade("MuzzleRight");
						base.PlayCrossfade("Gesture, Right Cannon", "FireGrenadeRight", 0.1f);
					}
					this.grenadeCount++;
				}
				if (base.fixedAge >= this.duration)
				{
					this.outer.SetNextStateToMain();
					return;
				}
			}
		}

		// Token: 0x06000787 RID: 1927 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04000991 RID: 2449
		public static GameObject effectPrefab;

		// Token: 0x04000992 RID: 2450
		public static GameObject hitEffectPrefab;

		// Token: 0x04000993 RID: 2451
		public static int grenadeCountMax = 3;

		// Token: 0x04000994 RID: 2452
		public static float damageCoefficient;

		// Token: 0x04000995 RID: 2453
		public static float fireDuration = 1f;

		// Token: 0x04000996 RID: 2454
		public static float baseDuration = 2f;

		// Token: 0x04000997 RID: 2455
		public static float minSpread = 0f;

		// Token: 0x04000998 RID: 2456
		public static float maxSpread = 5f;

		// Token: 0x04000999 RID: 2457
		public static float recoilAmplitude = 1f;

		// Token: 0x0400099A RID: 2458
		public static string attackSoundString;

		// Token: 0x0400099B RID: 2459
		public static float force;

		// Token: 0x0400099C RID: 2460
		public static float maxDistance;

		// Token: 0x0400099D RID: 2461
		public static float radius;

		// Token: 0x0400099E RID: 2462
		public static GameObject tracerEffectPrefab;

		// Token: 0x0400099F RID: 2463
		private Ray aimRay;

		// Token: 0x040009A0 RID: 2464
		private Transform modelTransform;

		// Token: 0x040009A1 RID: 2465
		private float duration;

		// Token: 0x040009A2 RID: 2466
		private float fireTimer;

		// Token: 0x040009A3 RID: 2467
		private int grenadeCount;
	}
}
