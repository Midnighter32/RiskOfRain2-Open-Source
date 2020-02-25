using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.VagrantMonster.Weapon
{
	// Token: 0x02000806 RID: 2054
	public class JellyBarrage : BaseState
	{
		// Token: 0x06002EB4 RID: 11956 RVA: 0x000C6920 File Offset: 0x000C4B20
		public override void OnEnter()
		{
			base.OnEnter();
			this.missileStopwatch -= JellyBarrage.missileSpawnDelay;
			if (base.sfxLocator && base.sfxLocator.barkSound != "")
			{
				Util.PlaySound(base.sfxLocator.barkSound, base.gameObject);
			}
			Transform modelTransform = base.GetModelTransform();
			if (modelTransform)
			{
				this.childLocator = modelTransform.GetComponent<ChildLocator>();
				if (this.childLocator)
				{
					this.childLocator.FindChild(JellyBarrage.muzzleString);
				}
			}
		}

		// Token: 0x06002EB5 RID: 11957 RVA: 0x000C69C0 File Offset: 0x000C4BC0
		private void FireBlob(Ray projectileRay, float bonusPitch, float bonusYaw)
		{
			projectileRay.direction = Util.ApplySpread(projectileRay.direction, 0f, JellyBarrage.maxSpread, 1f, 1f, bonusYaw, bonusPitch);
			ProjectileManager.instance.FireProjectile(JellyBarrage.projectilePrefab, projectileRay.origin, Util.QuaternionSafeLookRotation(projectileRay.direction), base.gameObject, this.damageStat * JellyBarrage.damageCoefficient, 0f, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
		}

		// Token: 0x06002EB6 RID: 11958 RVA: 0x000C6A4C File Offset: 0x000C4C4C
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			this.missileStopwatch += Time.fixedDeltaTime;
			if (this.missileStopwatch >= 1f / JellyBarrage.missileSpawnFrequency)
			{
				this.missileStopwatch -= 1f / JellyBarrage.missileSpawnFrequency;
				Transform transform = this.childLocator.FindChild(JellyBarrage.muzzleString);
				if (transform)
				{
					Ray projectileRay = default(Ray);
					projectileRay.origin = transform.position;
					projectileRay.direction = base.GetAimRay().direction;
					float maxDistance = 1000f;
					RaycastHit raycastHit;
					if (Physics.Raycast(base.GetAimRay(), out raycastHit, maxDistance, LayerIndex.world.mask))
					{
						projectileRay.direction = raycastHit.point - transform.position;
					}
					this.FireBlob(projectileRay, 0f, 0f);
				}
			}
			if (this.stopwatch >= JellyBarrage.baseDuration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x04002BDC RID: 11228
		private float stopwatch;

		// Token: 0x04002BDD RID: 11229
		private float missileStopwatch;

		// Token: 0x04002BDE RID: 11230
		public static float baseDuration;

		// Token: 0x04002BDF RID: 11231
		public static string muzzleString;

		// Token: 0x04002BE0 RID: 11232
		public static float missileSpawnFrequency;

		// Token: 0x04002BE1 RID: 11233
		public static float missileSpawnDelay;

		// Token: 0x04002BE2 RID: 11234
		public static float damageCoefficient;

		// Token: 0x04002BE3 RID: 11235
		public static float maxSpread;

		// Token: 0x04002BE4 RID: 11236
		public static GameObject projectilePrefab;

		// Token: 0x04002BE5 RID: 11237
		public static GameObject muzzleflashPrefab;

		// Token: 0x04002BE6 RID: 11238
		private ChildLocator childLocator;
	}
}
