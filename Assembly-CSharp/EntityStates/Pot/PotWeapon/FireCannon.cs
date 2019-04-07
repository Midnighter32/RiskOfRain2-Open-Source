using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Pot.PotWeapon
{
	// Token: 0x020000F8 RID: 248
	internal class FireCannon : BaseState
	{
		// Token: 0x060004C1 RID: 1217 RVA: 0x00013E24 File Offset: 0x00012024
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
				EffectManager.instance.SimpleMuzzleFlash(FireCannon.effectPrefab, base.gameObject, targetMuzzle, false);
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

		// Token: 0x060004C2 RID: 1218 RVA: 0x00013FF8 File Offset: 0x000121F8
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FireCannon.baseDuration / this.attackSpeedStat;
			this.modelTransform = base.GetModelTransform();
			this.aimRay = base.GetAimRay();
			base.StartAimMode(this.aimRay, 2f, false);
		}

		// Token: 0x060004C3 RID: 1219 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x060004C4 RID: 1220 RVA: 0x00014048 File Offset: 0x00012248
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

		// Token: 0x060004C5 RID: 1221 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04000485 RID: 1157
		public static GameObject effectPrefab;

		// Token: 0x04000486 RID: 1158
		public static GameObject hitEffectPrefab;

		// Token: 0x04000487 RID: 1159
		public static GameObject projectilePrefab;

		// Token: 0x04000488 RID: 1160
		public static float selfForce = 1000f;

		// Token: 0x04000489 RID: 1161
		public static int grenadeCountMax = 3;

		// Token: 0x0400048A RID: 1162
		public static float damageCoefficient;

		// Token: 0x0400048B RID: 1163
		public static float fireDuration = 1f;

		// Token: 0x0400048C RID: 1164
		public static float baseDuration = 2f;

		// Token: 0x0400048D RID: 1165
		public static float minSpread = 0f;

		// Token: 0x0400048E RID: 1166
		public static float maxSpread = 5f;

		// Token: 0x0400048F RID: 1167
		public static float arcAngle = 5f;

		// Token: 0x04000490 RID: 1168
		private Ray aimRay;

		// Token: 0x04000491 RID: 1169
		private Transform modelTransform;

		// Token: 0x04000492 RID: 1170
		private float duration;

		// Token: 0x04000493 RID: 1171
		private float fireTimer;

		// Token: 0x04000494 RID: 1172
		private int grenadeCount;
	}
}
