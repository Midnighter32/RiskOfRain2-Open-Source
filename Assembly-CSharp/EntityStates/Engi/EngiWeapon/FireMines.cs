using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Engi.EngiWeapon
{
	// Token: 0x02000887 RID: 2183
	public class FireMines : BaseState
	{
		// Token: 0x06003121 RID: 12577 RVA: 0x000D3758 File Offset: 0x000D1958
		public override void OnEnter()
		{
			base.OnEnter();
			Util.PlaySound(FireMines.throwMineSoundString, base.gameObject);
			this.duration = FireMines.baseDuration / this.attackSpeedStat;
			Ray aimRay = base.GetAimRay();
			base.StartAimMode(aimRay, 2f, false);
			if (base.GetModelAnimator())
			{
				float num = this.duration * 0.3f;
				base.PlayCrossfade("Gesture, Additive", "FireMineRight", "FireMine.playbackRate", this.duration + num, 0.05f);
			}
			string muzzleName = "MuzzleCenter";
			if (FireMines.effectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(FireMines.effectPrefab, base.gameObject, muzzleName, false);
			}
			if (base.isAuthority)
			{
				ProjectileManager.instance.FireProjectile(this.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * this.damageCoefficient, this.force, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
			}
		}

		// Token: 0x06003122 RID: 12578 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06003123 RID: 12579 RVA: 0x000D3863 File Offset: 0x000D1A63
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06003124 RID: 12580 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04002F63 RID: 12131
		public static GameObject effectPrefab;

		// Token: 0x04002F64 RID: 12132
		public static GameObject hitEffectPrefab;

		// Token: 0x04002F65 RID: 12133
		[SerializeField]
		public GameObject projectilePrefab;

		// Token: 0x04002F66 RID: 12134
		[SerializeField]
		public float damageCoefficient;

		// Token: 0x04002F67 RID: 12135
		[SerializeField]
		public float force;

		// Token: 0x04002F68 RID: 12136
		public static float baseDuration = 2f;

		// Token: 0x04002F69 RID: 12137
		public static string throwMineSoundString;

		// Token: 0x04002F6A RID: 12138
		private float duration;
	}
}
