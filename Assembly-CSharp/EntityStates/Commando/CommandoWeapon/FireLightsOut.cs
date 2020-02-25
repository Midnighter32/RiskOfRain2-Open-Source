using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Commando.CommandoWeapon
{
	// Token: 0x020008BA RID: 2234
	public class FireLightsOut : BaseState
	{
		// Token: 0x06003215 RID: 12821 RVA: 0x000D8448 File Offset: 0x000D6648
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
				EffectManager.SimpleMuzzleFlash(FireLightsOut.effectPrefab, base.gameObject, muzzleName, false);
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

		// Token: 0x06003216 RID: 12822 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06003217 RID: 12823 RVA: 0x000D85FF File Offset: 0x000D67FF
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06003218 RID: 12824 RVA: 0x0000AC89 File Offset: 0x00008E89
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Any;
		}

		// Token: 0x040030CD RID: 12493
		public static GameObject effectPrefab;

		// Token: 0x040030CE RID: 12494
		public static GameObject hitEffectPrefab;

		// Token: 0x040030CF RID: 12495
		public static GameObject tracerEffectPrefab;

		// Token: 0x040030D0 RID: 12496
		public static float damageCoefficient;

		// Token: 0x040030D1 RID: 12497
		public static float force;

		// Token: 0x040030D2 RID: 12498
		public static float minSpread;

		// Token: 0x040030D3 RID: 12499
		public static float maxSpread;

		// Token: 0x040030D4 RID: 12500
		public static int bulletCount;

		// Token: 0x040030D5 RID: 12501
		public static float baseDuration = 2f;

		// Token: 0x040030D6 RID: 12502
		public static string attackSoundString;

		// Token: 0x040030D7 RID: 12503
		public static float recoilAmplitude;

		// Token: 0x040030D8 RID: 12504
		private ChildLocator childLocator;

		// Token: 0x040030D9 RID: 12505
		public int bulletCountCurrent = 1;

		// Token: 0x040030DA RID: 12506
		private float duration;
	}
}
