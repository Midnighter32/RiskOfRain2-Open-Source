using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Drone.DroneWeapon
{
	// Token: 0x02000197 RID: 407
	internal class FireMissileBarrage : BaseState
	{
		// Token: 0x060007D5 RID: 2005 RVA: 0x00026CD4 File Offset: 0x00024ED4
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
				EffectManager.instance.SimpleMuzzleFlash(FireMissileBarrage.effectPrefab, base.gameObject, targetMuzzle, false);
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

		// Token: 0x060007D6 RID: 2006 RVA: 0x00026EB7 File Offset: 0x000250B7
		public override void OnEnter()
		{
			base.OnEnter();
			this.modelTransform = base.GetModelTransform();
			this.fireInterval = FireMissileBarrage.baseFireInterval / this.attackSpeedStat;
		}

		// Token: 0x060007D7 RID: 2007 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x060007D8 RID: 2008 RVA: 0x00026EE0 File Offset: 0x000250E0
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

		// Token: 0x060007D9 RID: 2009 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04000A35 RID: 2613
		public static GameObject effectPrefab;

		// Token: 0x04000A36 RID: 2614
		public static GameObject projectilePrefab;

		// Token: 0x04000A37 RID: 2615
		public static float damageCoefficient = 1f;

		// Token: 0x04000A38 RID: 2616
		public static float baseFireInterval = 0.1f;

		// Token: 0x04000A39 RID: 2617
		public static float minSpread = 0f;

		// Token: 0x04000A3A RID: 2618
		public static float maxSpread = 5f;

		// Token: 0x04000A3B RID: 2619
		public static int maxMissileCount;

		// Token: 0x04000A3C RID: 2620
		private float fireTimer;

		// Token: 0x04000A3D RID: 2621
		private float fireInterval;

		// Token: 0x04000A3E RID: 2622
		private Transform modelTransform;

		// Token: 0x04000A3F RID: 2623
		private AimAnimator aimAnimator;

		// Token: 0x04000A40 RID: 2624
		private int missileCount;
	}
}
