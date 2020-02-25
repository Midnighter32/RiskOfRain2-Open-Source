using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.GreaterWispMonster
{
	// Token: 0x0200072C RID: 1836
	public class FireCannons : BaseState
	{
		// Token: 0x06002AA7 RID: 10919 RVA: 0x000B36B4 File Offset: 0x000B18B4
		public override void OnEnter()
		{
			base.OnEnter();
			Ray aimRay = base.GetAimRay();
			string text = "MuzzleLeft";
			string text2 = "MuzzleRight";
			this.duration = FireCannons.baseDuration / this.attackSpeedStat;
			if (this.effectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(this.effectPrefab, base.gameObject, text, false);
				EffectManager.SimpleMuzzleFlash(this.effectPrefab, base.gameObject, text2, false);
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
						ProjectileManager.instance.FireProjectile(this.projectilePrefab, transform.position, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * this.damageCoefficient, this.force, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
					}
					if (transform2)
					{
						ProjectileManager.instance.FireProjectile(this.projectilePrefab, transform2.position, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * this.damageCoefficient, this.force, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
					}
				}
			}
		}

		// Token: 0x06002AA8 RID: 10920 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06002AA9 RID: 10921 RVA: 0x000B3870 File Offset: 0x000B1A70
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06002AAA RID: 10922 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04002683 RID: 9859
		[SerializeField]
		public GameObject projectilePrefab;

		// Token: 0x04002684 RID: 9860
		[SerializeField]
		public GameObject effectPrefab;

		// Token: 0x04002685 RID: 9861
		public static float baseDuration = 2f;

		// Token: 0x04002686 RID: 9862
		[SerializeField]
		public float damageCoefficient = 1.2f;

		// Token: 0x04002687 RID: 9863
		[SerializeField]
		public float force = 20f;

		// Token: 0x04002688 RID: 9864
		private float duration;
	}
}
