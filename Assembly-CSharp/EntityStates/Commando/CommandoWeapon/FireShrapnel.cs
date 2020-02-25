using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Commando.CommandoWeapon
{
	// Token: 0x020008C1 RID: 2241
	public class FireShrapnel : BaseState
	{
		// Token: 0x06003241 RID: 12865 RVA: 0x000D92DC File Offset: 0x000D74DC
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
				EffectManager.SimpleMuzzleFlash(FireShrapnel.effectPrefab, base.gameObject, muzzleName, false);
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

		// Token: 0x06003242 RID: 12866 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06003243 RID: 12867 RVA: 0x000D9502 File Offset: 0x000D7702
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06003244 RID: 12868 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x0400311A RID: 12570
		public static GameObject effectPrefab;

		// Token: 0x0400311B RID: 12571
		public static GameObject hitEffectPrefab;

		// Token: 0x0400311C RID: 12572
		public static GameObject tracerEffectPrefab;

		// Token: 0x0400311D RID: 12573
		public static float damageCoefficient;

		// Token: 0x0400311E RID: 12574
		public static float blastRadius;

		// Token: 0x0400311F RID: 12575
		public static float force;

		// Token: 0x04003120 RID: 12576
		public static float minSpread;

		// Token: 0x04003121 RID: 12577
		public static float maxSpread;

		// Token: 0x04003122 RID: 12578
		public static int bulletCount;

		// Token: 0x04003123 RID: 12579
		public static float baseDuration = 2f;

		// Token: 0x04003124 RID: 12580
		public static string attackSoundString;

		// Token: 0x04003125 RID: 12581
		public static float maxDistance;

		// Token: 0x04003126 RID: 12582
		private float duration;

		// Token: 0x04003127 RID: 12583
		private Ray modifiedAimRay;
	}
}
