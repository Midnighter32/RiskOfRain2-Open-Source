using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Drone.DroneWeapon
{
	// Token: 0x0200089E RID: 2206
	public class FireTwinRocket : BaseState
	{
		// Token: 0x06003177 RID: 12663 RVA: 0x000D5120 File Offset: 0x000D3320
		public override void OnEnter()
		{
			base.OnEnter();
			this.stopwatch = 0f;
			this.duration = FireTwinRocket.baseDuration / this.attackSpeedStat;
			base.GetAimRay();
			Transform modelTransform = base.GetModelTransform();
			if (modelTransform)
			{
				this.childLocator = modelTransform.GetComponent<ChildLocator>();
			}
			this.FireProjectile("GatLeft");
			this.FireProjectile("GatRight");
		}

		// Token: 0x06003178 RID: 12664 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06003179 RID: 12665 RVA: 0x000D5188 File Offset: 0x000D3388
		private void FireProjectile(string muzzleString)
		{
			base.GetAimRay();
			Transform transform = this.childLocator.FindChild(muzzleString);
			if (FireTwinRocket.muzzleEffectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(FireTwinRocket.muzzleEffectPrefab, base.gameObject, muzzleString, false);
			}
			if (base.isAuthority && FireTwinRocket.projectilePrefab != null)
			{
				float maxDistance = 1000f;
				Ray aimRay = base.GetAimRay();
				Vector3 forward = aimRay.direction;
				Vector3 position = aimRay.origin;
				if (transform)
				{
					position = transform.position;
					RaycastHit raycastHit;
					if (Physics.Raycast(aimRay, out raycastHit, maxDistance, LayerIndex.world.mask | LayerIndex.entityPrecise.mask))
					{
						forward = raycastHit.point - transform.position;
					}
				}
				ProjectileManager.instance.FireProjectile(FireTwinRocket.projectilePrefab, position, Util.QuaternionSafeLookRotation(forward), base.gameObject, this.damageStat * FireTwinRocket.damageCoefficient, FireTwinRocket.force, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
			}
		}

		// Token: 0x0600317A RID: 12666 RVA: 0x000D52A0 File Offset: 0x000D34A0
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (this.stopwatch >= this.duration / this.attackSpeedStat && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x0600317B RID: 12667 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04002FED RID: 12269
		public static GameObject projectilePrefab;

		// Token: 0x04002FEE RID: 12270
		public static GameObject muzzleEffectPrefab;

		// Token: 0x04002FEF RID: 12271
		public static float damageCoefficient;

		// Token: 0x04002FF0 RID: 12272
		public static float force;

		// Token: 0x04002FF1 RID: 12273
		public static float baseDuration = 2f;

		// Token: 0x04002FF2 RID: 12274
		private ChildLocator childLocator;

		// Token: 0x04002FF3 RID: 12275
		private float stopwatch;

		// Token: 0x04002FF4 RID: 12276
		private float duration;
	}
}
