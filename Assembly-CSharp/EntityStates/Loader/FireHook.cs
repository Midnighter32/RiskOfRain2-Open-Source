using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Loader
{
	// Token: 0x020007E5 RID: 2021
	public class FireHook : BaseSkillState
	{
		// Token: 0x06002E00 RID: 11776 RVA: 0x000C3D1C File Offset: 0x000C1F1C
		public override void OnEnter()
		{
			base.OnEnter();
			if (base.isAuthority)
			{
				Ray aimRay = base.GetAimRay();
				FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
				{
					position = aimRay.origin,
					rotation = Quaternion.LookRotation(aimRay.direction),
					crit = base.characterBody.RollCrit(),
					damage = this.damageStat * FireHook.damageCoefficient,
					force = 0f,
					damageColorIndex = DamageColorIndex.Default,
					procChainMask = default(ProcChainMask),
					projectilePrefab = this.projectilePrefab,
					owner = base.gameObject
				};
				ProjectileManager.instance.FireProjectile(fireProjectileInfo);
			}
			EffectManager.SimpleMuzzleFlash(FireHook.muzzleflashEffectPrefab, base.gameObject, "MuzzleLeft", false);
			Util.PlaySound(FireHook.fireSoundString, base.gameObject);
			base.PlayAnimation("Grapple", "FireHookIntro");
		}

		// Token: 0x06002E01 RID: 11777 RVA: 0x000C3E0E File Offset: 0x000C200E
		public void SetHookReference(GameObject hook)
		{
			this.hookInstance = hook;
			this.hookStickOnImpact = hook.GetComponent<ProjectileStickOnImpact>();
			this.hadHookInstance = true;
		}

		// Token: 0x06002E02 RID: 11778 RVA: 0x000C3E2C File Offset: 0x000C202C
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (this.hookStickOnImpact)
			{
				if (this.hookStickOnImpact.stuck && !this.isStuck)
				{
					base.PlayAnimation("Grapple", "FireHookLoop");
				}
				this.isStuck = this.hookStickOnImpact.stuck;
			}
			if (base.isAuthority && !this.hookInstance && this.hadHookInstance)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x06002E03 RID: 11779 RVA: 0x000C3EAA File Offset: 0x000C20AA
		public override void OnExit()
		{
			base.PlayAnimation("Grapple", "FireHookExit");
			EffectManager.SimpleMuzzleFlash(FireHook.muzzleflashEffectPrefab, base.gameObject, "MuzzleLeft", false);
			base.OnExit();
		}

		// Token: 0x06002E04 RID: 11780 RVA: 0x0000C5D3 File Offset: 0x0000A7D3
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Pain;
		}

		// Token: 0x04002B0F RID: 11023
		[SerializeField]
		public GameObject projectilePrefab;

		// Token: 0x04002B10 RID: 11024
		public static float damageCoefficient;

		// Token: 0x04002B11 RID: 11025
		public static GameObject muzzleflashEffectPrefab;

		// Token: 0x04002B12 RID: 11026
		public static string fireSoundString;

		// Token: 0x04002B13 RID: 11027
		public GameObject hookInstance;

		// Token: 0x04002B14 RID: 11028
		protected ProjectileStickOnImpact hookStickOnImpact;

		// Token: 0x04002B15 RID: 11029
		private bool isStuck;

		// Token: 0x04002B16 RID: 11030
		private bool hadHookInstance;

		// Token: 0x04002B17 RID: 11031
		private uint soundID;
	}
}
