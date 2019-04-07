using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Commando.CommandoWeapon
{
	// Token: 0x020001A7 RID: 423
	internal class FireLightsOut : BaseState
	{
		// Token: 0x0600083C RID: 2108 RVA: 0x0002934C File Offset: 0x0002754C
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FireLightsOut.baseDuration / this.attackSpeedStat;
			base.AddRecoil(-3f * FireLightsOut.recoilAmplitude, -4f * FireLightsOut.recoilAmplitude, -0.5f * FireLightsOut.recoilAmplitude, 0.5f * FireLightsOut.recoilAmplitude);
			Ray aimRay = base.GetAimRay();
			base.StartAimMode(aimRay, 2f, false);
			string muzzleName = "MuzzlePistol";
			Util.PlaySound(FireLightsOut.attackSoundString, base.gameObject);
			base.PlayAnimation("Gesture, Additive", "FireRevolver");
			base.PlayAnimation("Gesture, Override", "FireRevolver");
			if (FireLightsOut.effectPrefab)
			{
				EffectManager.instance.SimpleMuzzleFlash(FireLightsOut.effectPrefab, base.gameObject, muzzleName, false);
			}
			if (base.isAuthority)
			{
				BulletAttack bulletAttack = new BulletAttack();
				bulletAttack.owner = base.gameObject;
				bulletAttack.weapon = base.gameObject;
				bulletAttack.origin = aimRay.origin;
				bulletAttack.aimVector = aimRay.direction;
				bulletAttack.minSpread = FireLightsOut.minSpread;
				bulletAttack.maxSpread = FireLightsOut.maxSpread;
				bulletAttack.bulletCount = (uint)((FireLightsOut.bulletCount > 0) ? FireLightsOut.bulletCount : 0);
				bulletAttack.damage = FireLightsOut.damageCoefficient * this.damageStat;
				bulletAttack.force = FireLightsOut.force;
				bulletAttack.falloffModel = BulletAttack.FalloffModel.None;
				bulletAttack.tracerEffectPrefab = FireLightsOut.tracerEffectPrefab;
				bulletAttack.muzzleName = muzzleName;
				bulletAttack.hitEffectPrefab = FireLightsOut.hitEffectPrefab;
				bulletAttack.isCrit = Util.CheckRoll(this.critStat, base.characterBody.master);
				bulletAttack.HitEffectNormal = false;
				bulletAttack.radius = 0.5f;
				bulletAttack.damageType |= DamageType.ResetCooldownsOnKill;
				bulletAttack.smartCollision = true;
				bulletAttack.Fire();
			}
		}

		// Token: 0x0600083D RID: 2109 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x0600083E RID: 2110 RVA: 0x00029508 File Offset: 0x00027708
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x0600083F RID: 2111 RVA: 0x0000A1ED File Offset: 0x000083ED
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Any;
		}

		// Token: 0x04000AE3 RID: 2787
		public static GameObject effectPrefab;

		// Token: 0x04000AE4 RID: 2788
		public static GameObject hitEffectPrefab;

		// Token: 0x04000AE5 RID: 2789
		public static GameObject tracerEffectPrefab;

		// Token: 0x04000AE6 RID: 2790
		public static float damageCoefficient;

		// Token: 0x04000AE7 RID: 2791
		public static float force;

		// Token: 0x04000AE8 RID: 2792
		public static float minSpread;

		// Token: 0x04000AE9 RID: 2793
		public static float maxSpread;

		// Token: 0x04000AEA RID: 2794
		public static int bulletCount;

		// Token: 0x04000AEB RID: 2795
		public static float baseDuration = 2f;

		// Token: 0x04000AEC RID: 2796
		public static string attackSoundString;

		// Token: 0x04000AED RID: 2797
		public static float recoilAmplitude;

		// Token: 0x04000AEE RID: 2798
		private ChildLocator childLocator;

		// Token: 0x04000AEF RID: 2799
		public int bulletCountCurrent = 1;

		// Token: 0x04000AF0 RID: 2800
		private float duration;
	}
}
