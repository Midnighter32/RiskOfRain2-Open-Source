using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.AncientWispMonster
{
	// Token: 0x020000D6 RID: 214
	internal class FireRHCannon : BaseState
	{
		// Token: 0x06000437 RID: 1079 RVA: 0x00011674 File Offset: 0x0000F874
		public override void OnEnter()
		{
			base.OnEnter();
			Ray aimRay = base.GetAimRay();
			string text = "MuzzleRight";
			this.duration = FireRHCannon.baseDuration / this.attackSpeedStat;
			this.durationBetweenShots = FireRHCannon.baseDurationBetweenShots / this.attackSpeedStat;
			if (FireRHCannon.effectPrefab)
			{
				EffectManager.instance.SimpleMuzzleFlash(FireRHCannon.effectPrefab, base.gameObject, text, false);
			}
			base.PlayAnimation("Gesture", "FireRHCannon", "FireRHCannon.playbackRate", this.duration);
			if (base.isAuthority && base.modelLocator && base.modelLocator.modelTransform)
			{
				ChildLocator component = base.modelLocator.modelTransform.GetComponent<ChildLocator>();
				if (component)
				{
					Transform transform = component.FindChild(text);
					if (transform)
					{
						Vector3 forward = aimRay.direction;
						RaycastHit raycastHit;
						if (Physics.Raycast(aimRay, out raycastHit, (float)LayerIndex.world.mask))
						{
							forward = raycastHit.point - transform.position;
						}
						ProjectileManager.instance.FireProjectile(FireRHCannon.projectilePrefab, transform.position, Util.QuaternionSafeLookRotation(forward), base.gameObject, this.damageStat * FireRHCannon.damageCoefficient, FireRHCannon.force, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
					}
				}
			}
		}

		// Token: 0x06000438 RID: 1080 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06000439 RID: 1081 RVA: 0x000117E0 File Offset: 0x0000F9E0
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority)
			{
				if (this.bulletCountCurrent == FireRHCannon.bulletCount && base.fixedAge >= this.duration)
				{
					this.outer.SetNextStateToMain();
					return;
				}
				if (this.bulletCountCurrent < FireRHCannon.bulletCount && base.fixedAge >= this.durationBetweenShots)
				{
					FireRHCannon fireRHCannon = new FireRHCannon();
					fireRHCannon.bulletCountCurrent = this.bulletCountCurrent + 1;
					this.outer.SetNextState(fireRHCannon);
					return;
				}
			}
		}

		// Token: 0x0600043A RID: 1082 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x040003F0 RID: 1008
		public static GameObject projectilePrefab;

		// Token: 0x040003F1 RID: 1009
		public static GameObject effectPrefab;

		// Token: 0x040003F2 RID: 1010
		public static float baseDuration = 2f;

		// Token: 0x040003F3 RID: 1011
		public static float baseDurationBetweenShots = 0.5f;

		// Token: 0x040003F4 RID: 1012
		public static float damageCoefficient = 1.2f;

		// Token: 0x040003F5 RID: 1013
		public static float force = 20f;

		// Token: 0x040003F6 RID: 1014
		public static int bulletCount;

		// Token: 0x040003F7 RID: 1015
		private float duration;

		// Token: 0x040003F8 RID: 1016
		private float durationBetweenShots;

		// Token: 0x040003F9 RID: 1017
		public int bulletCountCurrent = 1;
	}
}
