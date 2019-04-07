using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.GreaterWispMonster
{
	// Token: 0x020000CD RID: 205
	internal class FireCannons : BaseState
	{
		// Token: 0x060003FF RID: 1023 RVA: 0x00010694 File Offset: 0x0000E894
		public override void OnEnter()
		{
			base.OnEnter();
			Ray aimRay = base.GetAimRay();
			string text = "MuzzleLeft";
			string text2 = "MuzzleRight";
			this.duration = FireCannons.baseDuration / this.attackSpeedStat;
			if (FireCannons.effectPrefab)
			{
				EffectManager.instance.SimpleMuzzleFlash(FireCannons.effectPrefab, base.gameObject, text, false);
				EffectManager.instance.SimpleMuzzleFlash(FireCannons.effectPrefab, base.gameObject, text2, false);
			}
			base.PlayAnimation("Gesture", "FireCannons", "FireCannons.playbackRate", this.duration);
			if (base.isAuthority && base.modelLocator && base.modelLocator.modelTransform)
			{
				ChildLocator component = base.modelLocator.modelTransform.GetComponent<ChildLocator>();
				if (component)
				{
					int childIndex = component.FindChildIndex(text);
					int childIndex2 = component.FindChildIndex(text2);
					Transform transform = component.FindChild(childIndex);
					Transform transform2 = component.FindChild(childIndex2);
					if (transform)
					{
						ProjectileManager.instance.FireProjectile(FireCannons.projectilePrefab, transform.position, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * FireCannons.damageCoefficient, FireCannons.force, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
					}
					if (transform)
					{
						ProjectileManager.instance.FireProjectile(FireCannons.projectilePrefab, transform2.position, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * FireCannons.damageCoefficient, FireCannons.force, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
					}
				}
			}
		}

		// Token: 0x06000400 RID: 1024 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06000401 RID: 1025 RVA: 0x00010851 File Offset: 0x0000EA51
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06000402 RID: 1026 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x040003BE RID: 958
		public static GameObject projectilePrefab;

		// Token: 0x040003BF RID: 959
		public static GameObject effectPrefab;

		// Token: 0x040003C0 RID: 960
		public static float baseDuration = 2f;

		// Token: 0x040003C1 RID: 961
		public static float damageCoefficient = 1.2f;

		// Token: 0x040003C2 RID: 962
		public static float force = 20f;

		// Token: 0x040003C3 RID: 963
		private float duration;
	}
}
