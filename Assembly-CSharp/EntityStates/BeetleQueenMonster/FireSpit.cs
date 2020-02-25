using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.BeetleQueenMonster
{
	// Token: 0x020008ED RID: 2285
	public class FireSpit : BaseState
	{
		// Token: 0x06003314 RID: 13076 RVA: 0x000DD5C8 File Offset: 0x000DB7C8
		public override void OnEnter()
		{
			base.OnEnter();
			string muzzleName = "Mouth";
			this.duration = FireSpit.baseDuration / this.attackSpeedStat;
			if (FireSpit.effectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(FireSpit.effectPrefab, base.gameObject, muzzleName, false);
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
			EffectManager.SimpleMuzzleFlash(FireSpit.effectPrefab, base.gameObject, muzzleName, false);
			if (base.isAuthority)
			{
				for (int i = 0; i < FireSpit.projectileCount; i++)
				{
					this.FireBlob(this.aimRay, 0f, ((float)FireSpit.projectileCount / 2f - (float)i) * FireSpit.yawSpread, magnitude);
				}
			}
		}

		// Token: 0x06003315 RID: 13077 RVA: 0x000DD788 File Offset: 0x000DB988
		private void FireBlob(Ray aimRay, float bonusPitch, float bonusYaw, float speed)
		{
			Vector3 forward = Util.ApplySpread(aimRay.direction, FireSpit.minSpread, FireSpit.maxSpread, 1f, 1f, bonusYaw, bonusPitch);
			ProjectileManager.instance.FireProjectile(FireSpit.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(forward), base.gameObject, this.damageStat * FireSpit.damageCoefficient, 0f, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, speed);
		}

		// Token: 0x06003316 RID: 13078 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06003317 RID: 13079 RVA: 0x000DD805 File Offset: 0x000DBA05
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06003318 RID: 13080 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x0400326F RID: 12911
		public static GameObject projectilePrefab;

		// Token: 0x04003270 RID: 12912
		public static GameObject effectPrefab;

		// Token: 0x04003271 RID: 12913
		public static float baseDuration = 2f;

		// Token: 0x04003272 RID: 12914
		public static float damageCoefficient = 1.2f;

		// Token: 0x04003273 RID: 12915
		public static float force = 20f;

		// Token: 0x04003274 RID: 12916
		public static int projectileCount = 3;

		// Token: 0x04003275 RID: 12917
		public static float yawSpread = 5f;

		// Token: 0x04003276 RID: 12918
		public static float minSpread = 0f;

		// Token: 0x04003277 RID: 12919
		public static float maxSpread = 5f;

		// Token: 0x04003278 RID: 12920
		public static float arcAngle = 5f;

		// Token: 0x04003279 RID: 12921
		public static float projectileHSpeed = 50f;

		// Token: 0x0400327A RID: 12922
		private Ray aimRay;

		// Token: 0x0400327B RID: 12923
		private float duration;
	}
}
