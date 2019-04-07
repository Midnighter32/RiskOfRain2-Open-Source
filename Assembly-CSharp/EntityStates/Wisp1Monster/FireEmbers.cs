using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Wisp1Monster
{
	// Token: 0x020000C8 RID: 200
	public class FireEmbers : BaseState
	{
		// Token: 0x060003E4 RID: 996 RVA: 0x00010100 File Offset: 0x0000E300
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
				EffectManager.instance.SimpleMuzzleFlash(FireEmbers.effectPrefab, base.gameObject, muzzleName, false);
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

		// Token: 0x060003E5 RID: 997 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x060003E6 RID: 998 RVA: 0x00010290 File Offset: 0x0000E490
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x060003E7 RID: 999 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x040003A3 RID: 931
		public static GameObject effectPrefab;

		// Token: 0x040003A4 RID: 932
		public static GameObject hitEffectPrefab;

		// Token: 0x040003A5 RID: 933
		public static GameObject tracerEffectPrefab;

		// Token: 0x040003A6 RID: 934
		public static float damageCoefficient;

		// Token: 0x040003A7 RID: 935
		public static float force;

		// Token: 0x040003A8 RID: 936
		public static float minSpread;

		// Token: 0x040003A9 RID: 937
		public static float maxSpread;

		// Token: 0x040003AA RID: 938
		public static int bulletCount;

		// Token: 0x040003AB RID: 939
		public static float baseDuration = 2f;

		// Token: 0x040003AC RID: 940
		public static string attackString;

		// Token: 0x040003AD RID: 941
		private float duration;
	}
}
