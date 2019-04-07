using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.VagrantMonster.Weapon
{
	// Token: 0x02000130 RID: 304
	public class JellyBarrage : BaseState
	{
		// Token: 0x060005DD RID: 1501 RVA: 0x0001AC78 File Offset: 0x00018E78
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

		// Token: 0x060005DE RID: 1502 RVA: 0x0001AD18 File Offset: 0x00018F18
		private void FireBlob(Ray projectileRay, float bonusPitch, float bonusYaw)
		{
			projectileRay.direction = Util.ApplySpread(projectileRay.direction, 0f, JellyBarrage.maxSpread, 1f, 1f, bonusYaw, bonusPitch);
			ProjectileManager.instance.FireProjectile(JellyBarrage.projectilePrefab, projectileRay.origin, Util.QuaternionSafeLookRotation(projectileRay.direction), base.gameObject, this.damageStat * JellyBarrage.damageCoefficient, 0f, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
		}

		// Token: 0x060005DF RID: 1503 RVA: 0x0001ADA4 File Offset: 0x00018FA4
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

		// Token: 0x040006B8 RID: 1720
		private float stopwatch;

		// Token: 0x040006B9 RID: 1721
		private float missileStopwatch;

		// Token: 0x040006BA RID: 1722
		public static float baseDuration;

		// Token: 0x040006BB RID: 1723
		public static string muzzleString;

		// Token: 0x040006BC RID: 1724
		public static float missileSpawnFrequency;

		// Token: 0x040006BD RID: 1725
		public static float missileSpawnDelay;

		// Token: 0x040006BE RID: 1726
		public static float damageCoefficient;

		// Token: 0x040006BF RID: 1727
		public static float maxSpread;

		// Token: 0x040006C0 RID: 1728
		public static GameObject projectilePrefab;

		// Token: 0x040006C1 RID: 1729
		public static GameObject muzzleflashPrefab;

		// Token: 0x040006C2 RID: 1730
		private ChildLocator childLocator;
	}
}
