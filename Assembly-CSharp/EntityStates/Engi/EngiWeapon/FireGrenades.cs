using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Engi.EngiWeapon
{
	// Token: 0x02000187 RID: 391
	internal class FireGrenades : BaseState
	{
		// Token: 0x0600078A RID: 1930 RVA: 0x00025184 File Offset: 0x00023384
		private void FireGrenade(string targetMuzzle)
		{
			Util.PlaySound(FireGrenades.attackSoundString, base.gameObject);
			this.projectileRay = base.GetAimRay();
			if (this.modelTransform)
			{
				ChildLocator component = this.modelTransform.GetComponent<ChildLocator>();
				if (component)
				{
					Transform transform = component.FindChild(targetMuzzle);
					if (transform)
					{
						this.projectileRay.origin = transform.position;
					}
				}
			}
			base.AddRecoil(-1f * FireGrenades.recoilAmplitude, -2f * FireGrenades.recoilAmplitude, -1f * FireGrenades.recoilAmplitude, 1f * FireGrenades.recoilAmplitude);
			if (FireGrenades.effectPrefab)
			{
				EffectManager.instance.SimpleMuzzleFlash(FireGrenades.effectPrefab, base.gameObject, targetMuzzle, false);
			}
			if (base.isAuthority)
			{
				float x = UnityEngine.Random.Range(0f, base.characterBody.spreadBloomAngle);
				float z = UnityEngine.Random.Range(0f, 360f);
				Vector3 up = Vector3.up;
				Vector3 axis = Vector3.Cross(up, this.projectileRay.direction);
				Vector3 vector = Quaternion.Euler(0f, 0f, z) * (Quaternion.Euler(x, 0f, 0f) * Vector3.forward);
				float y = vector.y;
				vector.y = 0f;
				float angle = Mathf.Atan2(vector.z, vector.x) * 57.29578f - 90f;
				float angle2 = Mathf.Atan2(y, vector.magnitude) * 57.29578f + FireGrenades.arcAngle;
				Vector3 forward = Quaternion.AngleAxis(angle, up) * (Quaternion.AngleAxis(angle2, axis) * this.projectileRay.direction);
				ProjectileManager.instance.FireProjectile(FireGrenades.projectilePrefab, this.projectileRay.origin, Util.QuaternionSafeLookRotation(forward), base.gameObject, this.damageStat * FireGrenades.damageCoefficient, 0f, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
			}
			base.characterBody.AddSpreadBloom(FireGrenades.spreadBloomValue);
		}

		// Token: 0x0600078B RID: 1931 RVA: 0x0002539E File Offset: 0x0002359E
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FireGrenades.baseDuration / this.attackSpeedStat;
			this.modelTransform = base.GetModelTransform();
			base.StartAimMode(2f, false);
		}

		// Token: 0x0600078C RID: 1932 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x0600078D RID: 1933 RVA: 0x000253D0 File Offset: 0x000235D0
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority)
			{
				this.fireTimer -= Time.fixedDeltaTime;
				float num = FireGrenades.fireDuration / this.attackSpeedStat / (float)this.grenadeCountMax;
				if (this.fireTimer <= 0f && this.grenadeCount < this.grenadeCountMax)
				{
					this.fireTimer += num;
					if (this.grenadeCount % 2 == 0)
					{
						this.FireGrenade("MuzzleLeft");
						base.PlayCrossfade("Gesture Left Cannon, Additive", "FireGrenadeLeft", 0.1f);
					}
					else
					{
						this.FireGrenade("MuzzleRight");
						base.PlayCrossfade("Gesture Right Cannon, Additive", "FireGrenadeRight", 0.1f);
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

		// Token: 0x0600078E RID: 1934 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x040009A4 RID: 2468
		public static GameObject effectPrefab;

		// Token: 0x040009A5 RID: 2469
		public static GameObject projectilePrefab;

		// Token: 0x040009A6 RID: 2470
		public int grenadeCountMax = 3;

		// Token: 0x040009A7 RID: 2471
		public static float damageCoefficient;

		// Token: 0x040009A8 RID: 2472
		public static float fireDuration = 1f;

		// Token: 0x040009A9 RID: 2473
		public static float baseDuration = 2f;

		// Token: 0x040009AA RID: 2474
		public static float arcAngle = 5f;

		// Token: 0x040009AB RID: 2475
		public static float recoilAmplitude = 1f;

		// Token: 0x040009AC RID: 2476
		public static string attackSoundString;

		// Token: 0x040009AD RID: 2477
		public static float spreadBloomValue = 0.3f;

		// Token: 0x040009AE RID: 2478
		private Ray projectileRay;

		// Token: 0x040009AF RID: 2479
		private Transform modelTransform;

		// Token: 0x040009B0 RID: 2480
		private float duration;

		// Token: 0x040009B1 RID: 2481
		private float fireTimer;

		// Token: 0x040009B2 RID: 2482
		private int grenadeCount;
	}
}
