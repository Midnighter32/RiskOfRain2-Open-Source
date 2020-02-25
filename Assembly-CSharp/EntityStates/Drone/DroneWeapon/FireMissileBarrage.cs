using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Drone.DroneWeapon
{
	// Token: 0x0200089C RID: 2204
	public class FireMissileBarrage : BaseState
	{
		// Token: 0x0600316A RID: 12650 RVA: 0x000D4CB0 File Offset: 0x000D2EB0
		private void FireMissile(string targetMuzzle)
		{
			this.missileCount++;
			base.PlayAnimation("Gesture, Additive", "FireMissile");
			Ray aimRay = base.GetAimRay();
			if (this.modelTransform)
			{
				ChildLocator component = this.modelTransform.GetComponent<ChildLocator>();
				if (component)
				{
					Transform transform = component.FindChild(targetMuzzle);
					if (transform)
					{
						aimRay.origin = transform.position;
					}
				}
			}
			if (FireMissileBarrage.effectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(FireMissileBarrage.effectPrefab, base.gameObject, targetMuzzle, false);
			}
			if (base.characterBody)
			{
				base.characterBody.SetAimTimer(2f);
			}
			if (base.isAuthority)
			{
				float x = UnityEngine.Random.Range(FireMissileBarrage.minSpread, FireMissileBarrage.maxSpread);
				float z = UnityEngine.Random.Range(0f, 360f);
				Vector3 up = Vector3.up;
				Vector3 axis = Vector3.Cross(up, aimRay.direction);
				Vector3 vector = Quaternion.Euler(0f, 0f, z) * (Quaternion.Euler(x, 0f, 0f) * Vector3.forward);
				float y = vector.y;
				vector.y = 0f;
				float angle = Mathf.Atan2(vector.z, vector.x) * 57.29578f - 90f;
				float angle2 = Mathf.Atan2(y, vector.magnitude) * 57.29578f;
				Vector3 forward = Quaternion.AngleAxis(angle, up) * (Quaternion.AngleAxis(angle2, axis) * aimRay.direction);
				ProjectileManager.instance.FireProjectile(FireMissileBarrage.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(forward), base.gameObject, this.damageStat * FireMissileBarrage.damageCoefficient, 0f, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
			}
		}

		// Token: 0x0600316B RID: 12651 RVA: 0x000D4E8E File Offset: 0x000D308E
		public override void OnEnter()
		{
			base.OnEnter();
			this.modelTransform = base.GetModelTransform();
			this.fireInterval = FireMissileBarrage.baseFireInterval / this.attackSpeedStat;
		}

		// Token: 0x0600316C RID: 12652 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x0600316D RID: 12653 RVA: 0x000D4EB4 File Offset: 0x000D30B4
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.fireTimer -= Time.fixedDeltaTime;
			if (this.fireTimer <= 0f)
			{
				this.FireMissile("Muzzle");
				this.fireTimer += this.fireInterval;
			}
			if (this.missileCount >= FireMissileBarrage.maxMissileCount && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x0600316E RID: 12654 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04002FD4 RID: 12244
		public static GameObject effectPrefab;

		// Token: 0x04002FD5 RID: 12245
		public static GameObject projectilePrefab;

		// Token: 0x04002FD6 RID: 12246
		public static float damageCoefficient = 1f;

		// Token: 0x04002FD7 RID: 12247
		public static float baseFireInterval = 0.1f;

		// Token: 0x04002FD8 RID: 12248
		public static float minSpread = 0f;

		// Token: 0x04002FD9 RID: 12249
		public static float maxSpread = 5f;

		// Token: 0x04002FDA RID: 12250
		public static int maxMissileCount;

		// Token: 0x04002FDB RID: 12251
		private float fireTimer;

		// Token: 0x04002FDC RID: 12252
		private float fireInterval;

		// Token: 0x04002FDD RID: 12253
		private Transform modelTransform;

		// Token: 0x04002FDE RID: 12254
		private AimAnimator aimAnimator;

		// Token: 0x04002FDF RID: 12255
		private int missileCount;
	}
}
