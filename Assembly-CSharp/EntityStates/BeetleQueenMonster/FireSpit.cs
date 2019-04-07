using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.BeetleQueenMonster
{
	// Token: 0x020001D2 RID: 466
	internal class FireSpit : BaseState
	{
		// Token: 0x06000914 RID: 2324 RVA: 0x0002D960 File Offset: 0x0002BB60
		public override void OnEnter()
		{
			base.OnEnter();
			string muzzleName = "Mouth";
			this.duration = FireSpit.baseDuration / this.attackSpeedStat;
			if (FireSpit.effectPrefab)
			{
				EffectManager.instance.SimpleMuzzleFlash(FireSpit.effectPrefab, base.gameObject, muzzleName, false);
			}
			base.PlayCrossfade("Gesture", "FireSpit", "FireSpit.playbackRate", this.duration, 0.1f);
			this.aimRay = base.GetAimRay();
			float magnitude = FireSpit.projectileHSpeed;
			Ray ray = this.aimRay;
			ray.origin = this.aimRay.GetPoint(6f);
			RaycastHit raycastHit;
			if (Util.CharacterRaycast(base.gameObject, ray, out raycastHit, float.PositiveInfinity, LayerIndex.world.mask | LayerIndex.entityPrecise.mask, QueryTriggerInteraction.Ignore))
			{
				float num = magnitude;
				Vector3 vector = raycastHit.point - this.aimRay.origin;
				Vector2 vector2 = new Vector2(vector.x, vector.z);
				float magnitude2 = vector2.magnitude;
				float y = Trajectory.CalculateInitialYSpeed(magnitude2 / num, vector.y);
				Vector3 a = new Vector3(vector2.x / magnitude2 * num, y, vector2.y / magnitude2 * num);
				magnitude = a.magnitude;
				this.aimRay.direction = a / magnitude;
			}
			EffectManager.instance.SimpleMuzzleFlash(FireSpit.effectPrefab, base.gameObject, muzzleName, false);
			if (base.isAuthority)
			{
				for (int i = 0; i < FireSpit.projectileCount; i++)
				{
					this.FireBlob(this.aimRay, 0f, ((float)FireSpit.projectileCount / 2f - (float)i) * FireSpit.yawSpread, magnitude);
				}
			}
		}

		// Token: 0x06000915 RID: 2325 RVA: 0x0002DB2C File Offset: 0x0002BD2C
		private void FireBlob(Ray aimRay, float bonusPitch, float bonusYaw, float speed)
		{
			Vector3 forward = Util.ApplySpread(aimRay.direction, FireSpit.minSpread, FireSpit.maxSpread, 1f, 1f, bonusYaw, bonusPitch);
			ProjectileManager.instance.FireProjectile(FireSpit.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(forward), base.gameObject, this.damageStat * FireSpit.damageCoefficient, 0f, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, speed);
		}

		// Token: 0x06000916 RID: 2326 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06000917 RID: 2327 RVA: 0x0002DBA9 File Offset: 0x0002BDA9
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06000918 RID: 2328 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04000C47 RID: 3143
		public static GameObject projectilePrefab;

		// Token: 0x04000C48 RID: 3144
		public static GameObject effectPrefab;

		// Token: 0x04000C49 RID: 3145
		public static float baseDuration = 2f;

		// Token: 0x04000C4A RID: 3146
		public static float damageCoefficient = 1.2f;

		// Token: 0x04000C4B RID: 3147
		public static float force = 20f;

		// Token: 0x04000C4C RID: 3148
		public static int projectileCount = 3;

		// Token: 0x04000C4D RID: 3149
		public static float yawSpread = 5f;

		// Token: 0x04000C4E RID: 3150
		public static float minSpread = 0f;

		// Token: 0x04000C4F RID: 3151
		public static float maxSpread = 5f;

		// Token: 0x04000C50 RID: 3152
		public static float arcAngle = 5f;

		// Token: 0x04000C51 RID: 3153
		public static float projectileHSpeed = 50f;

		// Token: 0x04000C52 RID: 3154
		private Ray aimRay;

		// Token: 0x04000C53 RID: 3155
		private float duration;
	}
}
