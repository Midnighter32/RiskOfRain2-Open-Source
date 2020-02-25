using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Pot.PotWeapon
{
	// Token: 0x020007A9 RID: 1961
	public class FireCannon : BaseState
	{
		// Token: 0x06002CD5 RID: 11477 RVA: 0x000BD06C File Offset: 0x000BB26C
		private void FireBullet(string targetMuzzle)
		{
			this.aimRay = base.GetAimRay();
			if (this.modelTransform)
			{
				ChildLocator component = this.modelTransform.GetComponent<ChildLocator>();
				if (component)
				{
					Transform transform = component.FindChild(targetMuzzle);
					if (transform)
					{
						base.rigidbody.AddForceAtPosition(transform.forward * FireCannon.selfForce, transform.position, ForceMode.Impulse);
					}
				}
			}
			if (FireCannon.effectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(FireCannon.effectPrefab, base.gameObject, targetMuzzle, false);
			}
			if (base.isAuthority)
			{
				float x = UnityEngine.Random.Range(FireCannon.minSpread, FireCannon.maxSpread);
				float z = UnityEngine.Random.Range(0f, 360f);
				Vector3 up = Vector3.up;
				Vector3 axis = Vector3.Cross(up, this.aimRay.direction);
				Vector3 vector = Quaternion.Euler(0f, 0f, z) * (Quaternion.Euler(x, 0f, 0f) * Vector3.forward);
				float y = vector.y;
				vector.y = 0f;
				float angle = Mathf.Atan2(vector.z, vector.x) * 57.29578f - 90f;
				float angle2 = Mathf.Atan2(y, vector.magnitude) * 57.29578f + FireCannon.arcAngle;
				Vector3 forward = Quaternion.AngleAxis(angle, up) * (Quaternion.AngleAxis(angle2, axis) * this.aimRay.direction);
				ProjectileManager.instance.FireProjectile(FireCannon.projectilePrefab, this.aimRay.origin, Util.QuaternionSafeLookRotation(forward), base.gameObject, this.damageStat * FireCannon.damageCoefficient, 0f, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
			}
		}

		// Token: 0x06002CD6 RID: 11478 RVA: 0x000BD23C File Offset: 0x000BB43C
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FireCannon.baseDuration / this.attackSpeedStat;
			this.modelTransform = base.GetModelTransform();
			this.aimRay = base.GetAimRay();
			base.StartAimMode(this.aimRay, 2f, false);
		}

		// Token: 0x06002CD7 RID: 11479 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06002CD8 RID: 11480 RVA: 0x000BD28C File Offset: 0x000BB48C
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority)
			{
				this.fireTimer -= Time.fixedDeltaTime;
				float num = FireCannon.fireDuration / this.attackSpeedStat / (float)FireCannon.grenadeCountMax;
				if (this.fireTimer <= 0f && this.grenadeCount < FireCannon.grenadeCountMax)
				{
					this.fireTimer += num;
					if (this.grenadeCount % 2 == 0)
					{
						this.FireBullet("MuzzleLeft");
						base.PlayCrossfade("Gesture, Left Cannon", "FireGrenadeLeft", 0.1f);
					}
					else
					{
						this.FireBullet("MuzzleRight");
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

		// Token: 0x06002CD9 RID: 11481 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x040028EE RID: 10478
		public static GameObject effectPrefab;

		// Token: 0x040028EF RID: 10479
		public static GameObject hitEffectPrefab;

		// Token: 0x040028F0 RID: 10480
		public static GameObject projectilePrefab;

		// Token: 0x040028F1 RID: 10481
		public static float selfForce = 1000f;

		// Token: 0x040028F2 RID: 10482
		public static int grenadeCountMax = 3;

		// Token: 0x040028F3 RID: 10483
		public static float damageCoefficient;

		// Token: 0x040028F4 RID: 10484
		public static float fireDuration = 1f;

		// Token: 0x040028F5 RID: 10485
		public static float baseDuration = 2f;

		// Token: 0x040028F6 RID: 10486
		public static float minSpread = 0f;

		// Token: 0x040028F7 RID: 10487
		public static float maxSpread = 5f;

		// Token: 0x040028F8 RID: 10488
		public static float arcAngle = 5f;

		// Token: 0x040028F9 RID: 10489
		private Ray aimRay;

		// Token: 0x040028FA RID: 10490
		private Transform modelTransform;

		// Token: 0x040028FB RID: 10491
		private float duration;

		// Token: 0x040028FC RID: 10492
		private float fireTimer;

		// Token: 0x040028FD RID: 10493
		private int grenadeCount;
	}
}
