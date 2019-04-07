using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Drone.DroneWeapon
{
	// Token: 0x02000199 RID: 409
	internal class FireTwinRocket : BaseState
	{
		// Token: 0x060007E2 RID: 2018 RVA: 0x00027154 File Offset: 0x00025354
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

		// Token: 0x060007E3 RID: 2019 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x060007E4 RID: 2020 RVA: 0x000271BC File Offset: 0x000253BC
		private void FireProjectile(string muzzleString)
		{
			base.GetAimRay();
			Transform transform = this.childLocator.FindChild(muzzleString);
			if (FireTwinRocket.muzzleEffectPrefab)
			{
				EffectManager.instance.SimpleMuzzleFlash(FireTwinRocket.muzzleEffectPrefab, base.gameObject, muzzleString, false);
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

		// Token: 0x060007E5 RID: 2021 RVA: 0x000272DC File Offset: 0x000254DC
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

		// Token: 0x060007E6 RID: 2022 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04000A4E RID: 2638
		public static GameObject projectilePrefab;

		// Token: 0x04000A4F RID: 2639
		public static GameObject muzzleEffectPrefab;

		// Token: 0x04000A50 RID: 2640
		public static float damageCoefficient;

		// Token: 0x04000A51 RID: 2641
		public static float force;

		// Token: 0x04000A52 RID: 2642
		public static float baseDuration = 2f;

		// Token: 0x04000A53 RID: 2643
		private ChildLocator childLocator;

		// Token: 0x04000A54 RID: 2644
		private float stopwatch;

		// Token: 0x04000A55 RID: 2645
		private float duration;
	}
}
