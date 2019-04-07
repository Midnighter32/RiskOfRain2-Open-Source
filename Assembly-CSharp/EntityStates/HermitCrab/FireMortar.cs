using System;
using System.Linq;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.HermitCrab
{
	// Token: 0x0200015B RID: 347
	internal class FireMortar : BaseState
	{
		// Token: 0x060006BC RID: 1724 RVA: 0x000201CC File Offset: 0x0001E3CC
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FireMortar.baseDuration / this.attackSpeedStat;
			base.PlayCrossfade("Gesture, Additive", "FireMortar", 0f);
			Util.PlaySound(FireMortar.mortarSoundString, base.gameObject);
			EffectManager.instance.SimpleMuzzleFlash(FireMortar.mortarMuzzleflashEffect, base.gameObject, FireMortar.mortarMuzzleName, false);
			if (base.isAuthority)
			{
				this.Fire();
			}
		}

		// Token: 0x060006BD RID: 1725 RVA: 0x00020240 File Offset: 0x0001E440
		private void Fire()
		{
			Ray aimRay = base.GetAimRay();
			Ray ray = new Ray(aimRay.origin, Vector3.up);
			BullseyeSearch bullseyeSearch = new BullseyeSearch();
			bullseyeSearch.searchOrigin = aimRay.origin;
			bullseyeSearch.searchDirection = aimRay.direction;
			bullseyeSearch.filterByLoS = false;
			bullseyeSearch.teamMaskFilter = TeamMask.allButNeutral;
			if (base.teamComponent)
			{
				bullseyeSearch.teamMaskFilter.RemoveTeam(base.teamComponent.teamIndex);
			}
			bullseyeSearch.sortMode = BullseyeSearch.SortMode.Angle;
			bullseyeSearch.RefreshCandidates();
			HurtBox hurtBox = bullseyeSearch.GetResults().FirstOrDefault<HurtBox>();
			bool flag = false;
			Vector3 a = Vector3.zero;
			RaycastHit raycastHit;
			if (hurtBox)
			{
				a = hurtBox.transform.position;
				flag = true;
			}
			else if (Physics.Raycast(aimRay, out raycastHit, 1000f, LayerIndex.world.mask | LayerIndex.entityPrecise.mask, QueryTriggerInteraction.Ignore))
			{
				a = raycastHit.point;
				flag = true;
			}
			float magnitude = FireMortar.projectileVelocity;
			if (flag)
			{
				Vector3 vector = a - ray.origin;
				Vector2 a2 = new Vector2(vector.x, vector.z);
				float magnitude2 = a2.magnitude;
				Vector2 vector2 = a2 / magnitude2;
				if (magnitude2 < FireMortar.minimumDistance)
				{
					magnitude2 = FireMortar.minimumDistance;
				}
				float y = Trajectory.CalculateInitialYSpeed(FireMortar.timeToTarget, vector.y);
				float num = magnitude2 / FireMortar.timeToTarget;
				Vector3 direction = new Vector3(vector2.x * num, y, vector2.y * num);
				magnitude = direction.magnitude;
				ray.direction = direction;
			}
			for (int i = 0; i < FireMortar.mortarCount; i++)
			{
				Quaternion rotation = Util.QuaternionSafeLookRotation(ray.direction + ((i != 0) ? (UnityEngine.Random.insideUnitSphere * 0.05f) : Vector3.zero));
				ProjectileManager.instance.FireProjectile(FireMortar.mortarProjectilePrefab, ray.origin, rotation, base.gameObject, this.damageStat * FireMortar.mortarDamageCoefficient, 0f, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, magnitude);
			}
		}

		// Token: 0x060006BE RID: 1726 RVA: 0x0002046C File Offset: 0x0001E66C
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (this.stopwatch > this.duration)
			{
				Burrowed burrowed = new Burrowed();
				burrowed.duration = Burrowed.mortarCooldown;
				this.outer.SetNextState(burrowed);
			}
		}

		// Token: 0x060006BF RID: 1727 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04000844 RID: 2116
		public static GameObject mortarProjectilePrefab;

		// Token: 0x04000845 RID: 2117
		public static GameObject mortarMuzzleflashEffect;

		// Token: 0x04000846 RID: 2118
		public static int mortarCount;

		// Token: 0x04000847 RID: 2119
		public static string mortarMuzzleName;

		// Token: 0x04000848 RID: 2120
		public static string mortarSoundString;

		// Token: 0x04000849 RID: 2121
		public static float mortarDamageCoefficient;

		// Token: 0x0400084A RID: 2122
		public static float mortarProcCoefficient;

		// Token: 0x0400084B RID: 2123
		public static float baseDuration;

		// Token: 0x0400084C RID: 2124
		public static float timeToTarget = 3f;

		// Token: 0x0400084D RID: 2125
		public static float projectileVelocity = 55f;

		// Token: 0x0400084E RID: 2126
		public static float minimumDistance;

		// Token: 0x0400084F RID: 2127
		private float stopwatch;

		// Token: 0x04000850 RID: 2128
		private float duration;
	}
}
