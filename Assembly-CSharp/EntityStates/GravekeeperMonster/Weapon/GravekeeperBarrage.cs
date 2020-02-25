using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.GravekeeperMonster.Weapon
{
	// Token: 0x0200084B RID: 2123
	public class GravekeeperBarrage : BaseState
	{
		// Token: 0x06003005 RID: 12293 RVA: 0x000CDE58 File Offset: 0x000CC058
		public override void OnEnter()
		{
			base.OnEnter();
			this.missileStopwatch -= GravekeeperBarrage.missileSpawnDelay;
			Transform modelTransform = base.GetModelTransform();
			if (modelTransform)
			{
				this.childLocator = modelTransform.GetComponent<ChildLocator>();
				if (this.childLocator)
				{
					this.childLocator.FindChild("JarEffectLoop").gameObject.SetActive(true);
				}
			}
			base.PlayAnimation("Jar, Override", "BeginGravekeeperBarrage");
			EffectManager.SimpleMuzzleFlash(GravekeeperBarrage.jarOpenEffectPrefab, base.gameObject, GravekeeperBarrage.jarEffectChildLocatorString, false);
			Util.PlaySound(GravekeeperBarrage.jarOpenSoundString, base.gameObject);
			base.characterBody.SetAimTimer(GravekeeperBarrage.baseDuration + 2f);
		}

		// Token: 0x06003006 RID: 12294 RVA: 0x000CDF10 File Offset: 0x000CC110
		private void FireBlob(Ray projectileRay, float bonusPitch, float bonusYaw)
		{
			projectileRay.direction = Util.ApplySpread(projectileRay.direction, 0f, GravekeeperBarrage.maxSpread, 1f, 1f, bonusYaw, bonusPitch);
			EffectManager.SimpleMuzzleFlash(GravekeeperBarrage.muzzleflashPrefab, base.gameObject, GravekeeperBarrage.muzzleString, false);
			if (NetworkServer.active)
			{
				ProjectileManager.instance.FireProjectile(GravekeeperBarrage.projectilePrefab, projectileRay.origin, Util.QuaternionSafeLookRotation(projectileRay.direction), base.gameObject, this.damageStat * GravekeeperBarrage.damageCoefficient, GravekeeperBarrage.missileForce, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
			}
		}

		// Token: 0x06003007 RID: 12295 RVA: 0x000CDFBC File Offset: 0x000CC1BC
		public override void OnExit()
		{
			base.PlayCrossfade("Jar, Override", "EndGravekeeperBarrage", 0.06f);
			EffectManager.SimpleMuzzleFlash(GravekeeperBarrage.jarCloseEffectPrefab, base.gameObject, GravekeeperBarrage.jarEffectChildLocatorString, false);
			Util.PlaySound(GravekeeperBarrage.jarCloseSoundString, base.gameObject);
			if (this.childLocator)
			{
				this.childLocator.FindChild("JarEffectLoop").gameObject.SetActive(false);
			}
			base.OnExit();
		}

		// Token: 0x06003008 RID: 12296 RVA: 0x000CE034 File Offset: 0x000CC234
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			this.missileStopwatch += Time.fixedDeltaTime;
			if (this.missileStopwatch >= 1f / GravekeeperBarrage.missileSpawnFrequency)
			{
				this.missileStopwatch -= 1f / GravekeeperBarrage.missileSpawnFrequency;
				Transform transform = this.childLocator.FindChild(GravekeeperBarrage.muzzleString);
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
			if (this.stopwatch >= GravekeeperBarrage.baseDuration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x04002DE1 RID: 11745
		private float stopwatch;

		// Token: 0x04002DE2 RID: 11746
		private float missileStopwatch;

		// Token: 0x04002DE3 RID: 11747
		public static float baseDuration;

		// Token: 0x04002DE4 RID: 11748
		public static string muzzleString;

		// Token: 0x04002DE5 RID: 11749
		public static float missileSpawnFrequency;

		// Token: 0x04002DE6 RID: 11750
		public static float missileSpawnDelay;

		// Token: 0x04002DE7 RID: 11751
		public static float missileForce;

		// Token: 0x04002DE8 RID: 11752
		public static float damageCoefficient;

		// Token: 0x04002DE9 RID: 11753
		public static float maxSpread;

		// Token: 0x04002DEA RID: 11754
		public static GameObject projectilePrefab;

		// Token: 0x04002DEB RID: 11755
		public static GameObject muzzleflashPrefab;

		// Token: 0x04002DEC RID: 11756
		public static string jarEffectChildLocatorString;

		// Token: 0x04002DED RID: 11757
		public static string jarOpenSoundString;

		// Token: 0x04002DEE RID: 11758
		public static string jarCloseSoundString;

		// Token: 0x04002DEF RID: 11759
		public static GameObject jarOpenEffectPrefab;

		// Token: 0x04002DF0 RID: 11760
		public static GameObject jarCloseEffectPrefab;

		// Token: 0x04002DF1 RID: 11761
		private ChildLocator childLocator;
	}
}
