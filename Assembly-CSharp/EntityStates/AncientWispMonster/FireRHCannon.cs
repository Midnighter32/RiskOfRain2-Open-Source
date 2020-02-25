using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.AncientWispMonster
{
	// Token: 0x02000735 RID: 1845
	public class FireRHCannon : BaseState
	{
		// Token: 0x06002AE0 RID: 10976 RVA: 0x000B46BC File Offset: 0x000B28BC
		public override void OnEnter()
		{
			base.OnEnter();
			Ray aimRay = base.GetAimRay();
			string text = "MuzzleRight";
			this.duration = FireRHCannon.baseDuration / this.attackSpeedStat;
			this.durationBetweenShots = FireRHCannon.baseDurationBetweenShots / this.attackSpeedStat;
			if (FireRHCannon.effectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(FireRHCannon.effectPrefab, base.gameObject, text, false);
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

		// Token: 0x06002AE1 RID: 10977 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06002AE2 RID: 10978 RVA: 0x000B4824 File Offset: 0x000B2A24
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

		// Token: 0x06002AE3 RID: 10979 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x040026B6 RID: 9910
		public static GameObject projectilePrefab;

		// Token: 0x040026B7 RID: 9911
		public static GameObject effectPrefab;

		// Token: 0x040026B8 RID: 9912
		public static float baseDuration = 2f;

		// Token: 0x040026B9 RID: 9913
		public static float baseDurationBetweenShots = 0.5f;

		// Token: 0x040026BA RID: 9914
		public static float damageCoefficient = 1.2f;

		// Token: 0x040026BB RID: 9915
		public static float force = 20f;

		// Token: 0x040026BC RID: 9916
		public static int bulletCount;

		// Token: 0x040026BD RID: 9917
		private float duration;

		// Token: 0x040026BE RID: 9918
		private float durationBetweenShots;

		// Token: 0x040026BF RID: 9919
		public int bulletCountCurrent = 1;
	}
}
