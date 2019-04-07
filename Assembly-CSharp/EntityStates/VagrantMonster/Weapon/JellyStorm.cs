using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.VagrantMonster.Weapon
{
	// Token: 0x02000131 RID: 305
	public class JellyStorm : BaseState
	{
		// Token: 0x060005E1 RID: 1505 RVA: 0x0001AEC0 File Offset: 0x000190C0
		public override void OnEnter()
		{
			base.OnEnter();
			this.missileStopwatch -= JellyStorm.missileSpawnDelay;
			if (base.sfxLocator && base.sfxLocator.barkSound != "")
			{
				Util.PlaySound(base.sfxLocator.barkSound, base.gameObject);
			}
			base.PlayAnimation("Gesture", "StormEnter");
			Transform modelTransform = base.GetModelTransform();
			if (modelTransform)
			{
				this.childLocator = modelTransform.GetComponent<ChildLocator>();
				if (this.childLocator)
				{
					this.childLocator.FindChild(JellyStorm.stormPointChildString);
				}
			}
		}

		// Token: 0x060005E2 RID: 1506 RVA: 0x0001AF70 File Offset: 0x00019170
		private void FireBlob(Ray aimRay, float bonusPitch, float bonusYaw, float speed)
		{
			Vector3 forward = Util.ApplySpread(aimRay.direction, 0f, 0f, 1f, 1f, bonusYaw, bonusPitch);
			ProjectileManager.instance.FireProjectile(JellyStorm.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(forward), base.gameObject, this.damageStat * JellyStorm.damageCoefficient, 0f, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, speed);
		}

		// Token: 0x060005E3 RID: 1507 RVA: 0x0001AFF0 File Offset: 0x000191F0
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			this.missileStopwatch += Time.fixedDeltaTime;
			if (this.missileStopwatch >= 1f / JellyStorm.missileSpawnFrequency && !this.beginExitTransition)
			{
				this.missileStopwatch -= 1f / JellyStorm.missileSpawnFrequency;
				Transform transform = this.childLocator.FindChild(JellyStorm.stormPointChildString);
				if (transform)
				{
					for (int i = 0; i < JellyStorm.missileTurretCount; i++)
					{
						float bonusYaw = 360f / (float)JellyStorm.missileTurretCount * (float)i + 360f * JellyStorm.missileTurretYawFrequency * this.stopwatch;
						float bonusPitch = Mathf.Sin(6.2831855f * JellyStorm.missileTurretPitchFrequency * this.stopwatch) * JellyStorm.missileTurretPitchMagnitude;
						this.FireBlob(new Ray
						{
							origin = transform.position,
							direction = transform.transform.forward
						}, bonusPitch, bonusYaw, JellyStorm.missileSpeed);
					}
				}
			}
			if (this.stopwatch >= JellyStorm.stormDuration - JellyStorm.stormToIdleTransitionDuration && !this.beginExitTransition)
			{
				this.beginExitTransition = true;
				base.PlayCrossfade("Gesture", "StormExit", "StormExit.playbackRate", JellyStorm.stormToIdleTransitionDuration, 0.5f);
			}
			if (this.stopwatch >= JellyStorm.stormDuration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x040006C3 RID: 1731
		private float stopwatch;

		// Token: 0x040006C4 RID: 1732
		private float missileStopwatch;

		// Token: 0x040006C5 RID: 1733
		public static float stormDuration;

		// Token: 0x040006C6 RID: 1734
		public static float stormToIdleTransitionDuration;

		// Token: 0x040006C7 RID: 1735
		public static string stormPointChildString;

		// Token: 0x040006C8 RID: 1736
		public static float missileSpawnFrequency;

		// Token: 0x040006C9 RID: 1737
		public static float missileSpawnDelay;

		// Token: 0x040006CA RID: 1738
		public static int missileTurretCount;

		// Token: 0x040006CB RID: 1739
		public static float missileTurretYawFrequency;

		// Token: 0x040006CC RID: 1740
		public static float missileTurretPitchFrequency;

		// Token: 0x040006CD RID: 1741
		public static float missileTurretPitchMagnitude;

		// Token: 0x040006CE RID: 1742
		public static float missileSpeed;

		// Token: 0x040006CF RID: 1743
		public static float damageCoefficient;

		// Token: 0x040006D0 RID: 1744
		public static GameObject projectilePrefab;

		// Token: 0x040006D1 RID: 1745
		public static GameObject effectPrefab;

		// Token: 0x040006D2 RID: 1746
		private bool beginExitTransition;

		// Token: 0x040006D3 RID: 1747
		private ChildLocator childLocator;
	}
}
