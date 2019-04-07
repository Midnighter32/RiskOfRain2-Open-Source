using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Mage.Weapon
{
	// Token: 0x0200011B RID: 283
	internal class IceNova : BaseState
	{
		// Token: 0x0600056F RID: 1391 RVA: 0x00018B16 File Offset: 0x00016D16
		public override void OnEnter()
		{
			base.OnEnter();
			this.stopwatch = 0f;
			this.endDuration = IceNova.baseEndDuration / this.attackSpeedStat;
			this.startDuration = IceNova.baseStartDuration / this.attackSpeedStat;
		}

		// Token: 0x06000570 RID: 1392 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06000571 RID: 1393 RVA: 0x00018B50 File Offset: 0x00016D50
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (this.stopwatch >= this.startDuration && !this.hasCastNova)
			{
				this.hasCastNova = true;
				EffectManager.instance.SpawnEffect(IceNova.novaEffectPrefab, new EffectData
				{
					origin = base.transform.position,
					scale = IceNova.novaRadius
				}, true);
				BlastAttack blastAttack = new BlastAttack();
				blastAttack.radius = IceNova.novaRadius;
				blastAttack.procCoefficient = IceNova.procCoefficient;
				blastAttack.position = base.transform.position;
				blastAttack.attacker = base.gameObject;
				blastAttack.crit = Util.CheckRoll(base.characterBody.crit, base.characterBody.master);
				blastAttack.baseDamage = base.characterBody.damage * IceNova.damageCoefficient;
				blastAttack.falloffModel = BlastAttack.FalloffModel.None;
				blastAttack.damageType = DamageType.Freeze2s;
				blastAttack.baseForce = IceNova.force;
				blastAttack.teamIndex = TeamComponent.GetObjectTeam(blastAttack.attacker);
				blastAttack.Fire();
			}
			if (this.stopwatch >= this.startDuration + this.endDuration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06000572 RID: 1394 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x0400060B RID: 1547
		public static GameObject impactEffectPrefab;

		// Token: 0x0400060C RID: 1548
		public static GameObject novaEffectPrefab;

		// Token: 0x0400060D RID: 1549
		public static float baseStartDuration;

		// Token: 0x0400060E RID: 1550
		public static float baseEndDuration = 2f;

		// Token: 0x0400060F RID: 1551
		public static float damageCoefficient = 1.2f;

		// Token: 0x04000610 RID: 1552
		public static float procCoefficient;

		// Token: 0x04000611 RID: 1553
		public static float force = 20f;

		// Token: 0x04000612 RID: 1554
		public static float novaRadius;

		// Token: 0x04000613 RID: 1555
		public static string attackString;

		// Token: 0x04000614 RID: 1556
		private float stopwatch;

		// Token: 0x04000615 RID: 1557
		private float startDuration;

		// Token: 0x04000616 RID: 1558
		private float endDuration;

		// Token: 0x04000617 RID: 1559
		private bool hasCastNova;
	}
}
