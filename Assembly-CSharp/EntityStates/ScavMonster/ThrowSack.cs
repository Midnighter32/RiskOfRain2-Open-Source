using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.ScavMonster
{
	// Token: 0x02000798 RID: 1944
	public class ThrowSack : SackBaseState
	{
		// Token: 0x06002C8D RID: 11405 RVA: 0x000BBDE8 File Offset: 0x000B9FE8
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = ThrowSack.baseDuration / this.attackSpeedStat;
			Util.PlayScaledSound(ThrowSack.sound, base.gameObject, this.attackSpeedStat);
			base.PlayAnimation("Body", "ThrowSack", "ThrowSack.playbackRate", this.duration);
			if (ThrowSack.effectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(ThrowSack.effectPrefab, base.gameObject, SackBaseState.muzzleName, false);
			}
			this.Fire();
		}

		// Token: 0x06002C8E RID: 11406 RVA: 0x000BBE68 File Offset: 0x000BA068
		private void Fire()
		{
			Ray aimRay = base.GetAimRay();
			Ray ray = aimRay;
			Ray ray2 = aimRay;
			Vector3 point = aimRay.GetPoint(ThrowSack.minimumDistance);
			bool flag = false;
			RaycastHit raycastHit;
			if (Util.CharacterRaycast(base.gameObject, ray, out raycastHit, 500f, LayerIndex.world.mask | LayerIndex.entityPrecise.mask, QueryTriggerInteraction.Ignore))
			{
				point = raycastHit.point;
				flag = true;
			}
			float magnitude = ThrowSack.projectileVelocity;
			if (flag)
			{
				Vector3 vector = point - ray2.origin;
				Vector2 a = new Vector2(vector.x, vector.z);
				float magnitude2 = a.magnitude;
				Vector2 vector2 = a / magnitude2;
				if (magnitude2 < ThrowSack.minimumDistance)
				{
					magnitude2 = ThrowSack.minimumDistance;
				}
				float y = Trajectory.CalculateInitialYSpeed(ThrowSack.timeToTarget, vector.y);
				float num = magnitude2 / ThrowSack.timeToTarget;
				Vector3 direction = new Vector3(vector2.x * num, y, vector2.y * num);
				magnitude = direction.magnitude;
				ray2.direction = direction;
			}
			for (int i = 0; i < ThrowSack.projectileCount; i++)
			{
				Quaternion rotation = Util.QuaternionSafeLookRotation(Util.ApplySpread(ray2.direction, ThrowSack.minSpread, ThrowSack.maxSpread, 1f, 1f, 0f, 0f));
				ProjectileManager.instance.FireProjectile(ThrowSack.projectilePrefab, ray2.origin, rotation, base.gameObject, this.damageStat * ThrowSack.damageCoefficient, 0f, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, magnitude);
			}
		}

		// Token: 0x06002C8F RID: 11407 RVA: 0x000BC013 File Offset: 0x000BA213
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x0400289A RID: 10394
		public static float baseDuration;

		// Token: 0x0400289B RID: 10395
		public static string sound;

		// Token: 0x0400289C RID: 10396
		public static GameObject effectPrefab;

		// Token: 0x0400289D RID: 10397
		public static GameObject projectilePrefab;

		// Token: 0x0400289E RID: 10398
		public static float damageCoefficient;

		// Token: 0x0400289F RID: 10399
		public static float force;

		// Token: 0x040028A0 RID: 10400
		public static float minSpread;

		// Token: 0x040028A1 RID: 10401
		public static float maxSpread;

		// Token: 0x040028A2 RID: 10402
		public static string attackSoundString;

		// Token: 0x040028A3 RID: 10403
		public static float projectileVelocity;

		// Token: 0x040028A4 RID: 10404
		public static float minimumDistance;

		// Token: 0x040028A5 RID: 10405
		public static float timeToTarget = 3f;

		// Token: 0x040028A6 RID: 10406
		public static int projectileCount;

		// Token: 0x040028A7 RID: 10407
		private float duration;
	}
}
