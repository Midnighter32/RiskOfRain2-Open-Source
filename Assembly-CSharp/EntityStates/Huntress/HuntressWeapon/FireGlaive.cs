using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Huntress.HuntressWeapon
{
	// Token: 0x02000836 RID: 2102
	public class FireGlaive : BaseState
	{
		// Token: 0x06002F91 RID: 12177 RVA: 0x000CBAD8 File Offset: 0x000C9CD8
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
			if (base.isAuthority)
			{
				ProjectileManager.instance.FireProjectile(FireGlaive.projectilePrefab, position, rotation, base.gameObject, this.damageStat * FireGlaive.damageCoefficient, FireGlaive.force, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
			}
		}

		// Token: 0x06002F92 RID: 12178 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06002F93 RID: 12179 RVA: 0x000CBBCD File Offset: 0x000C9DCD
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06002F94 RID: 12180 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04002D46 RID: 11590
		public static GameObject projectilePrefab;

		// Token: 0x04002D47 RID: 11591
		public static float baseDuration = 2f;

		// Token: 0x04002D48 RID: 11592
		public static float damageCoefficient = 1.2f;

		// Token: 0x04002D49 RID: 11593
		public static float force = 20f;

		// Token: 0x04002D4A RID: 11594
		private float duration;
	}
}
