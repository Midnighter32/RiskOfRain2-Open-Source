using System;
using System.Linq;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.HermitCrab
{
	// Token: 0x0200083D RID: 2109
	public class FireMortar : BaseState
	{
		// Token: 0x06002FBD RID: 12221 RVA: 0x000CC858 File Offset: 0x000CAA58
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FireMortar.baseDuration / this.attackSpeedStat;
			base.PlayCrossfade("Gesture, Additive", "FireMortar", 0f);
			Util.PlaySound(FireMortar.mortarSoundString, base.gameObject);
			EffectManager.SimpleMuzzleFlash(FireMortar.mortarMuzzleflashEffect, base.gameObject, FireMortar.mortarMuzzleName, false);
			if (base.isAuthority)
			{
				this.Fire();
			}
		}

		// Token: 0x06002FBE RID: 12222 RVA: 0x000CC8C8 File Offset: 0x000CAAC8
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

		// Token: 0x06002FBF RID: 12223 RVA: 0x000CCAF4 File Offset: 0x000CACF4
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

		// Token: 0x06002FC0 RID: 12224 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04002D8A RID: 11658
		public static GameObject mortarProjectilePrefab;

		// Token: 0x04002D8B RID: 11659
		public static GameObject mortarMuzzleflashEffect;

		// Token: 0x04002D8C RID: 11660
		public static int mortarCount;

		// Token: 0x04002D8D RID: 11661
		public static string mortarMuzzleName;

		// Token: 0x04002D8E RID: 11662
		public static string mortarSoundString;

		// Token: 0x04002D8F RID: 11663
		public static float mortarDamageCoefficient;

		// Token: 0x04002D90 RID: 11664
		public static float baseDuration;

		// Token: 0x04002D91 RID: 11665
		public static float timeToTarget = 3f;

		// Token: 0x04002D92 RID: 11666
		public static float projectileVelocity = 55f;

		// Token: 0x04002D93 RID: 11667
		public static float minimumDistance;

		// Token: 0x04002D94 RID: 11668
		private float stopwatch;

		// Token: 0x04002D95 RID: 11669
		private float duration;
	}
}
