using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Wisp1Monster
{
	// Token: 0x02000724 RID: 1828
	public class FireEmbers : BaseState
	{
		// Token: 0x06002A89 RID: 10889 RVA: 0x000B30EC File Offset: 0x000B12EC
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FireEmbers.baseDuration / this.attackSpeedStat;
			Util.PlayScaledSound(FireEmbers.attackString, base.gameObject, this.attackSpeedStat);
			Ray aimRay = base.GetAimRay();
			base.StartAimMode(aimRay, 2f, false);
			base.PlayAnimation("Body", "FireAttack1", "FireAttack1.playbackRate", this.duration);
			string muzzleName = "";
			if (FireEmbers.effectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(FireEmbers.effectPrefab, base.gameObject, muzzleName, false);
			}
			if (base.isAuthority)
			{
				new BulletAttack
				{
					owner = base.gameObject,
					weapon = base.gameObject,
					origin = aimRay.origin,
					aimVector = aimRay.direction,
					minSpread = FireEmbers.minSpread,
					maxSpread = FireEmbers.maxSpread,
					bulletCount = (uint)((FireEmbers.bulletCount > 0) ? FireEmbers.bulletCount : 0),
					damage = FireEmbers.damageCoefficient * this.damageStat,
					force = FireEmbers.force,
					tracerEffectPrefab = FireEmbers.tracerEffectPrefab,
					muzzleName = muzzleName,
					hitEffectPrefab = FireEmbers.hitEffectPrefab,
					isCrit = Util.CheckRoll(this.critStat, base.characterBody.master),
					falloffModel = BulletAttack.FalloffModel.DefaultBullet,
					HitEffectNormal = false,
					radius = 0.5f,
					procCoefficient = 1f / (float)FireEmbers.bulletCount
				}.Fire();
			}
		}

		// Token: 0x06002A8A RID: 10890 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06002A8B RID: 10891 RVA: 0x000B326F File Offset: 0x000B146F
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06002A8C RID: 10892 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04002667 RID: 9831
		public static GameObject effectPrefab;

		// Token: 0x04002668 RID: 9832
		public static GameObject hitEffectPrefab;

		// Token: 0x04002669 RID: 9833
		public static GameObject tracerEffectPrefab;

		// Token: 0x0400266A RID: 9834
		public static float damageCoefficient;

		// Token: 0x0400266B RID: 9835
		public static float force;

		// Token: 0x0400266C RID: 9836
		public static float minSpread;

		// Token: 0x0400266D RID: 9837
		public static float maxSpread;

		// Token: 0x0400266E RID: 9838
		public static int bulletCount;

		// Token: 0x0400266F RID: 9839
		public static float baseDuration = 2f;

		// Token: 0x04002670 RID: 9840
		public static string attackString;

		// Token: 0x04002671 RID: 9841
		private float duration;
	}
}
