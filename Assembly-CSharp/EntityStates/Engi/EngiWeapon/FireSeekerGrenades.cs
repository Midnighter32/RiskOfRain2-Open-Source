using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Engi.EngiWeapon
{
	// Token: 0x0200018A RID: 394
	internal class FireSeekerGrenades : BaseState
	{
		// Token: 0x06000798 RID: 1944 RVA: 0x00025654 File Offset: 0x00023854
		private void FireGrenade(string targetMuzzle)
		{
			Util.PlaySound(FireSeekerGrenades.attackSoundString, base.gameObject);
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
			base.AddRecoil(-1f * FireSeekerGrenades.recoilAmplitude, -2f * FireSeekerGrenades.recoilAmplitude, -1f * FireSeekerGrenades.recoilAmplitude, 1f * FireSeekerGrenades.recoilAmplitude);
			if (FireSeekerGrenades.effectPrefab)
			{
				EffectManager.instance.SimpleMuzzleFlash(FireSeekerGrenades.effectPrefab, base.gameObject, targetMuzzle, false);
			}
			if (base.isAuthority)
			{
				float x = UnityEngine.Random.Range(FireSeekerGrenades.minSpread, FireSeekerGrenades.maxSpread);
				float z = UnityEngine.Random.Range(0f, 360f);
				Vector3 up = Vector3.up;
				Vector3 axis = Vector3.Cross(up, this.aimRay.direction);
				Vector3 vector = Quaternion.Euler(0f, 0f, z) * (Quaternion.Euler(x, 0f, 0f) * Vector3.forward);
				float y = vector.y;
				vector.y = 0f;
				float angle = Mathf.Atan2(vector.z, vector.x) * 57.29578f - 90f;
				float angle2 = Mathf.Atan2(y, vector.magnitude) * 57.29578f + FireSeekerGrenades.arcAngle;
				Vector3 forward = Quaternion.AngleAxis(angle, up) * (Quaternion.AngleAxis(angle2, axis) * this.aimRay.direction);
				ProjectileManager.instance.FireProjectile(FireSeekerGrenades.projectilePrefab, this.aimRay.origin, Util.QuaternionSafeLookRotation(forward), base.gameObject, this.damageStat * FireSeekerGrenades.damageCoefficient, 0f, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
			}
		}

		// Token: 0x06000799 RID: 1945 RVA: 0x00025858 File Offset: 0x00023A58
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FireSeekerGrenades.baseDuration / this.attackSpeedStat;
			this.modelTransform = base.GetModelTransform();
			this.aimRay = base.GetAimRay();
			base.StartAimMode(this.aimRay, 2f, false);
		}

		// Token: 0x0600079A RID: 1946 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x0600079B RID: 1947 RVA: 0x000258A8 File Offset: 0x00023AA8
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority)
			{
				this.fireTimer -= Time.fixedDeltaTime;
				float num = FireSeekerGrenades.fireDuration / this.attackSpeedStat / (float)FireSeekerGrenades.grenadeCountMax;
				if (this.fireTimer <= 0f && this.grenadeCount < FireSeekerGrenades.grenadeCountMax)
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

		// Token: 0x0600079C RID: 1948 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x040009BD RID: 2493
		public static GameObject effectPrefab;

		// Token: 0x040009BE RID: 2494
		public static GameObject hitEffectPrefab;

		// Token: 0x040009BF RID: 2495
		public static GameObject projectilePrefab;

		// Token: 0x040009C0 RID: 2496
		public static int grenadeCountMax = 3;

		// Token: 0x040009C1 RID: 2497
		public static float damageCoefficient;

		// Token: 0x040009C2 RID: 2498
		public static float fireDuration = 1f;

		// Token: 0x040009C3 RID: 2499
		public static float baseDuration = 2f;

		// Token: 0x040009C4 RID: 2500
		public static float minSpread = 0f;

		// Token: 0x040009C5 RID: 2501
		public static float maxSpread = 5f;

		// Token: 0x040009C6 RID: 2502
		public static float arcAngle = 5f;

		// Token: 0x040009C7 RID: 2503
		public static float recoilAmplitude = 1f;

		// Token: 0x040009C8 RID: 2504
		public static string attackSoundString;

		// Token: 0x040009C9 RID: 2505
		private Ray aimRay;

		// Token: 0x040009CA RID: 2506
		private Transform modelTransform;

		// Token: 0x040009CB RID: 2507
		private float duration;

		// Token: 0x040009CC RID: 2508
		private float fireTimer;

		// Token: 0x040009CD RID: 2509
		private int grenadeCount;
	}
}
