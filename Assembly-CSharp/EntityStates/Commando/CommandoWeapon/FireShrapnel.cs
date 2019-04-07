using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Commando.CommandoWeapon
{
	// Token: 0x020001AD RID: 429
	internal class FireShrapnel : BaseState
	{
		// Token: 0x06000864 RID: 2148 RVA: 0x0002A184 File Offset: 0x00028384
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FireShrapnel.baseDuration / this.attackSpeedStat;
			this.modifiedAimRay = base.GetAimRay();
			Util.PlaySound(FireShrapnel.attackSoundString, base.gameObject);
			string muzzleName = "MuzzleLaser";
			if (base.characterBody)
			{
				base.characterBody.SetAimTimer(2f);
			}
			base.PlayAnimation("Gesture", "FireLaser", "FireLaser.playbackRate", this.duration);
			if (FireShrapnel.effectPrefab)
			{
				EffectManager.instance.SimpleMuzzleFlash(FireShrapnel.effectPrefab, base.gameObject, muzzleName, false);
			}
			if (base.isAuthority)
			{
				RaycastHit raycastHit;
				if (Physics.Raycast(this.modifiedAimRay, out raycastHit, 1000f, LayerIndex.world.mask | LayerIndex.defaultLayer.mask))
				{
					new BlastAttack
					{
						attacker = base.gameObject,
						inflictor = base.gameObject,
						teamIndex = TeamComponent.GetObjectTeam(base.gameObject),
						baseDamage = this.damageStat * FireShrapnel.damageCoefficient,
						baseForce = FireShrapnel.force * 0.2f,
						position = raycastHit.point,
						radius = FireShrapnel.blastRadius
					}.Fire();
				}
				new BulletAttack
				{
					owner = base.gameObject,
					weapon = base.gameObject,
					origin = this.modifiedAimRay.origin,
					aimVector = this.modifiedAimRay.direction,
					radius = 0.25f,
					minSpread = FireShrapnel.minSpread,
					maxSpread = FireShrapnel.maxSpread,
					bulletCount = (uint)((FireShrapnel.bulletCount > 0) ? FireShrapnel.bulletCount : 0),
					damage = 0f,
					procCoefficient = 0f,
					force = FireShrapnel.force,
					tracerEffectPrefab = FireShrapnel.tracerEffectPrefab,
					muzzleName = muzzleName,
					hitEffectPrefab = FireShrapnel.hitEffectPrefab,
					isCrit = Util.CheckRoll(this.critStat, base.characterBody.master)
				}.Fire();
			}
		}

		// Token: 0x06000865 RID: 2149 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06000866 RID: 2150 RVA: 0x0002A3AE File Offset: 0x000285AE
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06000867 RID: 2151 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04000B32 RID: 2866
		public static GameObject effectPrefab;

		// Token: 0x04000B33 RID: 2867
		public static GameObject hitEffectPrefab;

		// Token: 0x04000B34 RID: 2868
		public static GameObject tracerEffectPrefab;

		// Token: 0x04000B35 RID: 2869
		public static float damageCoefficient;

		// Token: 0x04000B36 RID: 2870
		public static float blastRadius;

		// Token: 0x04000B37 RID: 2871
		public static float force;

		// Token: 0x04000B38 RID: 2872
		public static float minSpread;

		// Token: 0x04000B39 RID: 2873
		public static float maxSpread;

		// Token: 0x04000B3A RID: 2874
		public static int bulletCount;

		// Token: 0x04000B3B RID: 2875
		public static float baseDuration = 2f;

		// Token: 0x04000B3C RID: 2876
		public static string attackSoundString;

		// Token: 0x04000B3D RID: 2877
		public static float maxDistance;

		// Token: 0x04000B3E RID: 2878
		private float duration;

		// Token: 0x04000B3F RID: 2879
		private Ray modifiedAimRay;
	}
}
