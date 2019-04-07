using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Huntress.HuntressWeapon
{
	// Token: 0x02000154 RID: 340
	internal class FireGlaive : BaseState
	{
		// Token: 0x06000690 RID: 1680 RVA: 0x0001F41C File Offset: 0x0001D61C
		public override void OnEnter()
		{
			base.OnEnter();
			Ray aimRay = base.GetAimRay();
			base.StartAimMode(aimRay, 2f, false);
			Transform modelTransform = base.GetModelTransform();
			this.duration = FireGlaive.baseDuration / this.attackSpeedStat;
			base.PlayAnimation("Gesture", "FireGlaive", "FireGlaive.playbackRate", this.duration);
			Vector3 position = aimRay.origin;
			Quaternion rotation = Util.QuaternionSafeLookRotation(aimRay.direction);
			if (modelTransform)
			{
				ChildLocator component = modelTransform.GetComponent<ChildLocator>();
				if (component)
				{
					Transform transform = component.FindChild("RightHand");
					if (transform)
					{
						position = transform.position;
					}
				}
			}
			if (base.hasAuthority)
			{
				ProjectileManager.instance.FireProjectile(FireGlaive.projectilePrefab, position, rotation, base.gameObject, this.damageStat * FireGlaive.damageCoefficient, FireGlaive.force, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
			}
		}

		// Token: 0x06000691 RID: 1681 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06000692 RID: 1682 RVA: 0x0001F511 File Offset: 0x0001D711
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06000693 RID: 1683 RVA: 0x0000B306 File Offset: 0x00009506
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04000800 RID: 2048
		public static GameObject projectilePrefab;

		// Token: 0x04000801 RID: 2049
		public static float baseDuration = 2f;

		// Token: 0x04000802 RID: 2050
		public static float damageCoefficient = 1.2f;

		// Token: 0x04000803 RID: 2051
		public static float force = 20f;

		// Token: 0x04000804 RID: 2052
		private float duration;
	}
}
