using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.VagrantMonster.Weapon
{
	// Token: 0x02000807 RID: 2055
	public class JellyStorm : BaseState
	{
		// Token: 0x06002EB8 RID: 11960 RVA: 0x000C6B68 File Offset: 0x000C4D68
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

		// Token: 0x06002EB9 RID: 11961 RVA: 0x000C6C18 File Offset: 0x000C4E18
		private void FireBlob(Ray aimRay, float bonusPitch, float bonusYaw, float speed)
		{
			Vector3 forward = Util.ApplySpread(aimRay.direction, 0f, 0f, 1f, 1f, bonusYaw, bonusPitch);
			ProjectileManager.instance.FireProjectile(JellyStorm.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(forward), base.gameObject, this.damageStat * JellyStorm.damageCoefficient, 0f, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, speed);
		}

		// Token: 0x06002EBA RID: 11962 RVA: 0x000C6C98 File Offset: 0x000C4E98
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

		// Token: 0x04002BE7 RID: 11239
		private float stopwatch;

		// Token: 0x04002BE8 RID: 11240
		private float missileStopwatch;

		// Token: 0x04002BE9 RID: 11241
		public static float stormDuration;

		// Token: 0x04002BEA RID: 11242
		public static float stormToIdleTransitionDuration;

		// Token: 0x04002BEB RID: 11243
		public static string stormPointChildString;

		// Token: 0x04002BEC RID: 11244
		public static float missileSpawnFrequency;

		// Token: 0x04002BED RID: 11245
		public static float missileSpawnDelay;

		// Token: 0x04002BEE RID: 11246
		public static int missileTurretCount;

		// Token: 0x04002BEF RID: 11247
		public static float missileTurretYawFrequency;

		// Token: 0x04002BF0 RID: 11248
		public static float missileTurretPitchFrequency;

		// Token: 0x04002BF1 RID: 11249
		public static float missileTurretPitchMagnitude;

		// Token: 0x04002BF2 RID: 11250
		public static float missileSpeed;

		// Token: 0x04002BF3 RID: 11251
		public static float damageCoefficient;

		// Token: 0x04002BF4 RID: 11252
		public static GameObject projectilePrefab;

		// Token: 0x04002BF5 RID: 11253
		public static GameObject effectPrefab;

		// Token: 0x04002BF6 RID: 11254
		private bool beginExitTransition;

		// Token: 0x04002BF7 RID: 11255
		private ChildLocator childLocator;
	}
}
